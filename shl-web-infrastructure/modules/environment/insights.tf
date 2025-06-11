resource "azurerm_application_insights" "main" {
  name                = "appi-web-${var.environment}-${var.regions[var.primaryRegion].abbr}"
  location            = var.regions[var.primaryRegion].location
  resource_group_name = azurerm_resource_group.main.name
  application_type    = "web"
  workspace_id        = azurerm_log_analytics_workspace.main.id

  daily_data_cap_in_gb = 10
  retention_in_days    = (var.environment == "prd") ? 90 : 30

  tags = var.tags
}

resource "azurerm_log_analytics_workspace" "main" {
  name                = "log-web-${var.environment}-${var.regions[var.primaryRegion].abbr}"
  location            = var.regions[var.primaryRegion].location
  resource_group_name = azurerm_resource_group.main.name
  sku                 = "PerGB2018"
  retention_in_days   = (var.environment == "prd") ? 90 : 30
  daily_quota_gb      = 10
}