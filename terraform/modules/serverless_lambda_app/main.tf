#cf. https://www.terraform.io/docs/providers/aws/r/s3_bucket.html
resource "aws_s3_bucket" "assets" {
  bucket = var.assets_bucket_name
  region = var.aws_region

  # required if webfonts are delivered cf. https://docs.aws.amazon.com/AmazonS3/latest/dev/cors.html and https://zinoui.com/blog/cross-domain-fonts
  cors_rule {
    allowed_methods = ["GET"]
    allowed_origins = ["*"]
  }

  policy = <<POLICY
{
  "Version":"2012-10-17",
  "Statement":[{
      "Sid":"PublicReadGetObject",
      "Effect":"Allow",
      "Principal": "*",
      "Action":["s3:GetObject"],
      "Resource":["arn:aws:s3:::${var.assets_bucket_name}/*"]
    }
  ]
}
POLICY


  tags = {
    Created_By = "Terraform - do not modify in AWS Management Console"
  }
}

# cf. https://www.terraform.io/docs/providers/aws/r/lambda_function.html
resource "aws_lambda_function" "service" {
  filename         = var.lambda_file
  source_code_hash = var.source_code_hash
  function_name    = "${var.appname}service"
  role             = aws_iam_role.lambda_service_role.arn
  handler          = var.lambda_handler
  runtime          = var.lambda_runtime
  memory_size      = var.lambda_memory_size
  publish          = true
  timeout          = 11

  # Environment vars are specific to each version of the lambda function and can't be changed after deployment.
  # So each change of the vars requires a redeployment of the lambda function to have an effect.
  # cf. https://docs.aws.amazon.com/lambda/latest/dg/env_variables.html
  environment {
    variables = var.lambda_environment_vars
  }

  # cf. https://docs.aws.amazon.com/lambda/latest/dg/enabling-x-ray.html
  tracing_config {
    mode = "Active"
  }

  tags = {
    Name       = "${var.appname}service-lambda-fn"
    Created_By = "Terraform - do not modify in AWS Management Console"
  }
}

# cf. https://www.terraform.io/docs/providers/aws/r/lambda_function.html#cloudwatch-logging-and-permissions
resource "aws_cloudwatch_log_group" "lambda" {
  name              = "/aws/lambda/${aws_lambda_function.service.function_name}"
  retention_in_days = 14
  kms_key_id        = var.kms_key_id
}

# https://www.terraform.io/docs/providers/aws/r/iam_role.html
resource "aws_iam_role" "lambda_service_role" {
  name = "${var.appname}service-lambda-role"
  path = "/${var.appname}service/"

  # which entity can assume this role
  assume_role_policy = data.aws_iam_policy_document.lambda_service_role.json
}

# cf. siehe https://www.terraform.io/docs/providers/aws/d/iam_policy_document.html
data "aws_iam_policy_document" "lambda_service_role" {
  statement {
    effect = "Allow"

    principals {
      type        = "Service"
      identifiers = ["lambda.amazonaws.com"]
    }

    actions = ["sts:AssumeRole"]
  }
}

# https://www.terraform.io/docs/providers/aws/r/iam_role_policy_attachment.html
resource "aws_iam_role_policy_attachment" "lambda_service" {
  count      = length(var.lambda_policy_attachements)
  role       = aws_iam_role.lambda_service_role.name
  policy_arn = element(var.lambda_policy_attachements, count.index)
}

# can't access aws_api_gateway_rest_api.service.endpoint_configuration.types for output
# so local var is used
locals {
  endpoint_configuration_types = ["REGIONAL"]
}

# cf. https://www.terraform.io/docs/providers/aws/r/api_gateway_rest_api.html
resource "aws_api_gateway_rest_api" "service" {
  name        = "${var.appname}service"
  description = "API for ${var.appname}service"

  # Use regional endpoints because all requests to this api originate from a centrally managed reverse proxy
  # in the same region (eu-central-1) and regional endpoints have lower latency
  # if the requests originate from a service in the same region.
  # If the situation changes a dedicated cloudfront distribution can be created and assigned to the regional endpoint.
  # cf. https://docs.aws.amazon.com/apigateway/latest/developerguide/create-regional-api.html
  endpoint_configuration {
    types = local.endpoint_configuration_types
  }
}

