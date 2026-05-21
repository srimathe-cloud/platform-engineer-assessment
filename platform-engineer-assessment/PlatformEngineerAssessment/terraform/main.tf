data "azurerm_client_config" "current" {}

resource "azurerm_resource_group" "rg" {
  name     = "platform-rg"
  location = "Central India"
}

resource "azurerm_servicebus_namespace" "sb" {
  name                = "platformsb2026"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  sku                 = "Standard"
}

resource "azurerm_servicebus_queue" "queue" {
  name         = "work-queue"
  namespace_id = azurerm_servicebus_namespace.sb.id
}

resource "azurerm_container_registry" "acr" {
  name                = "psacontainer"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  sku                 = "Basic"
  admin_enabled       = true
}

resource "azurerm_log_analytics_workspace" "law" {
  name                = "platform-law"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  sku                 = "PerGB2018"
  retention_in_days   = 30
}

resource "azurerm_container_app_environment" "env" {
  name                       = "platform-container-env"
  location                   = azurerm_resource_group.rg.location
  resource_group_name        = azurerm_resource_group.rg.name
  log_analytics_workspace_id = azurerm_log_analytics_workspace.law.id
}
