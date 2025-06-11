resource "azurerm_linux_web_app" "main" {
  name                = var.name
  resource_group_name = var.resourcegroup_name
  location            = var.resourcegroup_location
  service_plan_id     = var.appservice_id

  https_only                = true
  virtual_network_subnet_id = var.subnet_id
  app_settings              = var.app_settings

  dynamic "connection_string" {
    for_each = var.app_connectionstrings
    content {
      name  = connection_string.key
      value = connection_string.value
      type  = "SQLAzure"
    }
  }

  dynamic "sticky_settings" {
    for_each = length(var.app_settings_slot) > 0 ? [true] : []
    content {
      app_setting_names = [for setting, key in var.app_settings_slot: setting]
    }
  }

  site_config {
    http2_enabled                 = true
    vnet_route_all_enabled        = true
    health_check_path             = "/"
    ip_restriction_default_action = "Deny"

    application_stack {
      dotnet_version = var.is_static == false ? "8.0" : null
      php_version    = var.is_static == true ? "8.2" : null
    }

    dynamic "ip_restriction" {
      for_each = var.allowlist_subnet
      content {
        virtual_network_subnet_id = ip_restriction.value["id"]
        name                      = "Allow-VNET-${ip_restriction.key + 1}"
        action                    = "Allow"
        priority                  = 100
      }
    }

    dynamic "ip_restriction" {
      for_each = var.allowlist_ip
      content {
        ip_address = ip_restriction.value["cidr"]
        name       = ip_restriction.value["name"]
        action     = "Allow"
        priority   = 300
      }
    }
  }

  identity {
    type = "SystemAssigned"
  }

  tags = var.tags
  lifecycle {
    ignore_changes = [
      site_config.0.application_stack.0.php_version
    ]
  }
}

resource "azurerm_linux_web_app_slot" "staging" {
  name           = "staging"
  app_service_id = azurerm_linux_web_app.main.id

  https_only                = true
  virtual_network_subnet_id = var.subnet_id
  app_settings              = merge(var.app_settings, var.app_settings_slot)

  dynamic "connection_string" {
    for_each = var.app_connectionstrings
    content {
      name  = connection_string.key
      value = connection_string.value
      type  = "SQLAzure"
    }
  }

  site_config {
    http2_enabled                 = true
    vnet_route_all_enabled        = true
    health_check_path             = "/"
    ip_restriction_default_action = "Deny"

    application_stack {
      dotnet_version = var.is_static == false ? "8.0" : null
      php_version    = var.is_static == true ? "8.2" : null
    }

    dynamic "ip_restriction" {
      for_each = var.allowlist_subnet
      content {
        virtual_network_subnet_id = ip_restriction.value["id"]
        name                      = "Allow-VNET-${ip_restriction.key + 1}"
        action                    = "Allow"
        priority                  = 100
      }
    }

    dynamic "ip_restriction" {
      for_each = var.allowlist_ip
      content {
        ip_address = ip_restriction.value["cidr"]
        name       = ip_restriction.value["name"]
        action     = "Allow"
        priority   = 300
      }
    }
  }

  identity {
    type = "SystemAssigned"
  }

  tags = var.tags
  lifecycle {
    ignore_changes = [
      site_config.0.application_stack.0.php_version
    ]
  }

  count = var.environment == "prd" ? 1 : 0
}