resource "azurerm_private_dns_zone" "sql" {
  name                = "privatelink.database.windows.net"
  resource_group_name = azurerm_resource_group.main.name

  tags = var.tags

  count = (var.environment == "prd") ? 1 : 0
}

resource "azurerm_private_dns_zone_virtual_network_link" "sql" {
  name                  = "shl-vnet-sql-${each.value.abbr}-${var.environment}"
  resource_group_name   = azurerm_resource_group.main.name
  private_dns_zone_name = azurerm_private_dns_zone.sql[0].name
  virtual_network_id    = azurerm_virtual_network.main[each.key].id

  tags = var.tags

  for_each = (var.environment == "prd") ? var.regions : {}
}

resource "azurerm_private_dns_a_record" "sql" {
  name                = azurerm_mssql_server.main.name
  zone_name           = azurerm_private_dns_zone.sql[0].name
  resource_group_name = azurerm_resource_group.main.name
  ttl                 = 300
  records             = [azurerm_private_endpoint.sql[0].private_service_connection[0].private_ip_address]

  count = (var.environment == "prd") ? 1 : 0
}