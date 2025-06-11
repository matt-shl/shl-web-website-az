locals {
  webapp_cms_appsettings = merge(local.webapp_web_appsettings, {
    "Application__ServerRole" = "SchedulingPublisher"
  })
  webapp_cms_appsettings_slot = {
    "Application__ServerRole" = "Subscriber"
  }
  webapp_cms_connectionstrings = local.webapp_web_connectionstrings
}

module "webapp_cms" {
  source = "../webapp"

  name                   = "app-cms-${var.environment}-${each.value.abbr}"
  resourcegroup_name     = azurerm_resource_group.main.name
  resourcegroup_location = each.value.location
  appservice_id          = (var.environment == "acc" || var.environment == "prd") ? azurerm_service_plan.cms[each.key].id : azurerm_service_plan.main[each.key].id
  environment            = var.environment
  tags                   = var.tags

  subnet_id    = azurerm_subnet.sp[each.key].id
  allowlist_ip = concat(module.clientAllowlist.ip_list_cidr, module.developerAllowlist.ip_list_cidr, [for key, ip in data.cloudflare_ip_ranges.main.ipv4_cidr_blocks : { name = "Cloudflare #${key + 1}", cidr = ip }])
  allowlist_subnet = [for subnet in azurerm_virtual_network.main[each.key].subnet : {
    id : subnet.id
  }]
  app_settings          = local.webapp_cms_appsettings
  app_settings_slot     = local.webapp_cms_appsettings_slot
  app_connectionstrings = local.webapp_cms_connectionstrings

  # for_each = var.regions
  for_each = { for k, v in var.regions : k => v if v.short == var.primaryRegion }
}

module "logging_webapp_cms" {
  source  = "app.terraform.io/dept/logging/azure"
  version = "0.1.0"

  enabled_logs = [
    "AppServiceAntivirusScanAuditLogs",
    "AppServiceAppLogs",
    "AppServiceAuditLogs",
    "AppServiceConsoleLogs",
    "AppServiceFileAuditLogs",
    "AppServiceHTTPLogs",
    "AppServiceIPSecAuditLogs",
    "AppServicePlatformLogs"
  ]
  enabled_logs_groups = []

  resource_id = each.value.appservice.id
  storage_id  = azurerm_storage_account.main[each.key].id

  for_each = module.webapp_cms
}

module "logging_webapp_cms_staging" {
  source  = "app.terraform.io/dept/logging/azure"
  version = "0.1.0"

  enabled_logs = [
    "AppServiceAntivirusScanAuditLogs",
    "AppServiceAppLogs",
    "AppServiceAuditLogs",
    "AppServiceConsoleLogs",
    "AppServiceFileAuditLogs",
    "AppServiceHTTPLogs",
    "AppServiceIPSecAuditLogs",
    "AppServicePlatformLogs"
  ]
  enabled_logs_groups = []

  resource_id = each.value.appservice_staging[0].id
  storage_id  = azurerm_storage_account.main[each.key].id

  for_each = (var.environment == "prd") ? module.webapp_cms : {}
}