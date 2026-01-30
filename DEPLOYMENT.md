# Deployment Guide for MyCRM Application

This document describes the deployment process for the MyCRM application to Azure using GitHub Actions.

## Table of Contents

1. [Architecture Overview](#architecture-overview)
2. [Prerequisites](#prerequisites)
3. [Initial Setup](#initial-setup)
4. [Deployment Workflows](#deployment-workflows)
5. [Database Migrations](#database-migrations)
6. [Rollback Procedures](#rollback-procedures)
7. [Monitoring and Alerts](#monitoring-and-alerts)
8. [Troubleshooting](#troubleshooting)

## Architecture Overview

### Infrastructure Components

- **Azure App Services**: Hosts both frontend and backend applications
  - Backend API (Ligot.BackEnd)
  - Frontend Web App (Ligot.FrontEnd)
- **Azure PostgreSQL Flexible Server**: Database with automated backups
- **Azure Blob Storage**: File storage for uploads with private endpoints
- **Application Insights**: Monitoring and telemetry
- **Virtual Network**: Private network with intranet-only access
- **Managed Identity**: Secure authentication between services

### Deployment Strategy

- **Staging Slot Swap**: Zero-downtime deployments
- **GitOps**: All deployments triggered via GitHub Actions
- **Manual Approvals**: Required for production and breaking schema changes
- **Private Network**: Applications accessible only via corporate VPN/intranet

## Prerequisites

### Azure Resources

1. Azure subscription with appropriate permissions
2. Resource groups:
   - `rg-mycrm-staging`
   - `rg-mycrm-production`
3. Service Principal with Contributor role

### GitHub Secrets

Configure the following secrets in your GitHub repository:

#### Required for All Environments

- `AZURE_CREDENTIALS`: Azure service principal credentials (JSON)
  ```json
  {
    "clientId": "<client-id>",
    "clientSecret": "<client-secret>",
    "subscriptionId": "<subscription-id>",
    "tenantId": "<tenant-id>"
  }
  ```
- `AZURE_SUBSCRIPTION_ID`: Your Azure subscription ID
- `POSTGRES_ADMIN_PASSWORD`: PostgreSQL administrator password
- `APPLICATIONINSIGHTS_CONNECTION_STRING`: Application Insights connection string

#### Environment-Specific

- `STAGING_DB_CONNECTION_STRING`: Staging database connection string
- `PRODUCTION_DB_CONNECTION_STRING`: Production database connection string

### Local Development Setup

1. Install .NET 10 SDK
2. Install Docker for local PostgreSQL
3. Clone the repository
4. Run `docker-compose -f docker-compose.postgres.yml up -d`
5. Run database migrations: `dotnet ef database update --project Ligot.DbApi`
6. Start the application: `dotnet run --project Ligot.AppHost`

## Initial Setup

### 1. Deploy Infrastructure

Deploy infrastructure for staging first, then production:

```bash
# Navigate to GitHub Actions > Deploy Infrastructure
# Select environment: staging
# Click "Run workflow"
```

This will create:
- App Service Plans with staging slots
- PostgreSQL Flexible Server with private endpoints
- Storage Account for file uploads
- Virtual Network for private access
- Managed Identities for secure authentication

### 2. Configure Network Peering

After infrastructure deployment, configure VNet peering with your corporate network:

```bash
az network vnet peering create \
  --name mycrm-to-corporate \
  --resource-group rg-mycrm-production \
  --vnet-name vnet-mycrm-production \
  --remote-vnet /subscriptions/{sub-id}/resourceGroups/{corp-rg}/providers/Microsoft.Network/virtualNetworks/{corp-vnet} \
  --allow-vnet-access
```

### 3. Configure Azure AD Authentication

1. Register application in Azure AD
2. Configure authentication in Azure App Services
3. Update application settings with AD configuration

### 4. Initial Database Setup

Run initial migrations manually for the first deployment:

```bash
# From your local development environment with VPN connected
dotnet ef database update \
  --project Ligot.DbApi/Ligot.BackEnd.csproj \
  --startup-project Ligot.DbApi/Ligot.BackEnd.csproj \
  --connection "Host=psql-mycrm-production.postgres.database.azure.com;..."
```

## Deployment Workflows

### CI - Build and Test

**Workflow**: `.github/workflows/ci-build.yml`

**Triggers**: Push to `main` or `develop` branches, Pull Requests

**Purpose**: Validates code changes, runs tests, creates deployment artifacts

**Steps**:
1. Checkout code
2. Setup .NET 10
3. Restore dependencies
4. Build solution
5. Run tests
6. Publish artifacts

### Deploy to Staging

**Workflow**: `.github/workflows/deploy-staging.yml`

**Trigger**: Manual (workflow_dispatch)

**Purpose**: Deploy to staging environment with automatic slot swap

**Steps**:
1. Build application
2. Run database migrations (optional)
3. Deploy to staging slot
4. Swap staging to production slot
5. Health checks

**Usage**:
```bash
# Navigate to GitHub Actions > Deploy to Staging
# Select "Run database migrations": true/false
# Click "Run workflow"
```

### Deploy to Production

**Workflow**: `.github/workflows/deploy-production.yml`

**Trigger**: Manual (workflow_dispatch)

**Purpose**: Deploy to production with manual approval

**Steps**:
1. Build and test application
2. Request migration approval (if needed)
3. Wait for manual approval
4. Run database migrations (if approved)
5. Deploy to staging slot
6. Staging health check
7. Swap staging to production
8. Production health check

**Usage**:
```bash
# Navigate to GitHub Actions > Deploy to Production
# Select "Run database migrations": true/false
# Select "Migration requires approval": true/false
# Click "Run workflow"
# Approve migration if prompted
# Approve production deployment
```

## Database Migrations

### Non-Breaking Changes (Automated)

Non-breaking changes can be deployed automatically:
- Adding new tables
- Adding nullable columns
- Adding indexes
- Adding new stored procedures

**Process**:
1. Create migration locally: `dotnet ef migrations add MigrationName --project Ligot.DbApi`
2. Commit and push to repository
3. Deploy with `run_migrations: true`

### Breaking Changes (Manual Approval)

Breaking changes require manual approval:
- Dropping tables or columns
- Renaming columns
- Changing column types
- Removing constraints

**Process**:
1. Create migration locally
2. Test thoroughly in development
3. Deploy to staging with `run_migrations: true`
4. Validate in staging
5. Deploy to production with `run_migrations: true` and `migration_approval: true`
6. Approve migration when prompted
7. Approve production deployment

### Migration Testing

Before deploying migrations to production:

1. **Test locally**: Run migration against local database
2. **Review SQL**: Check generated SQL for potential issues
   ```bash
   dotnet ef migrations script --project Ligot.DbApi
   ```
3. **Test in staging**: Deploy to staging first
4. **Backup production**: Verify automated backups are current
5. **Plan rollback**: Prepare rollback migration if needed

## Rollback Procedures

### Application Rollback (Slot Swap Reversal)

**Workflow**: `.github/workflows/rollback.yml`

**When to Use**:
- Application bugs discovered in production
- Performance issues
- Integration failures

**Steps**:
```bash
# Navigate to GitHub Actions > Rollback Deployment
# Select environment: staging or production
# Enter reason for rollback
# Approve rollback
```

This will:
1. Swap production and staging slots (reverting to previous version)
2. Run health checks
3. Log rollback information

**Timeline**: ~2-5 minutes

### Database Rollback

**Important**: Database rollbacks are more complex and should be avoided by:
- Thorough testing in staging
- Using additive migrations where possible
- Creating rollback migrations in advance

**If database rollback is needed**:

1. **For non-destructive migrations**: Apply reverse migration
   ```bash
   dotnet ef database update PreviousMigrationName --project Ligot.DbApi
   ```

2. **For destructive migrations**: Restore from backup
   ```bash
   # Using Azure Portal or CLI
   az postgres flexible-server restore \
     --source-server psql-mycrm-production \
     --restore-time "2024-01-20T10:00:00Z" \
     --name psql-mycrm-production-restored
   ```

3. **Update connection strings** to point to restored database

### Rollback Decision Matrix

| Issue Type | Rollback Method | Approval Required | Timeline |
|------------|----------------|-------------------|----------|
| UI Bug | Slot Swap | Yes | 2-5 min |
| API Error | Slot Swap | Yes | 2-5 min |
| Performance | Slot Swap | Yes | 2-5 min |
| Data Corruption | Database Restore | Yes | 30-60 min |
| Schema Issue | Reverse Migration | Yes | 10-30 min |

## Monitoring and Alerts

### Application Insights

Access via Azure Portal:
- Navigate to Application Insights resource
- View live metrics, traces, and exceptions
- Set up custom alerts

### Key Metrics to Monitor

1. **Availability**: Health check endpoints
2. **Response Time**: P50, P95, P99
3. **Error Rate**: 5xx responses
4. **Database Performance**: Query duration
5. **Storage Operations**: Blob read/write latency

### Health Check Endpoints

- Backend: `https://app-mycrm-backend-prod.azurewebsites.net/health`
- Frontend: `https://app-mycrm-frontend-prod.azurewebsites.net/health`

### Setting Up Alerts

See [RUNBOOKS/ALERTS-AND-ESCALATION.md](RUNBOOKS/ALERTS-AND-ESCALATION.md) for detailed alert configuration.

## Troubleshooting

### Deployment Failures

#### Build Failures

```bash
# Check build logs in GitHub Actions
# Common issues:
# - Missing dependencies
# - Compilation errors
# - Test failures

# Solution: Fix issues locally and push again
dotnet build Ligot.sln
dotnet test Ligot.sln
```

#### Deployment Timeout

```bash
# If deployment times out:
# 1. Check App Service logs
az webapp log tail --name app-mycrm-backend-prod --resource-group rg-mycrm-production

# 2. Restart the application
az webapp restart --name app-mycrm-backend-prod --resource-group rg-mycrm-production
```

#### Health Check Failures

```bash
# Check application logs
az webapp log tail --name app-mycrm-backend-prod --resource-group rg-mycrm-production

# Common issues:
# - Database connection failures
# - Missing configuration
# - File storage access issues

# Verify connection strings
az webapp config connection-string list \
  --name app-mycrm-backend-prod \
  --resource-group rg-mycrm-production
```

### Database Issues

#### Connection Failures

```bash
# Check PostgreSQL server status
az postgres flexible-server show \
  --name psql-mycrm-production \
  --resource-group rg-mycrm-production

# Verify private endpoint connectivity
az network private-endpoint show \
  --name pe-psql-mycrm-production \
  --resource-group rg-mycrm-production

# Check firewall rules (should be private only)
az postgres flexible-server firewall-rule list \
  --name psql-mycrm-production \
  --resource-group rg-mycrm-production
```

#### Migration Failures

```bash
# Check migration status
dotnet ef migrations list --project Ligot.DbApi

# Roll back to previous migration
dotnet ef database update PreviousMigrationName --project Ligot.DbApi

# Create rollback migration
dotnet ef migrations add RollbackMigrationName --project Ligot.DbApi
```

### Network Access Issues

#### Cannot Access Application

1. **Verify VPN connection**: Ensure connected to corporate network
2. **Check VNet peering**: Verify peering is active
3. **Verify private endpoints**: Check private endpoint configuration
4. **Check App Service VNet integration**: Ensure enabled

```bash
# Check VNet peering status
az network vnet peering list \
  --resource-group rg-mycrm-production \
  --vnet-name vnet-mycrm-production

# Verify App Service VNet integration
az webapp vnet-integration list \
  --name app-mycrm-backend-prod \
  --resource-group rg-mycrm-production
```

### Storage Issues

#### File Upload Failures

```bash
# Check storage account private endpoint
az network private-endpoint show \
  --name pe-st-mycrm-production \
  --resource-group rg-mycrm-production

# Verify Managed Identity has Storage Blob Data Contributor role
az role assignment list \
  --assignee <managed-identity-object-id> \
  --scope /subscriptions/{sub-id}/resourceGroups/rg-mycrm-production/providers/Microsoft.Storage/storageAccounts/stmycrmproduction
```

## Best Practices

1. **Always deploy to staging first**: Test thoroughly before production
2. **Use manual approvals for production**: Adds safety checkpoint
3. **Monitor after deployment**: Watch metrics for 30 minutes post-deployment
4. **Keep staging and production in sync**: Deploy regularly to staging
5. **Test rollback procedures**: Practice rollback in staging
6. **Document changes**: Update documentation with each deployment
7. **Backup before migrations**: Verify backups are current
8. **Use feature flags**: For gradual rollout of new features
9. **Security scans**: Run security scans before production deployment
10. **Communication**: Notify team before production deployments

## Support

For deployment issues:
1. Check this documentation
2. Review [RUNBOOKS/ALERTS-AND-ESCALATION.md](RUNBOOKS/ALERTS-AND-ESCALATION.md)
3. Contact DevOps team
4. Escalate to infrastructure team if needed

## Related Documentation

- [RUNBOOKS/ALERTS-AND-ESCALATION.md](RUNBOOKS/ALERTS-AND-ESCALATION.md) - Alert handling procedures
- [Infrastructure as Code](infrastructure/main.bicep) - Bicep template
- [GitHub Actions Workflows](.github/workflows/) - All workflow definitions

