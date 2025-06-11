resource "azurerm_nat_gateway" "main" {
  name                    = "ng-web-${var.environment}-${each.value.abbr}-001"
  location                = each.value.location
  resource_group_name     = azurerm_resource_group.main.name
  sku_name                = "Standard"
  idle_timeout_in_minutes = 10

  tags = var.tags

  for_each = var.regions
}

resource "azurerm_public_ip" "nat" {
  name                = "pip-web-${var.environment}-${each.value.abbr}-001"
  location            = each.value.location
  resource_group_name = azurerm_resource_group.main.name
  sku                 = "Standard"
  allocation_method   = "Static"
  ip_version          = "IPv4"

  zones = [
    1,
    2,
    3
  ]

  tags = var.tags
  lifecycle {
    prevent_destroy = true
  }

  for_each = var.regions
}

# Primary NAT association 
resource "azurerm_subnet_nat_gateway_association" "subnet_default" {
  subnet_id      = azurerm_subnet.default[each.key].id
  nat_gateway_id = azurerm_nat_gateway.main[each.key].id

  for_each = var.regions
}
resource "azurerm_subnet_nat_gateway_association" "subnet_sp" {
  subnet_id      = azurerm_subnet.sp[each.key].id
  nat_gateway_id = azurerm_nat_gateway.main[each.key].id

  for_each = var.regions
}

resource "azurerm_nat_gateway_public_ip_association" "main" {
  nat_gateway_id       = azurerm_nat_gateway.main[each.key].id
  public_ip_address_id = azurerm_public_ip.nat[each.key].id

  for_each = var.regions
}