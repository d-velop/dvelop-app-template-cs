output "endpoints" {
  value = formatlist(
    "%s/%s/",
    aws_api_gateway_deployment.deployment.*.invoke_url,
    var.appname,
  )
}

output "assets_bucket_domain_name" {
  value = aws_s3_bucket.assets.bucket_domain_name
}

output "aws_api_gateway_rest_api_id" {
  value = aws_api_gateway_rest_api.service.id
}

output "aws_api_gateway_rest_api_endpoint_configuration_types" {
  value = local.endpoint_configuration_types
}

output "stages" {
  value = aws_api_gateway_deployment.deployment.*.stage_name
}

output "function_name" {
  value = aws_lambda_function.service.function_name
}

output "function_version" {
  value = element(aws_lambda_alias.alias.*.function_version, 0)
}
