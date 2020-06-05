# cf. https://www.terraform.io/docs/providers/aws/r/kms_key.html
resource "aws_kms_key" "encryption_key" {
  description             = "Encryption kms key"
  deletion_window_in_days = 30

  policy = <<POLICY
{
  "Version" : "2012-10-17",
  "Id" : "encryption-key",
  "Statement" : [
    {
        "Sid": "Enable IAM User Permissions",
        "Effect": "Allow",
        "Principal": {
            "AWS": "${var.principal}"
        },
        "Action": "kms:*",
        "Resource": "*"
    },
    {
      "Effect": "Allow",
      "Principal": { "Service": "logs.eu-central-1.amazonaws.com" },
      "Action": [ 
        "kms:Encrypt*",
        "kms:Decrypt*",
        "kms:ReEncrypt*",
        "kms:GenerateDataKey*",
        "kms:Describe*"
      ],
      "Resource": "*",
      "Condition": {
                "ArnLike": {
                    "kms:EncryptionContext:aws:logs:arn": "arn:aws:logs:eu-central-1:*:log-group:/aws/lambda/${var.appname}*"
                }
            }
    }  
  ]
}
POLICY

  lifecycle {
    prevent_destroy = true
  }
}

# cf. https://www.terraform.io/docs/providers/aws/r/kms_alias.html
resource "aws_kms_alias" "kms_key_alias" {
  name          = "alias/${var.appname}-kms-key"
  target_key_id = aws_kms_key.encryption_key.key_id
}