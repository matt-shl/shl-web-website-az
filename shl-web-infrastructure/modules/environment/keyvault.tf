module "keyvault" {
  source  = "app.terraform.io/dept/keyvault/azure"
  version = "0.2.0"

  rsg_name                = azurerm_resource_group.main.name
  keyvault_name           = "kv-web-${var.environment}-${var.regions[var.primaryRegion].abbr}"
  keyvault_tenantid       = var.tenantId
  keyvault_access_default = "Deny"
  keyvault_access_bypass  = "AzureServices"
  keyvault_policies = concat(
    [
      {
        "tenantId" : var.tenantId,
        "objectId" : "c8cd696d-defd-4c23-a583-a42b057e04d0", # SHL-DEPT-EXTERN-SE-C-Reader
        "permissions" : {
          "secrets" : [
            "All",
          ],
        }
      },
      {
        "tenantId" : var.tenantId,
        "objectId" : "d1e7bed9-1223-4589-939c-54167e5680e1", # SHL-DEPT-EXTERN-SE-C-Contributor
        "permissions" : {
          "secrets" : [
            "All",
          ],
        }
      }
    ],
    [for webapp in module.webapp_web : {
      "tenantId" : var.tenantId,
      "objectId" : webapp.appservice.identity[0].principal_id,
      "permissions" : {
        "secrets" : [
          "Get",
          "List",
        ],
      }
    }],
    [for webapp in module.webapp_cms : {
      "tenantId" : var.tenantId,
      "objectId" : webapp.appservice.identity[0].principal_id,
      "permissions" : {
        "secrets" : [
          "Get",
          "List",
        ],
      }
    }],

    var.environment == "prd" ? concat(
      [for webapp in module.webapp_web : {
        "tenantId" : var.tenantId,
        "objectId" : webapp.appservice_staging[0].identity[0].principal_id,
        "permissions" : {
          "secrets" : [
            "Get",
            "List",
          ],
        }
      }],
      [for webapp in module.webapp_cms : {
        "tenantId" : var.tenantId,
        "objectId" : webapp.appservice_staging[0].identity[0].principal_id,
        "permissions" : {
          "secrets" : [
            "Get",
            "List",
          ],
        }
      }],
    ) : []
  )
  keyvault_access_ip = [
    for item in module.developerAllowlist.ip_list_cidr : { value : item.cidr }
  ]

  keyvault_access_virtualnetwork = [for subnet in azurerm_subnet.sp : {
    id : subnet.id
    ignoreMissingVnetServiceEndpoint : "false"
  }]

  keyvault_secrets = [
    {
      name : "UmbracoDBDSN"
      value : "Server=${azurerm_mssql_server.main.name}.database.windows.net;Initial Catalog=${azurerm_mssql_database.umbraco.name};User Id=${azurerm_mssql_server.main.administrator_login}@${azurerm_mssql_server.main.name};Password='${random_password.sql_password.result}';Trusted_Connection=False;Encrypt=True;Connection Timeout=30"
    },
    {
      name : "BlobConnectionString"
      value : azurerm_storage_account.main[var.primaryRegion].primary_blob_connection_string
    }
  ]
  tags = var.tags
}