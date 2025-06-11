resource "cloudflare_load_balancer_monitor" "web" {
  account_id       = data.cloudflare_zone.main.account_id
  type             = "https"
  port             = 443
  expected_codes   = "200"
  method           = "GET"
  timeout          = 10
  path             = "/"
  interval         = 60
  retries          = 2
  description      = "${var.environment}-www"
  allow_insecure   = true
  follow_redirects = true
  # probe_zone     = "shl-medical.com"

  count = (var.environment == "prd") ? 1 : 0
}

resource "cloudflare_load_balancer_pool" "web" {
  account_id    = data.cloudflare_zone.main.account_id
  name          = "${var.environment}-www.shl-medical.com"
  check_regions = ["WEU"]

  dynamic "origins" {
    for_each = module.webapp_web

    content {
      name    = origins.value.appservice.default_hostname
      address = origins.value.appservice.default_hostname
      enabled = true
    }

  }

  enabled         = true
  minimum_origins = 1
  monitor         = cloudflare_load_balancer_monitor.web[0].id

  count = (var.environment == "prd") ? 1 : 0
}

resource "cloudflare_load_balancer" "lb" {
  zone_id          = data.cloudflare_zone.main.zone_id
  name             = "www.shl-medical.com"
  fallback_pool_id = cloudflare_load_balancer_pool.web[0].id
  default_pool_ids = [cloudflare_load_balancer_pool.web[0].id]
  proxied          = true
  session_affinity = "none"
  enabled          = true

  count = (var.environment == "prd") ? 1 : 0
}

resource "cloudflare_healthcheck" "web" {
  zone_id     = data.cloudflare_zone.main.zone_id
  name        = replace(each.value.appservice.default_hostname, ".", "_")
  description = "Health Check for ${each.value.appservice.default_hostname}"
  address     = each.value.appservice.default_hostname
  suspended   = false
  check_regions = [
    "WEU"
  ]
  type   = "HTTPS"
  port   = 443
  method = "GET"
  path   = "/"
  expected_codes = [
    "2xx",
    "301"
  ]
  follow_redirects = true
  allow_insecure   = false
  timeout               = 10
  retries               = 2
  interval              = 30
  consecutive_fails     = 3
  consecutive_successes = 2

  for_each = module.webapp_web
}