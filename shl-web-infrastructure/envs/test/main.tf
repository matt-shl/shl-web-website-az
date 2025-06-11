terraform {
  cloud {
    organization = "dept"

    workspaces {
      name = "beno-shl-infrastructure-test"
    }
  }
}

module "env" {
  source = "../../modules/environment"

  environment = "test"

  clientAllowlist = [
    {
      name = "Persistent - Office"
      cidr = "165.225.20.225/32"
      }, {
      name = "SHL - Tobias Home"
      cidr = "77.56.33.221/32"
      }, {
      name = "SHL - Tobias Home 2"
      cidr = "112.134.0.0/15"
      }, {
      name = "SHL - Tobias Home 3"
      cidr = "112.134.0.0/15"
      }, {
      name = "Carl - Home"
      cidr = "195.65.131.190/32"
      }, {
      name = "SHL - Carl - Home 2"
      cidr = "2a04:ee41::/32"
      }, {
      name = "SHL - Office"
      cidr = "59.124.130.162/32"
      }, {
      name = "SHL - Milan Home"
      cidr = "31.10.158.247/32"
      }, {
      name = "SHL - Milan Home 2"
      cidr = "31.10.159.135/32"
      }, {
      name = "SHL - Milan Home 3 IPv6"
      cidr = "2a02:aa12:7481:f80::/64"
      }, {
      name = "SHL - Jennifer Home"
      cidr = "12.188.54.220/32"
      }, {
      name = "SHL - Matthew Home"
      cidr = "59.124.192.115/32"
      }, {
      name = "SHL - Matthew Home 2"
      cidr = "218.32.245.240/32"
      }, {
      name = "SHL - Matthew Home 3"
      cidr = "123.193.182.196/32"
      }, {
      name = "SHL Matthew Home 3 IPv6"
      cidr = "2407:4d00:3c00:71c2:0000:0000:0000:0000/64"
      }, {
      name = "SHL Matthew Home 4"
      cidr = "223.165.4.137/32"
      }, {
      name = "SHL Matthew Home 5"
      cidr = "123.192.0.0/14"
      }, {
      name = "SHL Matthew Home 5 IPv6"
      cidr = "2407:4d00::/32"
      }, {
      name = "SHL Matthew Home 6"
      cidr = "103.107.199.180/32"
      }, {
      name = "SHL Matthew Home 7"
      cidr = "172.94.4.237/32"
      }, {
      name = "SHL - Stephanie Home"
      cidr = "59.124.130.162/32"
      }, {
      name = "SHL - Eliane Home"
      cidr = "178.197.192.0/19"
      }, {
      name = "SHL - Eliane Home 2"
      cidr = "213.55.237.4/32"
      }, {
      name = "SHL - Eliane Home 3"
      cidr = "213.55.176.0/20"
      }, {
      name = "SHL - Eliane Home 4"
      cidr = "157.143.0.0/17"
      }, {
      name = "SHL - Eliane Home 5"
      cidr = "31.10.128.0/17"
      }, {
      name = "SHL - Eliane Home 6"
      cidr = "85.0.0.0/14"
      }, {
      name = "SHL - Eliane Home 7"
      cidr = "213.55.224.0/20"
      }, {
      name = "SHL - Eliane Home 8"
      cidr = "92.104.186.69/32"
      }, {
      name = "SHL - Madeesen Home"
      cidr = "213.55.240.109/32"
      }, {
      name = "SHL - Adelia Home"
      cidr = "101.10.0.0/17"
      }, {
      name = "SHL - Adelia Home IPv6"
      cidr = "2402:7500:04e3:e058:0000:0000:0000:0000/64"
      }, {
      name = "SHL - Pablo Home"
      cidr = "42.72.0.0/13"
      }, {
      name = "SHL - Pablo Home IPv6"
      cidr = "2001:b400:e339:a028:0000:0000:0000:0000/64"
      }, {
      name = "SHL - CHRO Home"
      cidr = "195.65.128.0/21"
      }, {
      name = "SHL - Tina Home"
      cidr = "175.98.0.0/16"
      }, {
      name = "Halo Security 1",
      cidr = "74.82.62.128/26"
      }, {
      name = "Halo Security 2",
      cidr = "64.39.96.0/20"
      }, {
      name = "Halo Security 3",
      cidr = "139.87.112.0/23"
      }, {
      name = "Halo Security 4",
      cidr = "139.87.117.141/32"
      }, {
      name = "Halo Security 5",
      cidr = "139.87.105.179/32"
      }, {
      name = "Halo Security 6",
      cidr = "3.18.82.183/32"
      }
  ]

  regions = {
    "swedencentral" : {
      "abbr" : "sdc",
      "short" : "swedencentral",
      "location" : "Sweden Central",
      "cidr" : "10.0.1.0/24",
    }
  }
  tags = {
    "Environment" : "Test"
    "Client" : "SHLMedical"
  }
}