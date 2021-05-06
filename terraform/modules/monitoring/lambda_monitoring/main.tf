# cf. https://www.terraform.io/docs/providers/aws/r/cloudwatch_log_metric_filter.html
resource "aws_cloudwatch_log_metric_filter" "error_log_metric_filter" {
  count          = length(var.lambda_function_names)
  name           = "${element(var.lambda_function_names, count.index)} error messages"
  pattern        = "?ERROR ?\"[Error]\" ?EXCEPTION ?Exception ?exception ?FATAL ?\"[Fatal]\" ?fatal"
  log_group_name = "/aws/lambda/${element(var.lambda_function_names, count.index)}"

  metric_transformation {
    name          = "${element(var.lambda_function_names, count.index)}_log_errors"
    namespace     = "LogMetrics"
    value         = "1"
    default_value = "0"
  }
}

# cf. https://www.terraform.io/docs/providers/aws/r/cloudwatch_metric_alarm.html
resource "aws_cloudwatch_metric_alarm" "error_log_alarm" {
  count               = length(var.lambda_function_names)
  alarm_name          = "Lambda ${element(var.lambda_function_names, count.index)} error messages in log"
  alarm_description   = "Lambda ${element(var.lambda_function_names, count.index)} has to many error log messages"
  comparison_operator = "GreaterThanThreshold"
  evaluation_periods  = "1"
  metric_name         = "${element(var.lambda_function_names, count.index)}_log_errors"
  namespace           = "LogMetrics"
  period              = "60"
  statistic           = "Sum"
  threshold           = "0"
  datapoints_to_alarm = "1"
  treat_missing_data  = "notBreaching"
  alarm_actions       = [var.sns_topic]
}

# cf. https://www.terraform.io/docs/providers/aws/r/cloudwatch_log_metric_filter.html
resource "aws_cloudwatch_log_metric_filter" "timeout_log_metric_filter" {
  count          = length(var.lambda_function_names)
  name           = "${element(var.lambda_function_names, count.index)} timeout messages"
  pattern        = "?Task timed out after"
  log_group_name = "/aws/lambda/${element(var.lambda_function_names, count.index)}"

  metric_transformation {
    name          = "${element(var.lambda_function_names, count.index)}_log_timeouts"
    namespace     = "LogMetrics"
    value         = "1"
    default_value = "0"
  }
}

# cf. https://www.terraform.io/docs/providers/aws/r/cloudwatch_metric_alarm.html
resource "aws_cloudwatch_metric_alarm" "timeout_log_alarm" {
  count               = length(var.lambda_function_names)
  alarm_name          = "Lambda ${element(var.lambda_function_names, count.index)} timeout messages in log"
  alarm_description   = "Lambda ${element(var.lambda_function_names, count.index)} has to many timeout log messages"
  comparison_operator = "GreaterThanThreshold"
  evaluation_periods  = "1"
  metric_name         = "${element(var.lambda_function_names, count.index)}_log_timeouts"
  namespace           = "LogMetrics"
  period              = "60"
  statistic           = "Sum"
  threshold           = "0"
  datapoints_to_alarm = "1"
  treat_missing_data  = "notBreaching"
  alarm_actions       = [var.sns_topic]
}

# cf. https://www.terraform.io/docs/providers/aws/r/cloudwatch_metric_alarm.html
resource "aws_cloudwatch_metric_alarm" "lambda_errors_alarm" {
  count               = length(var.lambda_function_names)
  alarm_name          = "Lambda ${element(var.lambda_function_names, count.index)} function"
  alarm_description   = "Lambda function ${element(var.lambda_function_names, count.index)} has failed."
  comparison_operator = "GreaterThanThreshold"
  evaluation_periods  = "1"
  metric_name         = "Errors"
  namespace           = "AWS/Lambda"
  period              = "60"
  statistic           = "Sum"
  threshold           = "0"

  dimensions = {
    FunctionName = element(var.lambda_function_names, count.index)
  }

  treat_missing_data = "notBreaching"
  alarm_actions      = [var.sns_topic]
}

# cf. https://www.terraform.io/docs/providers/aws/r/cloudwatch_metric_alarm.html
resource "aws_cloudwatch_metric_alarm" "lambda_throttles_alarm" {
  count               = length(var.lambda_function_names)
  alarm_name          = "${element(var.lambda_function_names, count.index)} function throttles"
  alarm_description   = "Lambda function ${element(var.lambda_function_names, count.index)} has throttled."
  comparison_operator = "GreaterThanThreshold"
  evaluation_periods  = "1"
  metric_name         = "Throttles"
  namespace           = "AWS/Lambda"
  period              = "60"
  statistic           = "Sum"
  threshold           = "0"

  dimensions = {
    FunctionName = element(var.lambda_function_names, count.index)
  }

  treat_missing_data = "notBreaching"
  alarm_actions      = [var.sns_topic]
}