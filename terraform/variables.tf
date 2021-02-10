variable "aws_region" {
  default = "eu-central-1"
}

variable "signature_secret" {
  description = "base64 encoded secret key for validation of tenant headers"
}

variable "build_version" {
  description = "human readable build metadata like git commit hash or timestamp to identify this build of the lambda function"
  default     = "unknown"
}

variable "asset_hash" {
  description = "hash over all assets"
  default     = "unknown"
}

variable "appname" {
  description = "appname without app suffix e.g. pdf, dms, inbound."
}

variable "domainsuffix" {
  description = "dns suffix for the service endpoint"
}

variable "budget_amount" {
  description = "The amount of cost or usage being measured for a budget (in USD)."
  default     = "20.0"
}

variable "system_prefix" {
  description = "Prefix for urls and buckets to identify dev-account (e.g. 'dev-')"
  default     = ""
}

variable "tag_prod" {
  description = "Set this variable to 1 to set the prod alias to the latest lambda function version"
  default     = "0"
}
