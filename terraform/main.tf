locals {
  assets_bucket_name = "${var.system_prefix}${var.appname}-assets"

  lambda_file      = "../dist/lambda.zip"
  source_code_hash = filebase64sha256(local.lambda_file)

  # Unfortunately there is a bug in terraform which leads to the destruction of existing resources if
  # the element order of lists and maps changes cf. https://github.com/hashicorp/terraform/issues/16210
  # Elements of maps are ordered alphabetically. So each element has a prefix to preserve the order.
  # This prefix is not part of the stage name and will be removed internally.
  # If you add stages use keys which result in the stage appended at the end of the list!
  # If you are uncertain use terraform plan to check the changes terraform would make.
  stages = {
    "a_prod" = var.tag_prod == "1" ? "NewVersion" : data.terraform_remote_state.app.outputs.prod_service_lambda_version
    "b_dev"  = "$LATEST"
  }

  // to avoid unnecessary lambda function deployments the build version env var is only changed if the lambda function code has been changed
  build_version = local.source_code_hash != data.terraform_remote_state.app.outputs.source_code_hash ? var.build_version : data.terraform_remote_state.app.outputs.build_version
}

module "serverless_lambda_app" {
  source             = "./modules/serverless_lambda_app"
  stages             = local.stages
  appname            = var.appname
  lambda_file        = local.lambda_file
  source_code_hash   = local.source_code_hash
  lambda_memory_size = "512"

  # If you change your output file names or namespaces, you will have to edit the next line
  # <binary>::<name-space>.<class>::<function>
  lambda_handler     = "EntryPoint::Dvelop.Lambda.EntryPoint.LambdaEntryPoint::FunctionHandlerAsync"
  lambda_runtime     = "dotnet6"
  assets_bucket_name = local.assets_bucket_name

  # Which rights should the lambda function have.
  # Terraform user must have appropriate rights to attach these policies!
  lambda_policy_attachements = [
    "arn:aws:iam::aws:policy/service-role/AWSLambdaBasicExecutionRole",
    "arn:aws:iam::aws:policy/AWSXrayWriteOnlyAccess",
  ]

  lambda_environment_vars = {
    SIGNATURE_SECRET = var.signature_secret
    BUILD_VERSION    = local.build_version
    # change following entries if asset_cdn is enabled
    #ASSET_BASE_PATH  = "https://${module.asset_cdn.dns_name}/${var.asset_hash}"
    ASSET_BASE_PATH = "https://${local.assets_bucket_name}.s3-eu-central-1.amazonaws.com/${var.asset_hash}"
  }

  aws_region = var.aws_region
  kms_key_id = module.encryption.encryption_key_id
}

# If you use cloudfront (a CDN) to deliver your assets, you should remember to remove  's3-eu-central-1.amazonaws.com/' from this output. 
# Otherwise the deployment will show a different configuration than used.
output "asset_base_path" {
  value = "https://${local.assets_bucket_name}.amazonaws.com/${var.asset_hash}"
}

# Uncomment if you want to use cloudfront (a CDN) to deliver your assets OR custom domain names for your API endpoints.
# IMPORTANT:
# - Both, the cloudfront distribution and a custom domain name für API endpoints require a DNS hosted zone.
#   So this resources must be uncommented if you want to use either of them.
# cf. https://www.terraform.io/docs/providers/aws/d/route53_zone.html
/*
resource "aws_route53_zone" "hosted_zone" {
  name = "${var.system_prefix}${var.appname}${var.domainsuffix}"
}
output "nameserver" {
  value = aws_route53_zone.hosted_zone.name_servers
}
*/

# Uncomment if you want to use cloudfront (a CDN) to deliver your assets.
# IMPORTANT:
# - This module requires a working dns resolution for your hosted zone because
#   the module creates certificates which ar validated via DNS
# - The module might fail because it will take some time (up to or more than 30 min)
#   for a certificate to be validated by AWS. If this is the case just invoke terraform a second time.
/*
module "asset_cdn" {
  source                = "./modules/cloudfront_distribution"
  hosted_zone_id        = aws_route53_zone.hosted_zone.id
  custom_subdomain_name = "assets"
  origin_domain_name    = module.serverless_lambda_app.assets_bucket_domain_name
}
*/

# Uncomment if you want to use custom domain names for your API endpoints.
# cf. https://docs.aws.amazon.com/apigateway/latest/developerguide/how-to-custom-domains.html
# IMPORTANT:
# - This module requires a working dns resolution for your hosted zone because
#   the module creates certificates which ar validated via DNS
# - The module might fail because it will take some time (up to or more than 30 min)
#   for a certificate to be validated by AWS. If this is the case just invoke terraform a second time.
/*
module "api_custom_domains" {
  source                                                = "./modules/api_custom_domain"
  hosted_zone_id                                        = aws_route53_zone.hosted_zone.id
  aws_api_gateway_rest_api_id                           = module.serverless_lambda_app.aws_api_gateway_rest_api_id
  aws_api_gateway_rest_api_endpoint_configuration_types = module.serverless_lambda_app.aws_api_gateway_rest_api_endpoint_configuration_types
  stages                                                = module.serverless_lambda_app.stages
}
*/

# If you want to be notified in case of budget overrun or Cloudwatch alarms, you need to subscribe to the SNS topic manually via the AWS Console
# cf. https://www.terraform.io/docs/providers/aws/r/sns_topic.html
resource "aws_sns_topic" "monitoring_topic" {
  name         = "${var.appname}-Monitoring"
  display_name = "Monitoring for budget and cloudwatch alarms"
}

module "budget" {
  source       = "./modules/budget"
  limit_amount = var.budget_amount
  appname      = var.appname
  sns_topic    = aws_sns_topic.monitoring_topic.arn
}

module "monitoring" {
  source    = "./modules/monitoring"
  sns_topic = aws_sns_topic.monitoring_topic.arn

  lambda_function_names = [
    module.serverless_lambda_app.function_name
  ]

  api_names = [
    module.serverless_lambda_app.function_name
  ]
}

# cf. https://www.terraform.io/docs/providers/aws/d/caller_identity.html
data "aws_caller_identity" "current" {}

module "encryption" {
  source         = "./modules/encryption"
  appname        = var.appname
  aws_account_id = data.aws_caller_identity.current.account_id
}
