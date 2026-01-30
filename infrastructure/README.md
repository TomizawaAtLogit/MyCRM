# Azure Infrastructure for MyCRM

This directory contains the Infrastructure as Code (IaC) definitions for deploying the MyCRM application to Azure.

## Overview

The infrastructure is defined using Azure Bicep templates and includes:

- **App Services**: Frontend and Backend applications with staging slots
- **PostgreSQL Flexible Server**: Database with high availability (production)
- **Azure Blob Storage**: File storage with private endpoints
- **Virtual Network**: Private network for intranet-only access
- **Private Endpoints**: Secure connections to PaaS services
- **Managed Identities**: Secure authentication between services

## Files

- `main.bicep`: Main infrastructure template
- `parameters.staging.json`: Parameters for staging environment
- `parameters.production.json`: Parameters for production environment

## Deployment

### Prerequisites

1. Azure CLI installed and authenticated
2. Appropriate Azure subscription permissions
3. GitHub secrets configured (see DEPLOYMENT.md)

### Deploy Staging Environment

```bash
# Create resource group
az group create \
  --name rg-mycrm-staging \
  --location eastus

# Deploy infrastructure
az deployment group create \
  --resource-group rg-mycrm-staging \
  --template-file main.bicep \
  --parameters parameters.staging.json
```

### Deploy Production Environment

```bash
# Create resource group
az group create \
  --name rg-mycrm-production \
  --location eastus

# Deploy infrastructure
az deployment group create \
  --resource-group rg-mycrm-production \
  --template-file main.bicep \
  --parameters parameters.production.json
```

### Using GitHub Actions

The recommended way to deploy infrastructure is through GitHub Actions:

1. Navigate to GitHub Actions
2. Select "Deploy Infrastructure" workflow
3. Choose environment (staging or production)
4. Click "Run workflow"

## Architecture

```
┌─────────────────────────────────────────────────────────┐
│                   Corporate Network                      │
│                  (VPN/Intranet Only)                     │
└───────────────────────┬─────────────────────────────────┘
                        │
                        │ VNet Peering
                        │
┌───────────────────────▼─────────────────────────────────┐
│                  Azure Virtual Network                   │
│  ┌──────────────────────────────────────────────────┐  │
│  │          App Service Subnet (10.0.2.0/24)        │  │
│  │  ┌──────────────────┐    ┌──────────────────┐   │  │
│  │  │  Backend App     │    │  Frontend App    │   │  │
│  │  │  (with staging)  │    │  (with staging)  │   │  │
│  │  └──────────────────┘    └──────────────────┘   │  │
│  └──────────────────────────────────────────────────┘  │
│                                                          │
│  ┌──────────────────────────────────────────────────┐  │
│  │    Private Endpoint Subnet (10.0.1.0/24)         │  │
│  │  ┌──────────────┐  ┌──────────────┐              │  │
│  │  │ PostgreSQL   │  │ Blob Storage │              │  │
│  │  │   Private    │  │   Private    │              │  │
│  │  │   Endpoint   │  │   Endpoint   │              │  │
│  │  └──────────────┘  └──────────────┘              │  │
│  └──────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────┘
```

## Security Features

### Network Security

- **Private Endpoints**: All PaaS services are accessible only through private endpoints
- **VNet Integration**: App Services are integrated with VNet
- **No Public Access**: Database and storage are not accessible from internet
- **VNet Peering**: Connected to corporate network for user access

### Identity and Access

- **Managed Identity**: App Services use managed identity to access Azure resources
- **No Secrets in Code**: Credentials stored in Azure Key Vault
- **RBAC**: Role-based access control for all resources
- **Azure AD Integration**: User authentication through Azure AD

### Data Protection

- **TLS 1.2+**: All connections use TLS 1.2 or higher
- **Encrypted at Rest**: All data encrypted at rest
- **Encrypted in Transit**: All data encrypted in transit
- **Automated Backups**: PostgreSQL automated backups with geo-redundancy (production)

## Resource Naming Convention

Resources follow this naming pattern:

- Resource Groups: `rg-mycrm-{environment}`
- App Service Plans: `asp-mycrm-{environment}`
- App Services: `app-mycrm-{service}-{environment}`
- PostgreSQL: `psql-mycrm-{environment}`
- Storage: `stmycrm{environment}` (no hyphens)
- VNets: `vnet-mycrm-{environment}`
- Private Endpoints: `pe-{resource-name}`

## Cost Optimization

### Staging Environment

- App Service Plan: P1v3 (1 instance)
- PostgreSQL: Standard_D2s_v3 (General Purpose)
- Storage: Standard_LRS (Locally Redundant)
- High Availability: Disabled

**Estimated Monthly Cost**: ~$300-400

### Production Environment

- App Service Plan: P1v3 (can scale to multiple instances)
- PostgreSQL: Standard_D2s_v3+ (with HA)
- Storage: Standard_GRS (Geo-Redundant)
- High Availability: Enabled (Zone Redundant)

