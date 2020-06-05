# ACM Certificates for Cloudfront must be requested in US East
# cf. https://docs.aws.amazon.com/acm/latest/userguide/acm-regions.html
provider "aws" {
  alias   = "virginia"
  version = "~> 2.0"
  region  = "us-east-1"
}

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
  provider                  = aws.virginia

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

  provider = aws.virginia
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

# cf. https://www.terraform.io/docs/providers/aws/r/cloudfront_distribution.html
resource "aws_cloudfront_distribution" "dist" {
  origin {
    origin_id   = sha256(var.origin_domain_name)
    domain_name = var.origin_domain_name
  }

  enabled         = true
  is_ipv6_enabled = true
  aliases         = ["${var.custom_subdomain_name}.${local.hosted_zone_name}"]

  # https://aws.amazon.com/de/cloudfront/pricing/
  price_class = "PriceClass_100"

  default_cache_behavior {
    allowed_methods  = ["GET", "HEAD"]
    cached_methods   = ["GET", "HEAD"]
    compress         = true
    target_origin_id = sha256(var.origin_domain_name)

    forwarded_values {
      query_string = true

      cookies {
        forward = "none"
      }
    }

    viewer_protocol_policy = "https-only"

    # Choose TTL values to delegate the caching behaviour to the origin (that is the max-age header set by the origin server)
    # cf. https://docs.aws.amazon.com/de_de/AmazonCloudFront/latest/DeveloperGuide/Expiration.html#ExpirationDownloadDist
    min_ttl = 0 # default value

    max_ttl = 31536000 # default value

    # The value that you specify for Default TTL applies only when your origin does not add HTTP headers such as Cache-Control max-age, Cache-Control s-maxage, or Expires to objects
    # https://docs.aws.amazon.com/AmazonCloudFront/latest/DeveloperGuide/distribution-web-values-specify.html#DownloadDistValuesDefaultTTL
    default_ttl = 3600
  }

  viewer_certificate {
    minimum_protocol_version = "TLSv1.2_2018"
    ssl_support_method       = "sni-only"
    acm_certificate_arn      = aws_acm_certificate_validation.cert.certificate_arn
  }

  restrictions {
    geo_restriction {
      restriction_type = "none"
    }
  }

  tags = {
    Name       = "dist for ${var.origin_domain_name}"
    Created_By = "Terraform - do not modify in AWS Management Console"
  }
}

resource "aws_route53_record" "dist" {
  zone_id = var.hosted_zone_id
  name    = "${var.custom_subdomain_name}.${local.hosted_zone_name}"
  type    = "A"

  alias {
    name                   = aws_cloudfront_distribution.dist.domain_name
    zone_id                = aws_cloudfront_distribution.dist.hosted_zone_id
    evaluate_target_health = false
  }
}

