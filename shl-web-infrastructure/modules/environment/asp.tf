resource "azurerm_service_plan" "main" {
  name                = "asp-web-${var.environment}-${each.value.abbr}-001"
  resource_group_name = azurerm_resource_group.main.name
  location            = each.value.location
  os_type             = "Linux"
  sku_name            = "P1v2"

  tags = var.tags

  for_each = var.regions
}

resource "azurerm_service_plan" "cms" {
  name                = "asp-spcms-${var.environment}-${each.value.abbr}-001"
  resource_group_name = azurerm_resource_group.main.name
  location            = each.value.location
  os_type             = "Linux"
  sku_name = var.environment == "prd" ? "P2v2" : "P1v2"

  tags = var.tags

  for_each = (var.environment == "acc" || var.environment == "prd") ? { for k, v in var.regions : k => v if v.short == var.primaryRegion } : {}
}