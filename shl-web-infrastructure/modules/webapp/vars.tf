variable "environment" {
  type = string
}

variable "tags" {
  default = {}
}

variable "name" {
  type = string
}

variable "resourcegroup_name" {
  type = any
}

variable "resourcegroup_location" {
  type = any
}

variable "appservice_id" {
  type = string
}

variable "app_settings" {
  type    = map(string)
  default = {}
}

variable "app_settings_slot" {
  type    = map(string)
  default = {}
}

variable "app_connectionstrings" {
  type    = map(string)
  default = {}
}

variable "subnet_id" {
  type    = string
  default = null
}

variable "allowlist_ip" {
  type    = list(any)
  default = []
}

variable "allowlist_subnet" {
  type    = list(any)
  default = []
}

variable "is_static" {
  type    = bool
  default = false
}