# cf. https://www.terraform.io/docs/providers/aws/r/api_gateway_resource.html
resource "aws_api_gateway_resource" "proxyresource" {
  rest_api_id = aws_api_gateway_rest_api.service.id
  parent_id   = aws_api_gateway_rest_api.service.root_resource_id
  path_part   = "{proxy+}"
}

# cf. https://www.terraform.io/docs/providers/aws/r/api_gateway_method.html
resource "aws_api_gateway_method" "proxyresource_method" {
  rest_api_id   = aws_api_gateway_rest_api.service.id
  resource_id   = aws_api_gateway_resource.proxyresource.id
  http_method   = "ANY"
  authorization = "NONE"
}

# cf. https://www.terraform.io/docs/providers/aws/r/api_gateway_integration.html
resource "aws_api_gateway_integration" "proxyresource_method_integration" {
  rest_api_id             = aws_api_gateway_rest_api.service.id
  resource_id             = aws_api_gateway_resource.proxyresource.id
  http_method             = aws_api_gateway_method.proxyresource_method.http_method
  integration_http_method = "POST"
  type                    = "AWS_PROXY"
  uri                     = "arn:aws:apigateway:${var.aws_region}:lambda:path/2015-03-31/functions/${aws_lambda_function.service.arn}:$${stageVariables.lambda_alias_name}/invocations"
}

# cf. https://www.terraform.io/docs/providers/aws/r/lambda_permission.html
resource "aws_lambda_permission" "allow_apigateway_for_lambda" {
  statement_id  = "AllowAPIGatewayToExecuteLambdaFunction"
  action        = "lambda:InvokeFunction"
  function_name = aws_lambda_function.service.function_name
  principal     = "apigateway.amazonaws.com"
  source_arn    = "${aws_api_gateway_rest_api.service.execution_arn}/*/*/*"
}

# ${replace(aws_route53_zone.hosted_zone.name, "/[.]$/", "")}
# cf. https://www.terraform.io/docs/providers/aws/r/lambda_alias.html
resource "aws_lambda_alias" "alias" {
  count = length(var.stages)
  name = replace(
    element(keys(var.stages), count.index),
    "/^[a-zA-Z0-9]*_/",
    "",
  )
  description = "lambda function version to use for ${replace(
    element(keys(var.stages), count.index),
    "/^[a-zA-Z0-9]*_/",
    "",
  )}"
  function_name    = aws_lambda_function.service.arn
  function_version = element(values(var.stages), count.index) == "NewVersion" ? aws_lambda_function.service.version : element(values(var.stages), count.index)
}

# cf. https://www.terraform.io/docs/providers/aws/r/api_gateway_deployment.html
resource "aws_api_gateway_deployment" "deployment" {
  count = length(aws_lambda_alias.alias.*.name)

  depends_on = [aws_api_gateway_integration.proxyresource_method_integration]

  rest_api_id = aws_api_gateway_rest_api.service.id
  stage_name  = element(aws_lambda_alias.alias.*.name, count.index)

  # cf. http://docs.aws.amazon.com/apigateway/latest/developerguide/stage-variables.html
  variables = {
    "lambda_alias_name" = element(aws_lambda_alias.alias.*.name, count.index)
  }
}

# cf. https://www.terraform.io/docs/providers/aws/r/lambda_permission.html
resource "aws_lambda_permission" "allow_apigateway_for_alias" {
  count         = length(aws_lambda_alias.alias.*.name)
  statement_id  = "AllowAPIGatewayToExecute${element(aws_lambda_alias.alias.*.name, count.index)}LambdaFunction"
  action        = "lambda:InvokeFunction"
  function_name = aws_lambda_function.service.function_name
  qualifier     = element(aws_lambda_alias.alias.*.name, count.index)
  principal     = "apigateway.amazonaws.com"
  source_arn    = "${aws_api_gateway_rest_api.service.execution_arn}/*/*/*"
}

