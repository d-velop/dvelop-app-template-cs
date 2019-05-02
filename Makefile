APP_NAME=acme-apptemplatecs
DOMAIN_SUFFIX=.hackathon.service.d-velop.cloud
BUILD_VERSION=rev.$(shell git rev-parse --short HEAD).date.$(shell date '+%d-%m-%Y-%H.%M.%S')

all: build

generate: 
	echo "Start Build"

clean: init
	echo "You can only clean restored dotnet projects, cleaning solution"
	dotnet clean

init:
	echo "Create directories, if not present"
	mkdir -p /build/dist
	echo "Copy all source from the mount into the container, to enable deployment and editing with an IDE at the same time"
	rsync -r --verbose --exclude '.terraform' --exclude '.git' --exclude 'dist' --exclude '**/obj**' --exclude \
		'**/bin/**' --exclude terraform --exclude '.*' --exclude 'TestResults' /build/ /buildinternal/
	echo "Init project and download dependency from nuget-repository"
	dotnet restore
	

test: init
	echo "Executing all tests"
	dotnet test "--logger:trx"

build: clean build-app build-lambda

build-app: generate test
	echo "Building app for Windows..."
	dotnet publish --self-contained -r win-x64 -c Release ./SelfHosted/HostApplication/HostApplication.csproj
	cd /buildinternal/SelfHosted/HostApplication/bin/Release/netcoreapp2.1/win-x64/publish/ && 	\
		echo "Creating windows_<rev>.zip from: " && \
		zip -x wwwroot/dvelop-dux/images\* -r /build/dist/windows_$(BUILD_VERSION).zip .

build-lambda: generate test
	echo "Building lambda..."
	dotnet publish -c Release ./AwsLambda/Entrypoint/EntryPoint.csproj
	cd /buildinternal/AwsLambda/Entrypoint/bin/Release/netcoreapp2.1/publish/ && \
		echo "Creating lambda.zip from: " && \
		zip -x wwwroot\* -r /build/dist/lambda.zip .

tf-init:
	echo "Initializing terraform"
	cd /build/terraform && \
	terraform init -input=false -plugin-dir=/usr/local/lib/custom-terraform-plugins

plan: tf-init build-lambda asset_hash
	echo "Planning terraform changes"
	$(eval PLAN=$(shell mktemp))
	cd /build/terraform && \
	terraform plan -input=false \
	-var "signature_secret=$(SIGNATURE_SECRET)" \
	-var "build_version=$(BUILD_VERSION)" \
	-var "appname=$(APP_NAME)" \
	-var "domainsuffix=$(DOMAIN_SUFFIX)" \
	-var "asset_hash=$(ASSET_HASH)" \
	-out=$(PLAN)

apply: plan
	echo "Applying terraform changes"
	cd /build/terraform && \
	terraform apply -input=false -auto-approve=true $(PLAN)

deploy-assets: asset_hash apply
	echo "Deploying static content to S3"
	# best practice for immutable content: cache 1 year (vgl https://jakearchibald.com/2016/caching-best-practices/)
	aws s3 sync /buildinternal/AwsLambda/Entrypoint/bin/Release/netcoreapp2.1/publish/wwwroot s3://assets.$(APP_NAME)$(DOMAIN_SUFFIX)/$(ASSET_HASH) --exclude "*.html" --cache-control max-age=31536000

asset_hash:
	echo "Creating hash for static content to create a cachable path within S3"
	$(eval ASSET_HASH=$(shell find web -type f ! -path "*.html" -exec md5sum {} \; | sort -k 2 | md5sum | tr -d " -"))

deploy: apply deploy-assets
	echo "Deployment of AWS Resources finished"

show: tf-init
	echo "Show actual AWS Resources"
	cd /build/terraform && \
	terraform show -input=false
	
rename:
	if [ -z $${NAME} ]; then echo "NAME is not set. Usage: rename NAME=NEW_APP_NAME"; exit 1; fi
	@echo Rename App to $(NAME) ...
	find . -name "docker-build.*" -or -name "Makefile" -or -name "*.tf" -or -name "*.cs" | while read f; do		\
		echo "Processing file '$$f'";															\
		sed -i 's/$(APP_NAME)/$(NAME)/g' $$f;														\
	done

destroy: tf-init
	echo "destroy is disabled. Uncomment in Makefile to enable destroy."
	cd /build/terraform && \
	terraform destroy -var "signature_secret=$(SIGNATURE_SECRET)" \
	-var "build_version=$(BUILD_VERSION)" \
	-var "appname=$(APP_NAME)" \
	-var "domainsuffix=$(DOMAIN_SUFFIX)" \
	-var "asset_hash=$(ASSET_HASH)" \
	-input=false -force

dns: tf-init	
	cd /build/terraform && terraform output -input=false -json | jq "{Domain: .domain.value, Nameserver: .nameserver.value}" > ../dist/dns-entry.json