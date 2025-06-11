resource "azurerm_resource_group" "main" {
  name     = "rg-web-${var.environment}-001"
  location = var.primaryRegion

  tags = var.tags
  lifecycle {
    ignore_changes = [
      tags
    ]
  }
}

# sdc / gwc = regions abbrs