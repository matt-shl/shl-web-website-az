resource "random_password" "sql_password" {
  length           = 16
  special          = true
  override_special = "_%@;&="
}

resource "azurerm_mssql_server" "main" {
  name                          = "sql-web-${var.environment}-${var.regions[var.primaryRegion].abbr}"
  resource_group_name           = azurerm_resource_group.main.name
  location                      = var.regions[var.primaryRegion].location
  version                       = "12.0"
  administrator_login           = "shladmin"
  administrator_login_password  = random_password.sql_password.result
  minimum_tls_version           = "1.2"
  public_network_access_enabled = true

  azuread_administrator {
    azuread_authentication_only = "false"
    login_username              = "sqladmin"
    tenant_id                   = var.tenantId
    object_id                   = "d1e7bed9-1223-4589-939c-54167e5680e1"
  }

  tags = var.tags
  lifecycle {
  }
}

resource "azurerm_mssql_server_extended_auditing_policy" "main" {
  server_id              = azurerm_mssql_server.main.id
  log_monitoring_enabled = true
}

# SQL Server Databases

resource "azurerm_mssql_database" "umbraco" {
  name      = "umbraco"
  server_id = azurerm_mssql_server.main.id
  #   collation      = "Latin1_General_CI_AS"
  sku_name             = (var.environment == "prd") ? "S3" : "S0"
  storage_account_type = (var.environment == "prd") ? "Geo" : "Zone"

  tags = var.tags

  lifecycle {
    prevent_destroy = true
  }
}

resource "azurerm_mssql_database" "custom" {
  name      = "custom"
  server_id = azurerm_mssql_server.main.id
  #   collation      = "Latin1_General_CI_AS"
  sku_name             = "S0"
  storage_account_type = (var.environment == "prd") ? "Geo" : "Zone"

  tags = var.tags

  lifecycle {
    prevent_destroy = true
  }
}

# SQL Server Firewall Rules

resource "azurerm_mssql_firewall_rule" "developerAllowlist" {
  name             = "Developer - ${each.value["name"]}"
  server_id        = azurerm_mssql_server.main.id
  start_ip_address = cidrhost(each.value["cidr"], 0)
  end_ip_address   = cidrhost(each.value["cidr"], -1)

  for_each = {
    for index, item in module.developerAllowlist.ip_list_cidr :
    item.name => item
  }
}

resource "azurerm_mssql_virtual_network_rule" "vnet_sp" {
  name      = azurerm_subnet.sp[var.primaryRegion].name
  server_id = azurerm_mssql_server.main.id
  subnet_id = azurerm_subnet.sp[var.primaryRegion].id

  count = (var.environment == "prd") ? 0 : 1
}

resource "azurerm_mssql_firewall_rule" "vnetAllowlist" {
  name             = "VNET - ${each.key}"
  server_id        = azurerm_mssql_server.main.id
  start_ip_address = cidrhost(azurerm_virtual_network.main[each.key].address_space[0], 0)
  end_ip_address   = cidrhost(azurerm_virtual_network.main[each.key].address_space[0], -1)

  for_each = (var.environment == "prd") ? var.regions: {}
}



# Private endpoint to enable connectivity through vnet peering.

resource "azurerm_private_endpoint" "sql" {
  name                = "shl-sql-pep-euwe-${var.environment}-${var.regions[var.primaryRegion].abbr}"
  resource_group_name = azurerm_resource_group.main.name
  location            = var.regions[var.primaryRegion].location
  subnet_id           = azurerm_subnet.pep[var.primaryRegion].id

  private_service_connection {
    name                           = "vnet-connection"
    private_connection_resource_id = azurerm_mssql_server.main.id
    is_manual_connection           = false
    subresource_names              = ["sqlServer"]
  }

  count = (var.environment == "prd") ? 1 : 0
  
  tags = var.tags
}