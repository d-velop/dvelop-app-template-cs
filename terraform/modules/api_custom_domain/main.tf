# cf. https://www.terraform.io/docs/providers/aws/d/route53_zone.html
data "aws_route53_zone" "hosted_zone" {
  zone_id = var.hosted_zone_id
}

locals {
  // cf. https://github.com/terraform-providers/terraform-provider-aws/issues/241#issuecomment-438744460
  hosted_zone_name = replace(data.aws_route53_zone.hosted_zone.name, "/[.]$/", "")
}

# cf. https://www.terraform.io/docs/providers/aws/r/acm_certificate.html
resource "aws_acm_certificate" "cert" {
  domain_name               = local.hosted_zone_name
  subject_alternative_names = ["*.${local.hosted_zone_name}"]
  validation_method         = "DNS"

  lifecycle {
    create_before_destroy = true
  }
}

# cf. https://www.terraform.io/docs/providers/aws/r/acm_certificate_validation.html
resource "aws_acm_certificate_validation" "cert" {
  certificate_arn = aws_acm_certificate.cert.arn

  validation_record_fqdns = [
    aws_route53_record.cert_name_validation.fqdn,
    aws_route53_record.cert_alt_name_validation.fqdn,
  ]
}

# cf. https://www.terraform.io/docs/providers/aws/r/acm_certificate_validation.html
resource "aws_route53_record" "cert_name_validation" {
  allow_overwrite = true
  name            = aws_acm_certificate.cert.domain_validation_options[0].resource_record_name
  type            = aws_acm_certificate.cert.domain_validation_options[0].resource_record_type
  zone_id         = var.hosted_zone_id
  records         = [aws_acm_certificate.cert.domain_validation_options[0].resource_record_value]
  ttl             = 60
}

# cf. https://www.terraform.io/docs/providers/aws/r/acm_certificate_validation.html
resource "aws_route53_record" "cert_alt_name_validation" {
  allow_overwrite = true
  name            = aws_acm_certificate.cert.domain_validation_options[1].resource_record_name
  type            = aws_acm_certificate.cert.domain_validation_options[1].resource_record_type
  zone_id         = var.hosted_zone_id
  records         = [aws_acm_certificate.cert.domain_validation_options[1].resource_record_value]
  ttl             = 60
}

# cf. https://www.terraform.io/docs/providers/aws/r/api_gateway_domain_name.html
resource "aws_api_gateway_domain_name" "stage" {
  count = length(var.stages)

  # By convention the 'prod' stage is mapped to the name of the provided hosted zone without the prefix 'prod'
  domain_name              = "${var.stages[count.index] != "prod" ? format("%s.", var.stages[count.index]) : ""}${local.hosted_zone_name}"
  regional_certificate_arn = aws_acm_certificate_validation.cert.certificate_arn

  endpoint_configuration {
    types = var.aws_api_gateway_rest_api_endpoint_configuration_types
  }
}

# cf. https://www.terraform.io/docs/providers/aws/r/api_gateway_base_path_mapping.html
resource "aws_api_gateway_base_path_mapping" "stage" {
  count       = length(var.stages)
  api_id      = var.aws_api_gateway_rest_api_id
  stage_name  = var.stages[count.index]
  domain_name = element(aws_api_gateway_domain_name.stage.*.domain_name, count.index)
}

# cf. https://www.terraform.io/docs/providers/aws/r/api_gateway_domain_name.html
resource "aws_route53_record" "stage" {
  count   = length(var.stages)
  zone_id = var.hosted_zone_id
  name    = element(aws_api_gateway_domain_name.stage.*.domain_name, count.index)
  type    = "A"

  alias {
    name = element(
      aws_api_gateway_domain_name.stage.*.regional_domain_name,
      count.index,
    )
    zone_id = element(
      aws_api_gateway_domain_name.stage.*.regional_zone_id,
      count.index,
    )
    evaluate_target_health = false
  }
}

