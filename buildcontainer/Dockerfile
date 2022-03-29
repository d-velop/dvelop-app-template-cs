FROM mcr.microsoft.com/dotnet/sdk:6.0-focal

RUN wget https://packages.microsoft.com/config/ubuntu/21.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb && \
    dpkg -i packages-microsoft-prod.deb


WORKDIR /buildinternal
# unzip, zip, python, build-essential (make, gcc and more) ...
RUN echo 'debconf debconf/frontend select Noninteractive' | debconf-set-selections

RUN apt-get update && \
    apt-get install -y apt-transport-https

RUN apt-get update && \
	apt-get -y --fix-missing install ca-certificates apt-utils curl rsync unzip zip python3 python3-distutils python3-pip openssh-client git-core groff build-essential jq


RUN rm -rf /var/lib/apt/lists/*

RUN update-alternatives --install /usr/bin/python python /usr/bin/python3.8 1

ENV TERRAFORM_VERSION 1.0.5
ENV TERRAFORM_CHECKSUM 7ce24478859ab7ca0ba4d8c9c12bb345f52e8efdc42fa3ef9dd30033dbf4b561
ENV TERRAFORM_AWS_PLUGIN_VERSION 3.75.1
ENV TERRAFORM_AWS_PLUGIN_CHECKSUM ecd664c0d664fcf2d8a89a01462cb00bcae37da200305aef2de1b8fe185c9cd8
ENV TERRAFORM_TEMPLATE_PLUGIN_VERSION 2.2.0
ENV TERRAFORM_TEMPLATE_PLUGIN_CHECKSUM 8a154388f3708e3df5a69122a23bdfaf760a523788a5081976b3d5616f7d30ae
ENV TERRAFORM_TERRAFORM_PLUGIN_VERSION 2.2.0
ENV TERRAFORM_TERRAFORM_PLUGIN_CHECKSUM e63f12ea938520b3f83634fc29da28d92eed5cfbc5cc8ca08281a6a9c36cca65
ENV TERRAFORM_ARCHIVE_PLUGIN_VERSION 2.2.0
ENV TERRAFORM_ARCHIVE_PLUGIN_CHECKSUM e63f12ea938520b3f83634fc29da28d92eed5cfbc5cc8ca08281a6a9c36cca65
ENV PACKER_VERSION 1.2.5
ENV PACKER_CHECKSUM bc58aa3f3db380b76776e35f69662b49f3cf15cf80420fc81a15ce971430824c

#create terraforms default local provider structure
RUN mkdir -p ~/.terraform.d ~/.terraform.d/plugins -p ~/.terraform.d/plugins/registry.terraform.io/ ~/.terraform.d/plugins/registry.terraform.io/hashicorp/ && \
    mkdir -p ~/.terraform.d/plugins/registry.terraform.io/hashicorp/aws && \ 
    mkdir -p ~/.terraform.d/plugins/registry.terraform.io/hashicorp/template && \
    mkdir -p ~/.terraform.d/plugins/registry.terraform.io/hashicorp/archive && \
    cd ~/.terraform.d/plugins/registry.terraform.io/hashicorp/ && \
    mkdir -p aws/${TERRAFORM_AWS_PLUGIN_VERSION} aws/${TERRAFORM_AWS_PLUGIN_VERSION}/linux_amd64 && \
    mkdir -p archive/${TERRAFORM_ARCHIVE_PLUGIN_VERSION} archive/${TERRAFORM_ARCHIVE_PLUGIN_VERSION}/linux_amd64 && \
    mkdir -p template/${TERRAFORM_TEMPLATE_PLUGIN_VERSION} template/${TERRAFORM_TEMPLATE_PLUGIN_VERSION}/linux_amd64 && \
    mkdir -p archive/${TERRAFORM_TERRAFORM_PLUGIN_VERSION} archive/${TERRAFORM_TERRAFORM_PLUGIN_VERSION}/linux_amd64
    
# packer

RUN curl -fsSL https://releases.hashicorp.com/packer/${PACKER_VERSION}/packer_${PACKER_VERSION}_linux_amd64.zip -o packer.zip  && \
    echo "${PACKER_CHECKSUM} packer.zip" | sha256sum -c - && \
    unzip packer.zip -d /usr/local/bin && chmod +x /usr/local/bin/packer ; rm packer.zip

# terraform
RUN curl -fsSL https://releases.hashicorp.com/terraform/${TERRAFORM_VERSION}/terraform_${TERRAFORM_VERSION}_linux_amd64.zip -o terraform.zip  && \
    echo "${TERRAFORM_CHECKSUM} terraform.zip" | sha256sum -c - && \
    unzip terraform.zip -d /usr/local/bin && chmod +x /usr/local/bin/terraform ; rm terraform.zip

# terraform template provider plugin
RUN curl -fsSL https://releases.hashicorp.com/terraform-provider-template/${TERRAFORM_TEMPLATE_PLUGIN_VERSION}/terraform-provider-template_${TERRAFORM_TEMPLATE_PLUGIN_VERSION}_linux_amd64.zip -o terraform_template_plugin.zip  && \
    echo "${TERRAFORM_TEMPLATE_PLUGIN_CHECKSUM} terraform_template_plugin.zip" | sha256sum -c - && \
    unzip terraform_template_plugin.zip -d ~/.terraform.d/plugins/registry.terraform.io/hashicorp/template/${TERRAFORM_TEMPLATE_PLUGIN_VERSION}/linux_amd64/ ; rm terraform_template_plugin.zip

# terraform aws provider plugin
RUN curl -fsSL https://releases.hashicorp.com/terraform-provider-aws/${TERRAFORM_AWS_PLUGIN_VERSION}/terraform-provider-aws_${TERRAFORM_AWS_PLUGIN_VERSION}_linux_amd64.zip -o terraform_aws_plugin.zip  && \
    echo "${TERRAFORM_AWS_PLUGIN_CHECKSUM} terraform_aws_plugin.zip" | sha256sum -c - && \
    unzip terraform_aws_plugin.zip -d ~/.terraform.d/plugins/registry.terraform.io/hashicorp/aws/${TERRAFORM_AWS_PLUGIN_VERSION}/linux_amd64/  ; rm terraform_aws_plugin.zip

# terraform terraform provider archive (for lambda)
RUN curl -fsSL https://releases.hashicorp.com/terraform-provider-archive/${TERRAFORM_TERRAFORM_PLUGIN_VERSION}/terraform-provider-archive_${TERRAFORM_TERRAFORM_PLUGIN_VERSION}_linux_amd64.zip -o terraform_terraform_plugin.zip  && \
    echo "${TERRAFORM_TERRAFORM_PLUGIN_CHECKSUM} terraform_terraform_plugin.zip" | sha256sum -c - && \
    unzip terraform_terraform_plugin.zip -d ~/.terraform.d/plugins/registry.terraform.io/hashicorp/archive/${TERRAFORM_TERRAFORM_PLUGIN_VERSION}/linux_amd64/ ; rm terraform_terraform_plugin.zip

RUN curl -fsSL https://releases.hashicorp.com/terraform-provider-archive/${TERRAFORM_ARCHIVE_PLUGIN_VERSION}/terraform-provider-archive_${TERRAFORM_ARCHIVE_PLUGIN_VERSION}_linux_amd64.zip -o terraform_terraform_plugin.zip  && \
    echo "${TERRAFORM_ARCHIVE_PLUGIN_CHECKSUM} terraform_terraform_plugin.zip" | sha256sum -c - && \
    unzip terraform_terraform_plugin.zip -d ~/.terraform.d/plugins/registry.terraform.io/hashicorp/archive/${TERRAFORM_ARCHIVE_PLUGIN_VERSION}/linux_amd64/ ; rm terraform_terraform_plugin.zip
  
# aws cliv2
RUN curl -sSL https://awscli.amazonaws.com/awscli-exe-linux-x86_64.zip -o awscliv2.zip && \
    unzip awscliv2.zip && \
    aws/install -i /usr/local/aws -b /usr/local/bin && \
    rm awscliv2.zip && rm -rf ./aws

# node
RUN curl -sL https://deb.nodesource.com/setup_16.x | bash -; \
	apt-get install -y nodejs
RUN npm install -g markdown4documentation

ENV VAULT_VERSION 1.8.2
RUN curl -fsSL https://releases.hashicorp.com/vault/${VAULT_VERSION}/vault_${VAULT_VERSION}_linux_amd64.zip -o vault_linux_amd64.zip  && \
    unzip vault_linux_amd64.zip -d /usr/local/bin && chmod +x /usr/local/bin/vault ; rm vault_linux_amd64.zip

RUN apt-get clean
RUN rm -rf /var/lib/apt/lists/*

ENV DOTNET_CLI_TELEMETRY_OPTOUT 1
RUN dotnet tool install -g trx2junit
ENV PATH="${PATH}:~/.dotnet/tools"

COPY umask.sh /
RUN chmod +x /umask.sh

ENTRYPOINT [ "/umask.sh" ]