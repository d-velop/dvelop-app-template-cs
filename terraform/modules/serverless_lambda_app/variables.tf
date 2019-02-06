variable "stages" {
  type = "map"

  # Unfortunately there is a bug in terraform which leads to the destruction of existing resources if
  # the element order of lists and maps changes cf. https://github.com/hashicorp/terraform/issues/16210
  # Elements of maps are ordered alphabetically. So each element has a prefix to preserve the order.
  # This prefix is not part of the stage name and will be removed internally.
  # If you add stages use keys which result in the stage appended at the end of the list!
  # If you are uncertain use terraform plan to check the changes terraform would make.
  description = "Which stages should be created. Each stage must have a prefixe to preserve the order!"

  default = {
    "a_prod" = "$LATEST"
    "b_dev"  = "$LATEST"
  }
}

variable "assets_bucket_name" {
  description = "bucket name for static assets like css and js files/bundles"
}

variable "appname" {
  description = "appname without app suffix e.g. pdf, dms, inbound."
}

variable "aws_region" {}

variable "lambda_file" {
  description = "Path to ZIP file with lambda function"
}

variable "lambda_handler" {
  description = "name of the lambda handler"
}

variable "lambda_runtime" {
  description = "lambda runtime e.g. go1.x"
}

variable "lambda_memory_size" {
  description = "Amount of memory in MB your Lambda Function can use at runtime"
  default     = "128"
}

variable "lambda_environment_vars" {
  type        = "map"
  description = "map that defines environment variables for the lambda function"
}

variable "lambda_policy_attachements" {
  type        = "list"
  description = "list of policies to attach to the lambda function"
}

variable "source_code_hash" {
  description = "Source code hash of the lambda function. New version will only be deployed if the hash changes between 2 deployments."
}
