# cf. https://www.terraform.io/docs/providers/aws/r/budgets_budget.html
resource "aws_budgets_budget" "budget" {
  name              = "budget-monthly-${var.appname}"
  budget_type       = "COST"
  limit_amount      = var.limit_amount
  limit_unit        = "USD"
  time_period_end   = "2087-06-15_00:00"
  time_period_start = "2020-06-01_00:00"
  time_unit         = "MONTHLY"

  notification {
    comparison_operator       = "GREATER_THAN"
    threshold                 = 100
    threshold_type            = "PERCENTAGE"
    notification_type         = "FORECASTED"
    subscriber_sns_topic_arns = [var.sns_topic]
  }

  notification {
    comparison_operator       = "GREATER_THAN"
    threshold                 = 100
    threshold_type            = "PERCENTAGE"
    notification_type         = "ACTUAL"
    subscriber_sns_topic_arns = [var.sns_topic]
  }
}
