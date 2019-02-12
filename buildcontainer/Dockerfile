FROM microsoft/dotnet 
WORKDIR /build

RUN apt-get update

# curl
RUN apt-get -y install apt-utils curl && \
    apt-get -y --fix-missing install --no-install-recommends unzip zip python python-pip openssh-client git-core build-essential jq && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*

# packer
ENV PACKER_VERSION 1.2.5
ENV PACKER_CHECKSUM bc58aa3f3db380b76776e35f69662b49f3cf15cf80420fc81a15ce971430824c
RUN curl -fsSL https://releases.hashicorp.com/packer/${PACKER_VERSION}/packer_${PACKER_VERSION}_linux_amd64.zip -o packer.zip  && \
    echo "${PACKER_CHECKSUM} packer.zip" | sha256sum -c - && \
    unzip packer.zip -d /usr/local/bin && chmod +x /usr/local/bin/packer ; rm packer.zip

# terraform
ENV TERRAFORM_VERSION 0.11.8
ENV TERRAFORM_CHECKSUM 84ccfb8e13b5fce63051294f787885b76a1fedef6bdbecf51c5e586c9e20c9b7
RUN curl -fsSL https://releases.hashicorp.com/terraform/${TERRAFORM_VERSION}/terraform_${TERRAFORM_VERSION}_linux_amd64.zip -o terraform.zip  && \
    echo "${TERRAFORM_CHECKSUM} terraform.zip" | sha256sum -c - && \
    unzip terraform.zip -d /usr/local/bin && chmod +x /usr/local/bin/terraform ; rm terraform.zip

# terraform terraform provider plugin (for remote state)
ENV TERRAFORM_TERRAFORM_PLUGIN_VERSION 1.0.2
ENV TERRAFORM_TERRAFORM_PLUGIN_CHECKSUM c4c1e826c85ef45fb18ca75e06284d13b0f72bce61a652e74eb016cf7779eafd
RUN curl -fsSL https://releases.hashicorp.com/terraform-provider-terraform/${TERRAFORM_TERRAFORM_PLUGIN_VERSION}/terraform-provider-terraform_${TERRAFORM_TERRAFORM_PLUGIN_VERSION}_linux_amd64.zip -o terraform_terraform_plugin.zip  && \
    echo "${TERRAFORM_TERRAFORM_PLUGIN_CHECKSUM} terraform_terraform_plugin.zip" | sha256sum -c - && \
    unzip terraform_terraform_plugin.zip -d /usr/local/lib/custom-terraform-plugins ; rm terraform_terraform_plugin.zip

# terraform template provider plugin
ENV TERRAFORM_TEMPLATE_PLUGIN_VERSION 1.0.0
ENV TERRAFORM_TEMPLATE_PLUGIN_CHECKSUM f54c2764bd4d4c62c1c769681206dde7aa94b64b814a43cb05680f1ec8911977
RUN curl -fsSL https://releases.hashicorp.com/terraform-provider-template/${TERRAFORM_TEMPLATE_PLUGIN_VERSION}/terraform-provider-template_${TERRAFORM_TEMPLATE_PLUGIN_VERSION}_linux_amd64.zip -o terraform_template_plugin.zip  && \
    echo "${TERRAFORM_TEMPLATE_PLUGIN_CHECKSUM} terraform_template_plugin.zip" | sha256sum -c - && \
    unzip terraform_template_plugin.zip -d /usr/local/lib/custom-terraform-plugins ; rm terraform_template_plugin.zip

# terraform aws provider plugin
ENV TERRAFORM_AWS_PLUGIN_VERSION 1.31.0
ENV TERRAFORM_AWS_PLUGIN_CHECKSUM c64b54d3778133795845d1294c62be5b011c407ebb7b8de66f6de3ad31aeb95b
RUN curl -fsSL https://releases.hashicorp.com/terraform-provider-aws/${TERRAFORM_AWS_PLUGIN_VERSION}/terraform-provider-aws_${TERRAFORM_AWS_PLUGIN_VERSION}_linux_amd64.zip -o terraform_aws_plugin.zip  && \
    echo "${TERRAFORM_AWS_PLUGIN_CHECKSUM} terraform_aws_plugin.zip" | sha256sum -c - && \
    unzip terraform_aws_plugin.zip -d /usr/local/lib/custom-terraform-plugins ; rm terraform_aws_plugin.zip

# terraform null provider plugin
ENV TERRAFORM_NULL_PLUGIN_VERSION 1.0.0
ENV TERRAFORM_NULL_PLUGIN_CHECKSUM e0cb71b742e7feb69aa618b91c4cf5cb36e403e3a637c29c5d27cb201b1b68f5
RUN curl -fsSL https://releases.hashicorp.com/terraform-provider-null/${TERRAFORM_NULL_PLUGIN_VERSION}/terraform-provider-null_${TERRAFORM_NULL_PLUGIN_VERSION}_linux_amd64.zip -o terraform_null_plugin.zip  && \
    echo "${TERRAFORM_NULL_PLUGIN_CHECKSUM} terraform_null_plugin.zip" | sha256sum -c - && \
    unzip terraform_null_plugin.zip -d /usr/local/lib/custom-terraform-plugins ; rm terraform_null_plugin.zip

# terraform archive provider plugin
ENV TERRAFORM_ARCHIVE_PLUGIN_VERSION 1.1.0
ENV TERRAFORM_ARCHIVE_PLUGIN_CHECKSUM b277ad7326391438656c2432e438f369f3bab144771ef0aa5c7e842672b7f231
RUN curl -fsSL https://releases.hashicorp.com/terraform-provider-archive/${TERRAFORM_ARCHIVE_PLUGIN_VERSION}/terraform-provider-archive_${TERRAFORM_ARCHIVE_PLUGIN_VERSION}_linux_amd64.zip -o terraform_archive_plugin.zip  && \
    echo "${TERRAFORM_ARCHIVE_PLUGIN_CHECKSUM} terraform_archive_plugin.zip" | sha256sum -c - && \
    unzip terraform_archive_plugin.zip -d /usr/local/lib/custom-terraform-plugins ; rm terraform_archive_plugin.zip

# aws cli
RUN curl -sSL https://s3.amazonaws.com/aws-cli/awscli-bundle.zip -o awscli-bundle.zip && \
    unzip awscli-bundle.zip && \
    ./awscli-bundle/install -i /usr/local/aws -b /usr/local/bin/aws && \
    rm awscli-bundle.zip && rm -rf ./awscli-bundle

COPY umask.sh /
RUN chmod +x /umask.sh
	
WORKDIR /build
ENTRYPOINT [ "/umask.sh" ]