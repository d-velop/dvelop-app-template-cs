module "lambda_monitoring_app" {
  source                = "./lambda_monitoring"
  lambda_function_names = var.lambda_function_names
  sns_topic             = var.sns_topic
}

module "apigateway_monitoring_app" {
  source    = "./apigateway_monitoring"
  api_names = var.api_names
  sns_topic = var.sns_topic
}
