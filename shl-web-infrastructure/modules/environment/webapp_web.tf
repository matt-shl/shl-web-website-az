locals {
  webapp_web_appsettings = {
    # App Insights settings
    "APPINSIGHTS_INSTRUMENTATIONKEY"                  = azurerm_application_insights.main.instrumentation_key
    "APPLICATIONINSIGHTS_CONNECTION_STRING"           = azurerm_application_insights.main.connection_string
    "APPINSIGHTS_PROFILERFEATURE_VERSION"             = "1.0.0"
    "APPINSIGHTS_SNAPSHOTFEATURE_VERSION"             = "1.0.0"
    "ApplicationInsightsAgent_EXTENSION_VERSION"      = "~2"
    "DiagnosticServices_EXTENSION_VERSION"            = "~3"
    "InstrumentationEngine_EXTENSION_VERSION"         = "disabled"
    "SnapshotDebugger_EXTENSION_VERSION"              = "disabled"
    "XDT_MicrosoftApplicationInsights_BaseExtensions" = "disabled"
    "XDT_MicrosoftApplicationInsights_Java"           = "1"
    "XDT_MicrosoftApplicationInsights_Mode"           = "recommended"
    "XDT_MicrosoftApplicationInsights_NodeJS"         = "1"
    "XDT_MicrosoftApplicationInsights_PreemptSdk"     = "disabled"

    # Application settings
    "Umbraco__Storage__AzureBlob__Media__ConnectionString" = "@Microsoft.KeyVault(SecretUri=${module.keyvault.keyvault_uri}secrets/BlobConnectionString/)"
    "ASPNETCORE_ENVIRONMENT"                               = (var.environment == "prd") ? "Production" : (var.environment == "acc") ? "Acceptance" : "Test"
    "Application__ServerRole"                              = "Subscriber"
  }
  webapp_web_connectionstrings = {
    "UmbracoDBDSN" = "@Microsoft.KeyVault(SecretUri=${module.keyvault.keyvault_uri}secrets/UmbracoDBDSN/)"
  }
}

module "webapp_web" {
  source = "../webapp"

  name                   = "app-web-${var.environment}-${each.value.abbr}"
  resourcegroup_name     = azurerm_resource_group.main.name
  resourcegroup_location = each.value.location
  appservice_id          = azurerm_service_plan.main[each.key].id
  environment            = var.environment
  tags                   = var.tags

  subnet_id    = azurerm_subnet.sp[each.key].id
  allowlist_ip = concat(module.clientAllowlist.ip_list_cidr, module.developerAllowlist.ip_list_cidr, [for key, ip in data.cloudflare_ip_ranges.main.ipv4_cidr_blocks : { name = "Cloudflare #${key + 1}", cidr = ip }])
  allowlist_subnet = [for subnet in azurerm_virtual_network.main[each.key].subnet : {
    id : subnet.id
  }]
  app_settings          = local.webapp_web_appsettings
  app_connectionstrings = local.webapp_web_connectionstrings

  for_each = var.regions
}

module "logging_webapp_web" {
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

  for_each = module.webapp_web
}

module "logging_webapp_web_staging" {
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

  for_each = (var.environment == "prd") ? module.webapp_web : {}
}