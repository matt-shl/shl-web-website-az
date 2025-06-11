module "access" {
  source  = "app.terraform.io/dept/access/cloudflare"
  version = "0.2.2"
  #   source = "../../modules/terraform-cloudflare-access"

  domain                = "shl-medical.com"
  fw_protect_subdomains = ["(test-.*|acc-.*|prd-.*|staging-.*|cms)"]
  fw_allow_ips = [for ip in module.globalAllowlist.ip_list_cidr : {
    name : ip.name
    address = ip.cidr
  }]

  count = (var.environment == "test") ? 1 : 0
}

# Cloudflare configuration

resource "cloudflare_zone_settings_override" "main" {
  zone_id = data.cloudflare_zone.main.id
  settings {
    always_use_https         = "on"
    automatic_https_rewrites = "on"
    min_tls_version          = "1.2"
    polish                   = "lossless"
    webp                     = "on"
    mirage                   = "on"
    http2                    = "on"
    http3                    = "on"
    h2_prioritization        = "on"
    origin_max_http_version  = "1"
    zero_rtt                 = "on"
    waf                      = "on"
  }

  count = (var.environment == "test") ? 1 : 0
}

# DNS Records

resource "cloudflare_record" "www" {
  zone_id = data.cloudflare_zone.main.zone_id
  name    = (var.environment == "prd") ? "${var.environment}-www" : "${var.environment}-www"
  value   = module.webapp_web[var.primaryRegion].appservice.default_hostname
  type    = "CNAME"
  ttl     = 1
  proxied = true
}

resource "cloudflare_record" "cms" {
  zone_id = data.cloudflare_zone.main.zone_id
  name    = (var.environment == "prd") ? "cms" : "${var.environment}-cms"
  value   = module.webapp_cms[var.primaryRegion].appservice.default_hostname
  type    = "CNAME"
  ttl     = 1
  proxied = true
}

resource "cloudflare_record" "staging-www" {
  zone_id = data.cloudflare_zone.main.zone_id
  name    = "staging-www"
  value   = module.webapp_web[var.primaryRegion].appservice_staging[0].default_hostname
  type    = "CNAME"
  ttl     = 1
  proxied = true

  count = (var.environment == "prd") ? 1 : 0
}

resource "cloudflare_record" "staging-cms" {
  zone_id = data.cloudflare_zone.main.zone_id
  name    = "staging-cms"
  value   = module.webapp_cms[var.primaryRegion].appservice_staging[0].default_hostname
  type    = "CNAME"
  ttl     = 1
  proxied = true

  count = (var.environment == "prd") ? 1 : 0
}