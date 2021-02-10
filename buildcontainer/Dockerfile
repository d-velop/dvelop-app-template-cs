FROM mcr.microsoft.com/dotnet/core/sdk:3.1-bionic

WORKDIR /buildinternal
# unzip, zip, python, build-essential (make, gcc and more) ...
RUN apt-get update && \
    apt-get -y --fix-missing install ca-certificates apt-utils curl rsync unzip zip python python-pip openssh-client git-core build-essential jq && \
    apt-get clean     
RUN rm -rf /var/lib/apt/lists/*

# packer
ENV PACKER_VERSION 1.2.5
ENV PACKER_CHECKSUM bc58aa3f3db380b76776e35f69662b49f3cf15cf80420fc81a15ce971430824c
RUN curl -fsSL https://releases.hashicorp.com/packer/${PACKER_VERSION}/packer_${PACKER_VERSION}_linux_amd64.zip -o packer.zip  && \
    echo "${PACKER_CHECKSUM} packer.zip" | sha256sum -c - && \
    unzip packer.zip -d /usr/local/bin && chmod +x /usr/local/bin/packer ; rm packer.zip

# terraform
ENV TERRAFORM_VERSION 0.12.26
ENV TERRAFORM_CHECKSUM 607bc802b1c6c2a5e62cc48640f38aaa64bef1501b46f0ae4829feb51594b257
RUN curl -fsSL https://releases.hashicorp.com/terraform/${TERRAFORM_VERSION}/terraform_${TERRAFORM_VERSION}_linux_amd64.zip -o terraform.zip  && \
    echo "${TERRAFORM_CHECKSUM} terraform.zip" | sha256sum -c - && \
    unzip terraform.zip -d /usr/local/bin && chmod +x /usr/local/bin/terraform ; rm terraform.zip

# terraform terraform provider plugin (for remote state)
ENV TERRAFORM_TERRAFORM_PLUGIN_VERSION 1.0.2
ENV TERRAFORM_TERRAFORM_PLUGIN_CHECKSUM c4c1e826c85ef45fb18ca75e06284d13b0f72bce61a652e74eb016cf7779eafd
RUN curl -fsSL https://releases.hashicorp.com/terraform-provider-terraform/${TERRAFORM_TERRAFORM_PLUGIN_VERSION}/terraform-provider-terraform_${TERRAFORM_TERRAFORM_PLUGIN_VERSION}_linux_amd64.zip -o terraform_terraform_plugin.zip  && \
    echo "${TERRAFORM_TERRAFORM_PLUGIN_CHECKSUM} terraform_terraform_plugin.zip" | sha256sum -c - && \
    unzip terraform_terraform_plugin.zip -d /usr/local/lib/custom-terraform-plugins ; rm terraform_terraform_plugin.zip


# terraform mongodbatlas provider plugin (for mongodbatlas)
ENV TERRAFORM_MONGODBATLAS_PLUGIN_VERSION 0.5.1
ENV TERRAFORM_MONGODBATLAS_PLUGIN_CHECKSUM 70166a9f2022f1ff96a1841648a8282a4cfcc38a6dc592df7b9a78a600d3d87c
RUN curl -fsSL https://releases.hashicorp.com/terraform-provider-mongodbatlas/${TERRAFORM_MONGODBATLAS_PLUGIN_VERSION}/terraform-provider-mongodbatlas_${TERRAFORM_MONGODBATLAS_PLUGIN_VERSION}_linux_amd64.zip -o terraform_mongodbatlas_plugin.zip  && \
    echo "${TERRAFORM_MONGODBATLAS_PLUGIN_CHECKSUM} terraform_mongodbatlas_plugin.zip" | sha256sum -c - && \
    unzip terraform_mongodbatlas_plugin.zip -d /usr/local/lib/custom-terraform-plugins ; rm terraform_mongodbatlas_plugin.zip

# terraform template provider plugin
ENV TERRAFORM_TEMPLATE_PLUGIN_VERSION 2.1.2
ENV TERRAFORM_TEMPLATE_PLUGIN_CHECKSUM 149e4bf47ac21b67f6567767afcd29caaf0b0ca43714748093a00a2a98cd17a8
RUN curl -fsSL https://releases.hashicorp.com/terraform-provider-template/${TERRAFORM_TEMPLATE_PLUGIN_VERSION}/terraform-provider-template_${TERRAFORM_TEMPLATE_PLUGIN_VERSION}_linux_amd64.zip -o terraform_template_plugin.zip  && \
    echo "${TERRAFORM_TEMPLATE_PLUGIN_CHECKSUM} terraform_template_plugin.zip" | sha256sum -c - && \
    unzip terraform_template_plugin.zip -d /usr/local/lib/custom-terraform-plugins ; rm terraform_template_plugin.zip

# terraform aws provider plugin
ENV TERRAFORM_AWS_PLUGIN_VERSION 2.64.0 
ENV TERRAFORM_AWS_PLUGIN_CHECKSUM faf2b833298d5a958a94963f1b0a5d3501b80725148a7fb81e5535f2ecba9edf
RUN curl -fsSL https://releases.hashicorp.com/terraform-provider-aws/${TERRAFORM_AWS_PLUGIN_VERSION}/terraform-provider-aws_${TERRAFORM_AWS_PLUGIN_VERSION}_linux_amd64.zip -o terraform_aws_plugin.zip  && \
    echo "${TERRAFORM_AWS_PLUGIN_CHECKSUM} terraform_aws_plugin.zip" | sha256sum -c - && \
    unzip terraform_aws_plugin.zip -d /usr/local/lib/custom-terraform-plugins ; rm terraform_aws_plugin.zip

# terraform terraform provider archive (for lambda)
ENV TERRAFORM_TERRAFORM_PLUGIN_VERSION 1.3.0
ENV TERRAFORM_TERRAFORM_PLUGIN_CHECKSUM e0d1213625d40d124bd9570f0d92907416f8d61bc8c389c776e72c0a97020cce
RUN curl -fsSL https://releases.hashicorp.com/terraform-provider-archive/${TERRAFORM_TERRAFORM_PLUGIN_VERSION}/terraform-provider-archive_${TERRAFORM_TERRAFORM_PLUGIN_VERSION}_linux_amd64.zip -o terraform_terraform_plugin.zip  && \
    echo "${TERRAFORM_TERRAFORM_PLUGIN_CHECKSUM} terraform_terraform_plugin.zip" | sha256sum -c - && \
    unzip terraform_terraform_plugin.zip -d /usr/local/lib/custom-terraform-plugins ; rm terraform_terraform_plugin.zip

# aws cli
RUN curl -sSL https://s3.amazonaws.com/aws-cli/awscli-bundle.zip -o awscli-bundle.zip && \
    unzip awscli-bundle.zip && \
    ./awscli-bundle/install -i /usr/local/aws -b /usr/local/bin/aws && \
    rm awscli-bundle.zip && rm -rf ./awscli-bundle

COPY umask.sh /
RUN chmod +x /umask.sh


ENTRYPOINT [ "/umask.sh" ]