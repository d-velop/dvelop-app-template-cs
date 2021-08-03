provider "aws" {
  version = "~> 3.52.0"
  region  = var.aws_region
}

provider "mongodbatlas" {
  # Configuration options
}

provider "archive" {
  # Configuration options
}

provider "template" {
  # Configuration options
}