# Terraform backend configuration, that is the place where terraform stores the system state.
# cf. https://www.terraform.io/docs/backends/config.html
# cf. https://www.terraform.io/docs/backends/types/s3.html für eine Beschreibung des s3 backends
terraform {
  backend "s3" {
    key    = "state"

    # variables can't be used
    region = "eu-central-1"
  }
}

data "terraform_remote_state" "app" {
  backend = "s3"

  config = {
    # bucket names must be globally unique across all AWS customers
    # so we choose a combination of company prefix ('acme')
    # and purpose (terraform) and appname (apptemplatego)
    bucket = "${var.system_prefix}acme-${var.appname}-terraform"
    key    = "state"
    region = var.aws_region
  }

  defaults = {
    source_code_hash = "0"
    build_version    = "0"
  }
}

