variable "lambda_function_names" {
  type        = list(string)
  description = "names of the lambda functions"
}

variable "sns_topic" {
  description = "SNS topic arn to notifiy"
}
