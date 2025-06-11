# Vnets

resource "azurerm_virtual_network" "main" {
  name                = "vnet-web-${var.environment}-${each.value.abbr}"
  location            = each.value.location
  resource_group_name = azurerm_resource_group.main.name
  address_space       = [each.value.cidr]

  tags = var.tags
  lifecycle {
    ignore_changes = [
      subnet
    ]
  }

  for_each = var.regions
}

# Vnet Peering

resource "azurerm_virtual_network_peering" "primary_secondary" {
  name                         = "peer-sdc-${var.environment}-${each.value.abbr}-001"
  resource_group_name          = azurerm_resource_group.main.name
  virtual_network_name         = azurerm_virtual_network.main[var.primaryRegion].name
  remote_virtual_network_id    = azurerm_virtual_network.main[each.key].id
  allow_virtual_network_access = true
  allow_forwarded_traffic      = true

  for_each = { for k, v in var.regions : k => v if v.short != var.primaryRegion }
}

resource "azurerm_virtual_network_peering" "secondary_primary" {
  name                         = "peer-${each.value.abbr}-${var.environment}-sdc-001"
  resource_group_name          = azurerm_resource_group.main.name
  virtual_network_name         = azurerm_virtual_network.main[each.key].name
  remote_virtual_network_id    = azurerm_virtual_network.main[var.primaryRegion].id
  allow_virtual_network_access = true
  allow_forwarded_traffic      = true

  for_each = { for k, v in var.regions : k => v if v.short != var.primaryRegion }
}

# Vnet Subnets

resource "azurerm_subnet" "default" {
  name                 = "snet-default-${each.value.abbr}-001"
  resource_group_name  = azurerm_resource_group.main.name
  virtual_network_name = azurerm_virtual_network.main[each.key].name
  address_prefixes     = [cidrsubnet(each.value.cidr, 4, 0)]
  service_endpoints    = ["Microsoft.ServiceBus", (length(var.regions) <= 1 ? "Microsoft.Storage" : "Microsoft.Storage.Global"), "Microsoft.Web", "Microsoft.Sql", "Microsoft.KeyVault"]

  for_each = var.regions
}

resource "azurerm_subnet" "pep" {
  name                 = "snet-pep-${each.value.abbr}-001"
  resource_group_name  = azurerm_resource_group.main.name
  virtual_network_name = azurerm_virtual_network.main[each.key].name
  address_prefixes     = [cidrsubnet(each.value.cidr, 4, 1)]
  service_endpoints    = ["Microsoft.ServiceBus", (length(var.regions) <= 1 ? "Microsoft.Storage" : "Microsoft.Storage.Global"), "Microsoft.Web", "Microsoft.Sql", "Microsoft.KeyVault"]

  for_each = var.regions
}

resource "azurerm_subnet" "sp" {
  name                 = "snet-sp-${each.value.abbr}-001"
  resource_group_name  = azurerm_resource_group.main.name
  virtual_network_name = azurerm_virtual_network.main[each.key].name
  address_prefixes     = (var.environment == "prd" && (each.value.abbr == "sdc")) ? [cidrsubnet(each.value.cidr, 4, 2)] : [cidrsubnet(each.value.cidr, 3, 1)]
  service_endpoints    = ["Microsoft.ServiceBus", (length(var.regions) <= 1 ? "Microsoft.Storage" : "Microsoft.Storage.Global"), "Microsoft.Web", "Microsoft.Sql", "Microsoft.KeyVault"]

  delegation {
    name = "sp-delegation"
    service_delegation {
      name    = "Microsoft.Web/serverFarms"
      actions = ["Microsoft.Network/virtualNetworks/subnets/action"]
    }
  }

  for_each = var.regions
}