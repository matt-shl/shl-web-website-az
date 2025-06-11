terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~>3.111.0"
    }

    cloudflare = {
      source  = "cloudflare/cloudflare"
      version = "~> 4.0"
    }
  }
}
provider "azurerm" {
  features {}

  # skip_provider_registration = true
}

data "azurerm_client_config" "current" {}
data "cloudflare_ip_ranges" "main" {}
data "cloudflare_zone" "main" {
  name = "shl-medical.com"
}