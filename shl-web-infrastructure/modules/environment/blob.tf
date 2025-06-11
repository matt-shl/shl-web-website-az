resource "azurerm_storage_account" "main" {
  name                              = "stweb${var.environment}${each.value.abbr}001"
  resource_group_name               = azurerm_resource_group.main.name
  location                          = each.value.location
  account_tier                      = "Standard"
  account_replication_type          = "LRS"
  min_tls_version                   = "TLS1_2"
  infrastructure_encryption_enabled = true

  blob_properties {
    delete_retention_policy {
      days = 31
    }
    container_delete_retention_policy {
      days = 31
    }
  }

  network_rules {
    default_action = "Deny"
    bypass         = ["AzureServices"]
    ip_rules = [
      for item in module.developerAllowlist.ip_list_cidr :
      split("/", item.cidr)[1] == "32" ? split("/", item.cidr)[0] : item.cidr
    ]
    virtual_network_subnet_ids = [for subnet in azurerm_subnet.sp : subnet.id]
  }

  tags = var.tags
  lifecycle {
    prevent_destroy = true
  }

  for_each = var.regions
}

# Blob lifecycle - Makes sure the logs are getting rotated and not stored for longer then 90 days

resource "azurerm_storage_management_policy" "logs_rotate" {
  storage_account_id = azurerm_storage_account.main[each.key].id

  rule {
    name    = "logs-rotate-1"
    enabled = true
    filters {
      prefix_match = [
        "insights-logs-appserviceantivirusscanauditlogs/resourceId=",
        "insights-logs-appserviceauditlogs/resourceId=",
        "insights-logs-appserviceconsolelogs/resourceId=",
        "insights-logs-appservicefileauditlogs/resourceId=",
        "insights-logs-appservicehttplogs/resourceId=",
        "insights-logs-appserviceipsecauditlogs/resourceId=",
        "insights-logs-appserviceplatformlogs/resourceId=",
        "insights-metrics-pt1m/resourceId=",
      ]
      blob_types = [
        "blockBlob",
        "appendBlob",
      ]
    }
    actions {
      base_blob {
        delete_after_days_since_creation_greater_than = 90
      }
    }
  }

  for_each = var.regions
}