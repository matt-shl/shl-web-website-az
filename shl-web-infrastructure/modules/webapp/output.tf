output "appservice" {
  value = azurerm_linux_web_app.main
}

output "appservice_staging" {
  value = azurerm_linux_web_app_slot.staging
}