**Estimated Monthly Cost**: ~$600-800

### Cost Saving Tips

1. **Auto-scaling**: Configure auto-scale rules based on metrics
2. **Reserved Instances**: Purchase reserved capacity for production
3. **Dev/Test Pricing**: Use dev/test subscription for non-production
4. **Scheduled Scaling**: Scale down during off-hours if applicable

## Monitoring

### Application Insights

- Automatically configured for all App Services
- Collects telemetry, logs, and metrics
- OpenTelemetry endpoint configured

### Azure Monitor

- Alerts configured for resource health
- Metrics for performance monitoring
- Log Analytics for centralized logging

### Health Checks

- App Services: `/health` endpoint
- Auto-healing rules configured
- Health check-based deployment validation

## Maintenance

### Updates

Infrastructure updates should be:
1. Tested in staging first
2. Reviewed by team
3. Deployed during maintenance windows
4. Documented in change log

### Backup and Restore

#### PostgreSQL Backups

- Automated daily backups (7-day retention)
- Point-in-time restore capability
- Geo-redundant backups (production)

```bash
# Restore from backup
az postgres flexible-server restore \
  --source-server psql-mycrm-production \
  --name psql-mycrm-production-restored \
  --restore-time "2024-01-20T10:00:00Z" \
  --resource-group rg-mycrm-production
```

#### Storage Backups

- Soft delete enabled (7-day retention)
- Versioning enabled
- Automated blob snapshots

### Scaling

#### Vertical Scaling (Scale Up)

```bash
# Scale up App Service Plan
az appservice plan update \
  --name asp-mycrm-production \
  --resource-group rg-mycrm-production \
  --sku P2v3

# Scale up PostgreSQL
az postgres flexible-server update \
  --name psql-mycrm-production \
  --resource-group rg-mycrm-production \
  --sku-name Standard_D4s_v3
```

#### Horizontal Scaling (Scale Out)

```bash
# Scale out App Service (add instances)
az appservice plan update \
  --name asp-mycrm-production \
  --resource-group rg-mycrm-production \
  --number-of-workers 3
```

## Disaster Recovery

### Recovery Time Objective (RTO)

- **Staging**: 4 hours
- **Production**: 1 hour

### Recovery Point Objective (RPO)

- **Database**: 5 minutes (point-in-time restore)
- **File Storage**: 15 minutes (replication)

### DR Procedures

1. **Regional Outage**: Failover to geo-replicated resources
2. **Database Failure**: Restore from automated backup
3. **Application Failure**: Deploy previous version or rollback

See [DEPLOYMENT.md](../DEPLOYMENT.md) for detailed recovery procedures.

## Compliance

### Data Residency

- All data stored in East US region
- Geo-replication to West US (production)
- Complies with data residency requirements

### Audit Logging

- All resource changes logged in Activity Log
- 90-day retention
- Exported to Log Analytics for long-term retention

### Access Control

- RBAC enforced on all resources
- Least privilege principle
- Regular access reviews

## Troubleshooting

### Common Issues

#### Deployment Fails

```bash
# Check deployment status
az deployment group show \
  --resource-group rg-mycrm-staging \
  --name main

# View deployment operations
az deployment operation group list \
  --resource-group rg-mycrm-staging \
  --name main
```

#### Private Endpoint Connection Issues

```bash
# Check private endpoint status
az network private-endpoint show \
  --name pe-psql-mycrm-production \
  --resource-group rg-mycrm-production

# List private endpoint connections
az network private-endpoint-connection list \
  --name psql-mycrm-production \
  --resource-group rg-mycrm-production \
  --type Microsoft.DBforPostgreSQL/flexibleServers
```

#### VNet Integration Issues

```bash
# Check VNet integration
az webapp vnet-integration list \
  --name app-mycrm-backend-prod \
  --resource-group rg-mycrm-production

# Remove and re-add VNet integration if needed
az webapp vnet-integration remove \
  --name app-mycrm-backend-prod \
  --resource-group rg-mycrm-production

az webapp vnet-integration add \
  --name app-mycrm-backend-prod \
  --resource-group rg-mycrm-production \
  --vnet vnet-mycrm-production \
  --subnet snet-app-services
```

## Support

For infrastructure issues:
1. Check this documentation
2. Review [DEPLOYMENT.md](../DEPLOYMENT.md)
3. Check [RUNBOOKS/ALERTS-AND-ESCALATION.md](../RUNBOOKS/ALERTS-AND-ESCALATION.md)
4. Contact DevOps team
5. Escalate to Azure support if needed

## Related Documentation

- [Deployment Guide](../DEPLOYMENT.md)
- [Alerts and Escalation](../RUNBOOKS/ALERTS-AND-ESCALATION.md)
- [Azure Bicep Documentation](https://docs.microsoft.com/en-us/azure/azure-resource-manager/bicep/)
