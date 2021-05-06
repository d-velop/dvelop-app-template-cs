variable "api_names" {
  type        = list(string)
  description = "names of the api gateway apis"
}

variable "sns_topic" {
  description = "SNS topic arn to notifiy"
}