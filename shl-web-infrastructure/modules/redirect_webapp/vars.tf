variable "webapp_name" {
    type = string
}

variable rsg {
    type = object({
        name = string
        location = string
    })
}

variable "appservice_id" {
    type = string
}

variable "environment" {
    type = string
}

variable "tags" {
  default = {}
}

variable subnet_id {
    type = string
    default = ""
}