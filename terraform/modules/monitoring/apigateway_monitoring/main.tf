# cf. https://www.terraform.io/docs/providers/aws/r/cloudwatch_metric_alarm.html
resource "aws_cloudwatch_metric_alarm" "APIGateway_5xxError" {
  count               = length(var.api_names)
  alarm_name          = "APIGateway 5xxError ${element(var.api_names, count.index)}"
  comparison_operator = "GreaterThanThreshold"
  evaluation_periods  = "1"
  metric_name         = "5XXError"
  namespace           = "AWS/ApiGateway"
  period              = "3600"
  statistic           = "Sum"
  threshold           = "0"
  alarm_description   = "5XX errors in apigateway ${element(var.api_names, count.index)}"
  treat_missing_data  = "notBreaching"

  alarm_actions = [var.sns_topic]

  dimensions = {
    ApiName = var.api_names[count.index]
  }
}

# cf. https://www.terraform.io/docs/providers/aws/r/cloudwatch_metric_alarm.html
resource "aws_cloudwatch_metric_alarm" "APIGateway_Latency" {
  count               = length(var.api_names)
  alarm_name          = "APIGateway Latency ${element(var.api_names, count.index)}"
  comparison_operator = "GreaterThanThreshold"
  evaluation_periods  = "2"
  metric_name         = "Latency"
  namespace           = "AWS/ApiGateway"
  period              = "900"
  statistic           = "Maximum"
  threshold           = "25000"
  alarm_description   = "Timeout in api gateway ${element(var.api_names, count.index)}"
  treat_missing_data  = "notBreaching"
  datapoints_to_alarm = "2"

  alarm_actions = [var.sns_topic]

  dimensions = {
    ApiName = var.api_names[count.index]
  }
}
