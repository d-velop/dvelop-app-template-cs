terraform {
  required_version = ">= 1.0.5"
  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "4.8.0"
    }
    archive = {
      source  = "hashicorp/archive"
      version = "2.2.0"
    }
    template = {
      source  = "hashicorp/template"
      version = "2.2.0"
    }
  }
}
