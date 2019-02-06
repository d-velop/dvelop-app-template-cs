# cf. https://www.terraform.io/docs/providers/aws/d/route53_zone.html
resource "aws_route53_zone" "hosted_zone" {
  name = "${var.hosted_zone_name}"
}

# cf. https://www.terraform.io/docs/providers/aws/r/route53_record.html
resource "aws_route53_record" "ns" {
  zone_id = "${aws_route53_zone.hosted_zone.zone_id}"
  name    = "${aws_route53_zone.hosted_zone.name}"
  type    = "NS"
  ttl     = "30"

  records = [
    "${aws_route53_zone.hosted_zone.name_servers.0}",
    "${aws_route53_zone.hosted_zone.name_servers.1}",
    "${aws_route53_zone.hosted_zone.name_servers.2}",
    "${aws_route53_zone.hosted_zone.name_servers.3}",
  ]
}
