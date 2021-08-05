FROM mcr.microsoft.com/dotnet/core/sdk:3.1-bionic

WORKDIR /buildinternal
# unzip, zip, python, build-essential (make, gcc and more) ...
RUN apt-get update && \
    apt-get -y --fix-missing install ca-certificates apt-utils curl rsync unzip zip python3 python-pip groff openssh-client git-core build-essential jq && \
    apt-get clean
RUN rm -rf /var/lib/apt/lists/*

RUN update-alternatives --install /usr/bin/python python /usr/bin/python3.6 1

ENV TERRAFORM_AWS_PLUGIN_VERSION 3.52.0 
ENV TERRAFORM_TEMPLATE_PLUGIN_VERSION 2.2.0
ENV TERRAFORM_TERRAFORM_PLUGIN_VERSION 2.2.0

#create terraforms default local provider structure
RUN mkdir -p ~/.terraform.d ~/.terraform.d/plugins -p ~/.terraform.d/plugins/registry.terraform.io/ ~/.terraform.d/plugins/registry.terraform.io/hashicorp/ && \
    mkdir -p ~/.terraform.d/plugins/registry.terraform.io/hashicorp/aws && \ 
    mkdir -p ~/.terraform.d/plugins/registry.terraform.io/hashicorp/template && \
    mkdir -p ~/.terraform.d/plugins/registry.terraform.io/hashicorp/archive && \
    cd ~/.terraform.d/plugins/registry.terraform.io/hashicorp/ && \
    mkdir -p aws/${TERRAFORM_AWS_PLUGIN_VERSION} aws/${TERRAFORM_AWS_PLUGIN_VERSION}/linux_amd64 && \
    mkdir -p template/${TERRAFORM_TEMPLATE_PLUGIN_VERSION} template/${TERRAFORM_TEMPLATE_PLUGIN_VERSION}/linux_amd64 && \
    mkdir -p archive/${TERRAFORM_TERRAFORM_PLUGIN_VERSION} archive/${TERRAFORM_TERRAFORM_PLUGIN_VERSION}/linux_amd64
    
# packer
ENV PACKER_VERSION 1.2.5
ENV PACKER_CHECKSUM bc58aa3f3db380b76776e35f69662b49f3cf15cf80420fc81a15ce971430824c
RUN curl -fsSL https://releases.hashicorp.com/packer/${PACKER_VERSION}/packer_${PACKER_VERSION}_linux_amd64.zip -o packer.zip  && \
    echo "${PACKER_CHECKSUM} packer.zip" | sha256sum -c - && \
    unzip packer.zip -d /usr/local/bin && chmod +x /usr/local/bin/packer ; rm packer.zip

# terraform
ENV TERRAFORM_VERSION 0.14.11
ENV TERRAFORM_CHECKSUM 171ef5a4691b6f86eab524feaf9a52d5221c875478bd63dd7e55fef3939f7fd4
RUN curl -fsSL https://releases.hashicorp.com/terraform/${TERRAFORM_VERSION}/terraform_${TERRAFORM_VERSION}_linux_amd64.zip -o terraform.zip  && \
    echo "${TERRAFORM_CHECKSUM} terraform.zip" | sha256sum -c - && \
    unzip terraform.zip -d /usr/local/bin && chmod +x /usr/local/bin/terraform ; rm terraform.zip

# terraform template provider plugin
ENV TERRAFORM_TEMPLATE_PLUGIN_CHECKSUM 8a154388f3708e3df5a69122a23bdfaf760a523788a5081976b3d5616f7d30ae
RUN curl -fsSL https://releases.hashicorp.com/terraform-provider-template/${TERRAFORM_TEMPLATE_PLUGIN_VERSION}/terraform-provider-template_${TERRAFORM_TEMPLATE_PLUGIN_VERSION}_linux_amd64.zip -o terraform_template_plugin.zip  && \
    echo "${TERRAFORM_TEMPLATE_PLUGIN_CHECKSUM} terraform_template_plugin.zip" | sha256sum -c - && \
    unzip terraform_template_plugin.zip -d ~/.terraform.d/plugins/registry.terraform.io/hashicorp/template/${TERRAFORM_TEMPLATE_PLUGIN_VERSION}/linux_amd64/ ; rm terraform_template_plugin.zip

# terraform aws provider plugin
ENV TERRAFORM_AWS_PLUGIN_CHECKSUM 2e0432fabeb5e44d756a5566168768f1b6dea3cc0e5650fac966820e90d18367
RUN curl -fsSL https://releases.hashicorp.com/terraform-provider-aws/${TERRAFORM_AWS_PLUGIN_VERSION}/terraform-provider-aws_${TERRAFORM_AWS_PLUGIN_VERSION}_linux_amd64.zip -o terraform_aws_plugin.zip  && \
    echo "${TERRAFORM_AWS_PLUGIN_CHECKSUM} terraform_aws_plugin.zip" | sha256sum -c - && \
    unzip terraform_aws_plugin.zip -d ~/.terraform.d/plugins/registry.terraform.io/hashicorp/aws/${TERRAFORM_AWS_PLUGIN_VERSION}/linux_amd64/  ; rm terraform_aws_plugin.zip

# terraform terraform provider archive (for lambda)
ENV TERRAFORM_TERRAFORM_PLUGIN_CHECKSUM e63f12ea938520b3f83634fc29da28d92eed5cfbc5cc8ca08281a6a9c36cca65
RUN curl -fsSL https://releases.hashicorp.com/terraform-provider-archive/${TERRAFORM_TERRAFORM_PLUGIN_VERSION}/terraform-provider-archive_${TERRAFORM_TERRAFORM_PLUGIN_VERSION}_linux_amd64.zip -o terraform_terraform_plugin.zip  && \
    echo "${TERRAFORM_TERRAFORM_PLUGIN_CHECKSUM} terraform_terraform_plugin.zip" | sha256sum -c - && \
    unzip terraform_terraform_plugin.zip -d ~/.terraform.d/plugins/registry.terraform.io/hashicorp/archive/${TERRAFORM_TERRAFORM_PLUGIN_VERSION}/linux_amd64/ ; rm terraform_terraform_plugin.zip

RUN curl https://awscli.amazonaws.com/awscli-exe-linux-x86_64.zip -o awscliv2.zip && \
    unzip awscliv2.zip && \
    aws/install -i /usr/local/aws -b /usr/local/bin && \
    rm awscliv2.zip && rm -rf ./aws

COPY umask.sh /
RUN chmod +x /umask.sh


ENTRYPOINT [ "/umask.sh" ]