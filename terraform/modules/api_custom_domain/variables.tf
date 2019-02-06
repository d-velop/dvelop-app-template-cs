variable "hosted_zone_id" {
  description = "id of the DNS hosted zone"
}

variable "aws_api_gateway_rest_api_id" {
  description = "id of the api gateway"
}

variable "aws_api_gateway_rest_api_endpoint_configuration_types" {
  type        = "list"
  description = "A list of endpoint types. cf. https://www.terraform.io/docs/providers/aws/r/api_gateway_domain_name.html#endpoint_configuration-1"
}

variable "stages" {
  type        = "list"
  description = "Stages for which custom domain names should be created. By convention the 'prod' stage is mapped to the name of the provided hosted zone without the prefix 'prod'"
}