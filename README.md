# PlatformEngineerAssessment

## Overview

This project demonstrates a production-oriented platform engineering assessment built using .NET 8, Azure Services, Docker, Terraform, and Azure DevOps CI/CD.

The solution consists of:
- A REST API project responsible for accepting work requests
- A background Worker service responsible for asynchronous processing
- Azure Service Bus for messaging
- Docker containers for deployment packaging
- Terraform for Infrastructure as Code (IaC)
- Azure DevOps YAML pipeline for CI/CD automation

The implementation follows a decoupled API + Worker architecture commonly used in cloud-native distributed systems.

---

# Architecture

Client Request
↓
PlatformEngineer.Api
↓
Azure Service Bus Queue
↓
PlatformEngineer.Worker
↓
Message Processing

---

# Solution Structure

```text
PlatformEngineerAssessment/
│
├── PlatformEngineer.Api/
│   ├── Controllers/
│   ├── Models/
│   ├── Services/
│   ├── Dockerfile
│   ├── Program.cs
│   └── PlatformEngineer.Api.csproj
│
├── PlatformEngineer.Worker/
│   ├── Models/
│   ├── Worker.cs
│   ├── Dockerfile
│   ├── Program.cs
│   └── PlatformEngineer.Worker.csproj
│
├── terraform/
│   ├── main.tf
│   └── providers.tf
│
├── docker-compose.yml
├── azure-pipelines.yml
└── README.md
```

---

# Implemented Features

## API Endpoints

### Health Endpoints

| Method | Endpoint | Description |
|---|---|---|
| GET | `/health/live` | Liveness probe |
| GET | `/health/ready` | Readiness probe |

### Work Endpoints

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/work` | Returns processed work items |
| POST | `/api/work` | Sends work message to Azure Service Bus queue |

---

# Functional Workflow

## POST /api/work

- Accepts incoming work request
- Serializes payload
- Sends message to Azure Service Bus Queue

## Background Worker

- Runs independently as a Worker Service
- Continuously polls Azure Service Bus queue
- Processes queued messages asynchronously
- Logs processed messages

## GET /api/work

- Returns processed work items

---

# Technologies Used

| Technology | Purpose |
|---|---|
| .NET 8 | Application framework |
| ASP.NET Core Web API | REST API |
| Background Worker Service | Asynchronous processing |
| Azure Service Bus | Messaging queue |
| Docker | Containerization |
| Docker Compose | Multi-container local execution |
| Terraform | Infrastructure provisioning |
| Azure DevOps Pipelines | CI/CD |
| Azure Container Registry (ACR) | Container image storage |
| Azure Container Apps | Container hosting |
| Log Analytics Workspace | Monitoring integration |

---

# Docker Implementation

Both API and Worker services are containerized using separate Dockerfiles.

## API Dockerfile

- Multi-stage Docker build
- SDK image for build
- ASP.NET runtime image for execution
- Exposes port 80

## Worker Dockerfile

- Multi-stage Docker build
- Runs Worker Service independently

## Docker Compose

`docker-compose.yml` is used to run API and Worker together locally.


# Terraform Infrastructure

Terraform is used to provision Azure infrastructure.

## Provisioned Azure Resources

### Resource Group

```hcl
resource "azurerm_resource_group" "rg"
```

Creates:
- platform-rg

---

### Azure Service Bus Namespace

```hcl
resource "azurerm_servicebus_namespace" "sb"
```

Creates:
- platformsb2026

---

### Azure Service Bus Queue

```hcl
resource "azurerm_servicebus_queue" "queue"
```

Creates:
- work-queue

---

### Azure Container Registry

```hcl
resource "azurerm_container_registry" "acr"
```

Creates:
- psacontainer

Used for:
- Storing Docker container images

---

### Log Analytics Workspace

```hcl
resource "azurerm_log_analytics_workspace" "law"
```

Creates:
- platform-law

Used for:
- Monitoring and diagnostics

---

### Azure Container App Environment

```hcl
resource "azurerm_container_app_environment" "env"
```

Creates:
- platform-container-env

Used for:
- Hosting containerized applications

---

# Terraform Commands


```bash
terraform init
```

```bash
terraform validate
```

```bash
terraform plan
```

```bash
terraform apply
```

---

# Azure DevOps CI/CD Pipeline

The project includes Azure DevOps YAML pipeline automation.

## Pipeline Capabilities

- Restore .NET dependencies
- Build API and Worker projects
- Build Docker images
- Push images to Azure Container Registry
- Deploy containers to Azure

---

# Pipeline File

```text
azure-pipelines.yml
```

---

# Local Development Steps

## Prerequisites

Install:
- .NET 8 SDK
- Docker Desktop
- Terraform
- Azure CLI

---

# Run Locally

## Restore Projects

```bash
dotnet restore PlatformEngineer.Api/PlatformEngineer.Api.csproj

dotnet restore PlatformEngineer.Worker/PlatformEngineer.Worker.csproj
```

---

## Build Projects

```bash
dotnet build PlatformEngineer.Api/PlatformEngineer.Api.csproj

dotnet build PlatformEngineer.Worker/PlatformEngineer.Worker.csproj
```

---

## Run API

```bash
cd PlatformEngineer.Api
dotnet run
```

Swagger:
```text
https://localhost:<port>/swagger
```

---

## Run Worker

```bash
cd PlatformEngineer.Worker
dotnet run
```

---

# Docker Local Run

## Build Containers

```bash
docker-compose build
```

## Start Containers

```bash
docker-compose up
```

---

# Example Request

## POST /api/work

```json
{
  "applicationName": "sample-app",
  "environment": "dev"
}
```

---

# Design Decisions

## Why Separate API and Worker?

This approach provides:
- Better scalability
- Independent deployments
- Production-style architecture

---

# Challenges Resolved During Implementation

- .NET SDK compatibility issues
- global.json SDK resolution issues
- Docker build path issues
- Azure Service Bus configuration
- Terraform provider registration
- Azure DevOps pipeline path corrections
- GitHub configuration

---


# Final Outcome and validation

The assessment now supports:

- .NET 8 API
- Background Worker Service
- Azure Service Bus integration
- Docker containerization
- Docker Compose local orchestration
- Terraform-based Azure infrastructure
- Azure DevOps CI/CD automation
- Health endpoints
- Asynchronous message processing