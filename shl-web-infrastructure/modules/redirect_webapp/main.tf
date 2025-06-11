resource "azurerm_linux_web_app" "main" {
  name                = var.webapp_name
  resource_group_name = var.rsg.name
  location            = var.rsg.location
  service_plan_id     = var.appservice_id

  https_only                = true
  app_settings              = {

  }

  site_config {
    http2_enabled                 = true
    vnet_route_all_enabled        = true
    health_check_path             = "/healthz"
    ip_restriction_default_action = "Deny"
    app_command_line              = "/home/site/startup.sh"

    application_stack {
      php_version    = "8.2"
    }

    ip_restriction {
        ip_address  = "0.0.0.0/0"
        name        = "Allow-All"
        action      = "Allow"
        priority    = 100
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