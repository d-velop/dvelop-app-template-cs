output "source_code_hash" {
  value = "${local.source_code_hash}"
}

output "build_version" {
  value = "${local.build_version}"
}

output "endpoint"{
  value = "${module.serverless_lambda_app.endpoints}"
}

output "domain"{
  value="${var.appname}${var.domainsuffix}"
}

