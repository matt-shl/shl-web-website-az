module "webapp_redirect" {
    source = "../redirect_webapp"

    webapp_name = "app-redirect-${var.environment}-${each.value.abbr}"
    rsg = azurerm_resource_group.main
    appservice_id = azurerm_service_plan.main[var.primaryRegion].id
    environment = var.environment
    tags = var.tags

    # count = var.environment == "prd" ? 1 : 0
    for_each = var.environment == "prd" ? { for k, v in var.regions : k => v if v.short == var.primaryRegion } : {}
}