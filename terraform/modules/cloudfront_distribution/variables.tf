variable "origin_domain_name" {
  description = "The DNS domain name of either the S3 bucket, or web site of your custom origin. cf. https://www.terraform.io/docs/providers/aws/r/cloudfront_distribution.html#domain_name"
}

variable "custom_subdomain_name" {
  description = "Name of the custom subdomain for this distribution. Will be a subdomain of the provided hosted zone"
}

variable "hosted_zone_id" {
  description = "id of the DNS hosted zone"
}

