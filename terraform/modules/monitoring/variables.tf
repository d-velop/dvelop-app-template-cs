variable "lambda_function_names" {
  type        = list(string)
  description = "names of the lambda functions"
}

variable "api_names" {
  type        = list(string)
  description = "names of the api gateway apis"
}

variable "sns_topic" {
  description = "SNS topic arn to notifiy"
}
