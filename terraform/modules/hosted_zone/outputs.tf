output "id" {
  value = "${aws_route53_zone.hosted_zone.id}"
}

output "nameserver" {
  value = "${aws_route53_record.ns.records}"
}
