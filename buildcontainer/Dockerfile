FROM mcr.microsoft.com/dotnet/core/sdk:3.1-focal

RUN wget https://packages.microsoft.com/config/ubuntu/21.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb && \
    dpkg -i packages-microsoft-prod.deb


WORKDIR /buildinternal
# unzip, zip, python, build-essential (make, gcc and more) ...
RUN echo 'debconf debconf/frontend select Noninteractive' | debconf-set-selections

RUN apt-get update && \
    apt-get install -y apt-transport-https

RUN apt-get update && \
    apt-get -y --fix-missing install dotnet-sdk-3.1 apt-utils sudo curl rsync unzip zip python3 python3-pip groff openssh-client git-core build-essential jq lsb-release software-properties-common

RUN rm -rf /var/lib/apt/lists/*

RUN update-alternatives --install /usr/bin/python python /usr/bin/python3.8 1

RUN curl -fsSL https://download.docker.com/linux/ubuntu/gpg | apt-key add -
RUN apt-key fingerprint 0EBFCD88
RUN add-apt-repository \
   "deb [arch=amd64] https://download.docker.com/linux/ubuntu \
   $(lsb_release -cs) \
   stable"
 
ENV TERRAFORM_MONGODBATLAS_PLUGIN_VERSION 1.0.1
ENV TERRAFORM_AWS_PLUGIN_VERSION 3.54.0
ENV TERRAFORM_ARCHIVE_PLUGIN_VERSION 2.2.0

#create terraforms default local provider structure
RUN mkdir -p ~/.terraform.d ~/.terraform.d/plugins -p ~/.terraform.d/plugins/registry.terraform.io/ ~/.terraform.d/plugins/registry.terraform.io/hashicorp/ && \
    mkdir -p ~/.terraform.d/plugins/registry.terraform.io/hashicorp/aws && \ 
    mkdir -p ~/.terraform.d/plugins/registry.terraform.io/hashicorp/archive && \
    cd ~/.terraform.d/plugins/registry.terraform.io/hashicorp/ && \
    mkdir -p aws/${TERRAFORM_AWS_PLUGIN_VERSION} aws/${TERRAFORM_AWS_PLUGIN_VERSION}/linux_amd64 && \
    mkdir -p archive/${TERRAFORM_ARCHIVE_PLUGIN_VERSION} archive/${TERRAFORM_ARCHIVE_PLUGIN_VERSION}/linux_amd64 && \
    cd ~/.terraform.d/plugins/registry.terraform.io/ && \
    mkdir -p mongodb mongodb/mongodbatlas mongodb/mongodbatlas/${TERRAFORM_MONGODBATLAS_PLUGIN_VERSION} mongodb/mongodbatlas/${TERRAFORM_MONGODBATLAS_PLUGIN_VERSION}/linux_amd64

# terraform
ENV TERRAFORM_VERSION 1.0.5
ENV TERRAFORM_CHECKSUM 7ce24478859ab7ca0ba4d8c9c12bb345f52e8efdc42fa3ef9dd30033dbf4b561
RUN curl -fsSL https://releases.hashicorp.com/terraform/${TERRAFORM_VERSION}/terraform_${TERRAFORM_VERSION}_linux_amd64.zip -o terraform.zip  && \
    echo "${TERRAFORM_CHECKSUM} terraform.zip" | sha256sum -c - && \
    unzip terraform.zip -d /usr/local/bin && chmod +x /usr/local/bin/terraform ; rm terraform.zip

# terraform mongodbatlas provider plugin (for mongodbatlas)
ENV TERRAFORM_MONGODBATLAS_PLUGIN_CHECKSUM 249959a4ce72f4ed94613e8baff00c4f571088511a7b908ab49ae42e5af5ead0
RUN curl -fsSL https://releases.hashicorp.com/terraform-provider-mongodbatlas/${TERRAFORM_MONGODBATLAS_PLUGIN_VERSION}/terraform-provider-mongodbatlas_${TERRAFORM_MONGODBATLAS_PLUGIN_VERSION}_linux_amd64.zip -o terraform_mongodbatlas_plugin.zip  && \
    echo "${TERRAFORM_MONGODBATLAS_PLUGIN_CHECKSUM} terraform_mongodbatlas_plugin.zip" | sha256sum -c - && \
    unzip terraform_mongodbatlas_plugin.zip -d ~/.terraform.d/plugins/registry.terraform.io/mongodb/mongodbatlas/${TERRAFORM_MONGODBATLAS_PLUGIN_VERSION}/linux_amd64/ ; rm terraform_mongodbatlas_plugin.zip

# terraform aws provider plugin
ENV TERRAFORM_AWS_PLUGIN_CHECKSUM f1f1a0bebd03e479b072dde9831f573070f6deada2972f5b966dd4858b99a8c2
RUN curl -fsSL https://releases.hashicorp.com/terraform-provider-aws/${TERRAFORM_AWS_PLUGIN_VERSION}/terraform-provider-aws_${TERRAFORM_AWS_PLUGIN_VERSION}_linux_amd64.zip -o terraform_aws_plugin.zip  && \
    echo "${TERRAFORM_AWS_PLUGIN_CHECKSUM} terraform_aws_plugin.zip" | sha256sum -c - && \
    unzip terraform_aws_plugin.zip -d ~/.terraform.d/plugins/registry.terraform.io/hashicorp/aws/${TERRAFORM_AWS_PLUGIN_VERSION}/linux_amd64/  ; rm terraform_aws_plugin.zip

# terraform terraform provider archive (for lambda)
ENV TERRAFORM_ARCHIVE_PLUGIN_CHECKSUM e63f12ea938520b3f83634fc29da28d92eed5cfbc5cc8ca08281a6a9c36cca65
RUN curl -fsSL https://releases.hashicorp.com/terraform-provider-archive/${TERRAFORM_ARCHIVE_PLUGIN_VERSION}/terraform-provider-archive_${TERRAFORM_ARCHIVE_PLUGIN_VERSION}_linux_amd64.zip -o terraform_terraform_plugin.zip  && \
    echo "${TERRAFORM_ARCHIVE_PLUGIN_CHECKSUM} terraform_terraform_plugin.zip" | sha256sum -c - && \
    unzip terraform_terraform_plugin.zip -d ~/.terraform.d/plugins/registry.terraform.io/hashicorp/archive/${TERRAFORM_ARCHIVE_PLUGIN_VERSION}/linux_amd64/ ; rm terraform_terraform_plugin.zip
  
# aws cliv2
RUN curl -sSL https://awscli.amazonaws.com/awscli-exe-linux-x86_64.zip -o awscliv2.zip && \
    unzip awscliv2.zip && \
    aws/install -i /usr/local/aws -b /usr/local/bin && \
    rm awscliv2.zip && rm -rf ./aws


COPY umask.sh /
RUN chmod +x /umask.sh


ENTRYPOINT [ "/umask.sh" ]