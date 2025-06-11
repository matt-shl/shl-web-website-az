variable "environment" {
  type = string
}

variable "tags" {
  default = {}
}

variable "clientAllowlist" {
  type = list(object({
    name = string
    cidr = string
  }))
  default = []
}

module "clientAllowlist" {
  source  = "app.terraform.io/dept/cidr/general"
  version = "0.1.2"
  ip_expand = false

  ip_list = var.clientAllowlist
}

variable "developerAllowlist" {
  type = list(object({
    name = string
    cidr = string
  }))
  default = [
    {
      name = "Dept Cato EMEA - London"
      cidr = "85.255.26.168/32"
      }, {
      name = "Dept Cato EMEA - Amsterdam"
      cidr = "85.255.20.162/32"
      }, {
      name = "Dept Cato EMEA - Frankfurt"
      cidr = "85.255.29.206/32"
    }
  ]
}

module "developerAllowlist" {
  source  = "app.terraform.io/dept/cidr/general"
  version = "0.1.2"

  ip_list = var.developerAllowlist
  ip_expand = false
}

module "globalAllowlist" {
  source  = "app.terraform.io/dept/cidr/general"
  version = "0.1.2"

  ip_list = concat(var.developerAllowlist, var.clientAllowlist)
  ip_expand = false
}

variable "regions" {
  default = {
    "swedencentral" : {
      "abbr" : "sdc",
      "short" : "swedencentral",
      "location" : "Sweden Central",
      "cidr" : "10.0.1.0/24",
    }
  }

  type = map(object({
    abbr : string
    short : string
    location : string
    cidr : string
  }))
}

variable "primaryRegion" {
  default = "swedencentral"
}

variable "tenantId" {
  default = "e18f907f-8bc5-4fa9-a207-f60b33b2cc15"
}