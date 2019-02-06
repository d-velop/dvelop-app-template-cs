output "source_code_hash" {
  value = "${local.source_code_hash}"
}

output "build_version" {
  value = "${local.build_version}"
}

output "nameserver" {
  value = "${module.hosted_zone.nameserver}"
}

output "endpoint"{
  value = "${module.serverless_lambda_app.endpoints}"
}

