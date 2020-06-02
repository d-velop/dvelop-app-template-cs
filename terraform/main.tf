locals {
  assets_bucket_name = "assets.${var.appname}${var.domainsuffix}"

  lambda_file      = "../dist/lambda.zip"
  source_code_hash = "${base64sha256(file("${local.lambda_file}"))}"

  # Unfortunately there is a bug in terraform which leads to the destruction of existing resources if
  # the element order of lists and maps changes cf. https://github.com/hashicorp/terraform/issues/16210
  # Elements of maps are ordered alphabetically. So each element has a prefix to preserve the order.
  # This prefix is not part of the stage name and will be removed internally.
  # If you add stages use keys which result in the stage appended at the end of the list!
  # If you are uncertain use terraform plan to check the changes terraform would make.
  stages = {
    "a_prod" = "$LATEST"
    "b_dev"  = "$LATEST"
  }

  // to avoid unnecessary lambda function deployments the build version env var is only changed if the lambda function code has been changed
  build_version = "${local.source_code_hash != data.terraform_remote_state.app.source_code_hash ? var.build_version : data.terraform_remote_state.app.build_version}"
}

module "serverless_lambda_app" {
  source             = "modules/serverless_lambda_app"
  stages             = "${local.stages}"
  appname            = "${var.appname}"
  lambda_file        = "${local.lambda_file}"
  source_code_hash   = "${local.source_code_hash}"
  lambda_memory_size = "512"
  
  # If you change your output file names or namespaces, you will have to edit the next line
  # <binary>::<name-space>.<class>::<function>
  lambda_handler      = "EntryPoint::Dvelop.Lambda.EntryPoint.LambdaEntryPoint::FunctionHandlerAsync"
  lambda_runtime      = "dotnetcore3.1"
  assets_bucket_name = "${local.assets_bucket_name}"

  # Which rights should the lambda function have.
  # Terraform user must have appropriate rights to attach these policies!
  lambda_policy_attachements = [
    "arn:aws:iam::aws:policy/service-role/AWSLambdaBasicExecutionRole",
    "arn:aws:iam::aws:policy/AWSXrayWriteOnlyAccess",
  ]

  lambda_environment_vars = {
    SIGNATURE_SECRET = "${var.signature_secret}"
    BUILD_VERSION    = "${local.build_version}"

    # change to ASSET_BASE_PATH  = "https://${module.asset_cdn.dns_name}/${var.asset_hash}" if asset_cdn is enabled
    ASSET_BASE_PATH = "https://s3-eu-central-1.amazonaws.com/${local.assets_bucket_name}/${var.asset_hash}"

    # This should be removed in production
    ASPNETCORE_ENVIRONMENT = "Development"
  }

  aws_region = "${var.aws_region}"
}

# If you use cloudfront (a CDN) to deliver your assets, you should remember to remove  's3-eu-central-1.amazonaws.com/' from this output. 
# Otherwise the deployment will show a different configuration than used.
output "asset_base_path" {
  value = "https://s3-eu-central-1.amazonaws.com/${local.assets_bucket_name}/${var.asset_hash}"
}



# Uncomment if you want to use cloudfront (a CDN) to deliver your assets OR custom domain names for your API endpoints.
# IMPORTANT:
# - Both, the cloudfront distribution and a custom domain name für API endpoints require a DNS hosted zone.
#   So this resources must be uncommented if you want to use either of them.
# cf. https://www.terraform.io/docs/providers/aws/d/route53_zone.html
/*
resource "aws_route53_zone" "hosted_zone" {
  name = "${var.appname}${var.domainsuffix}"
}
output "nameserver" {
  value = "${aws_route53_zone.hosted_zone.name_servers}"
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
  source                = "modules/cloudfront_distribution"
  hosted_zone_id        = "${aws_route53_zone.hosted_zone.id}"
  custom_subdomain_name = "assets"
  origin_domain_name    = "${module.serverless_lambda_app.assets_bucket_domain_name}"
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
  source                                                = "modules/api_custom_domain"
  hosted_zone_id                                        = "${aws_route53_zone.hosted_zone.id}"
  aws_api_gateway_rest_api_id                           = "${module.serverless_lambda_app.aws_api_gateway_rest_api_id}"
  aws_api_gateway_rest_api_endpoint_configuration_types = "${module.serverless_lambda_app.aws_api_gateway_rest_api_endpoint_configuration_types}"
  stages                                                = "${module.serverless_lambda_app.stages}"
}
*/
