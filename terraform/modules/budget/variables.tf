variable "limit_amount" {
  description = "The amount of cost or usage being measured for a budget."
}

variable "appname" {
  description = "appname without app suffix e.g. pdf, dms, inbound."
}

variable "sns_topic" {
  description = "SNS topic arn to notifiy"
}
