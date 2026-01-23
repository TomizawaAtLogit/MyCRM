# GitHub Secrets Configuration

This document lists all the secrets that need to be configured in GitHub for the CI/CD pipelines to work.

## Required Secrets

### Azure Credentials

#### `AZURE_CREDENTIALS`

Azure Service Principal credentials in JSON format.

**How to create**:

```bash
# Create service principal
az ad sp create-for-rbac \
  --name "github-actions-mycrm" \
  --role contributor \
  --scopes /subscriptions/{subscription-id} \
  --sdk-auth

# Output will be in this format:
{
  "clientId": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
  "clientSecret": "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
  "subscriptionId": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
  "tenantId": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx"
}
```

**Where to add**: Repository Settings > Secrets and variables > Actions > New repository secret

#### `AZURE_SUBSCRIPTION_ID`

Your Azure subscription ID.

**Value**: `xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx`

### Database Credentials

#### `POSTGRES_ADMIN_PASSWORD`

PostgreSQL administrator password (used for infrastructure deployment).

**Requirements**:
- At least 8 characters
- Contains uppercase, lowercase, numbers, and special characters
- Does not contain username

**Example**: `MyS3cur3P@ssw0rd!`

#### `STAGING_DB_CONNECTION_STRING`

Full connection string for staging database.

**Format**:
```
Host=psql-mycrm-staging.postgres.database.azure.com;Port=5432;Database=aspire_db;Username=pgadmin;Password=YourPassword;SSL Mode=Require;
```

#### `PRODUCTION_DB_CONNECTION_STRING`

Full connection string for production database.

**Format**:
```
Host=psql-mycrm-production.postgres.database.azure.com;Port=5432;Database=aspire_db;Username=pgadmin;Password=YourPassword;SSL Mode=Require;
```

### Application Insights

#### `APPLICATIONINSIGHTS_CONNECTION_STRING`

Application Insights connection string for monitoring.

**How to get**:

```bash
# Create Application Insights (if not exists)
az monitor app-insights component create \
  --app mycrm-appinsights \
  --location eastus \
  --resource-group rg-mycrm-common

# Get connection string
az monitor app-insights component show \
  --app mycrm-appinsights \
  --resource-group rg-mycrm-common \
  --query connectionString \
  --output tsv
```

**Format**: `InstrumentationKey=xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx;IngestionEndpoint=https://...`

## Environment-Specific Configuration

### GitHub Environments

Configure these environments in GitHub:

1. **staging**
   - No protection rules (auto-deploy)
   - Required reviewers: None

2. **staging-infrastructure**
   - Required reviewers: DevOps team
   - Deployment branch: main only

3. **production**
   - Required reviewers: 2 from [team members]
   - Deployment branch: main only
   - Wait timer: 5 minutes

4. **production-infrastructure**
   - Required reviewers: DevOps lead + Infrastructure team
   - Deployment branch: main only

5. **production-migration-approval**
   - Required reviewers: Database admin + DevOps lead
   - Used only for database migrations

6. **staging-rollback-approval**
   - Required reviewers: 1 from DevOps team

7. **production-rollback-approval**
   - Required reviewers: 2 from [team members]

### How to Configure Environments

1. Go to Repository Settings
2. Click "Environments"
3. Click "New environment"
4. Enter environment name
5. Configure protection rules:
   - Check "Required reviewers"
   - Add reviewer(s)
   - Optionally add wait timer
   - Optionally restrict deployment branches

## Optional Secrets

### `OTEL_EXPORTER_OTLP_ENDPOINT`

OpenTelemetry collector endpoint (if using external collector).

**Format**: `https://your-collector:4318`

**Note**: Leave empty to use Application Insights only.

## Security Best Practices

### Secret Rotation

Rotate secrets regularly:
- **Azure Service Principal**: Every 90 days
- **Database Passwords**: Every 90 days
- **Storage Keys**: Every 90 days

### Secret Storage

- **Do not** store secrets in code
- **Do not** commit secrets to repository
- **Do not** expose secrets in logs
- **Use** GitHub Secrets for CI/CD
- **Use** Azure Key Vault for application secrets
- **Use** Managed Identity where possible

### Least Privilege

Service Principal permissions:
- **Contributor** role on resource groups only
- **Not** Owner role
- **Not** subscription-wide access (if possible)

### Audit

- Review secret access logs regularly
- Monitor for unauthorized access
- Set up alerts for secret changes

## Validation

After configuring secrets, validate by:

1. **Test Infrastructure Deployment**:
   - Run "Deploy Infrastructure" workflow for staging
   - Verify resources are created successfully

2. **Test Application Deployment**:
   - Run "Deploy to Staging" workflow
   - Verify application is accessible
   - Check health endpoints

3. **Test Rollback**:
   - Run "Rollback Deployment" workflow
   - Verify rollback completes successfully

## Troubleshooting

### "Azure login failed"

- Verify `AZURE_CREDENTIALS` format is correct (JSON)
- Verify Service Principal has not expired
- Verify Service Principal has correct permissions

### "Database connection failed"

- Verify connection string format
- Verify PostgreSQL server is accessible
- Verify private endpoint is configured
- Verify VPN connection (for private networks)

### "Unable to push to blob storage"

- Verify Managed Identity has Storage Blob Data Contributor role
- Verify private endpoint is configured
- Verify App Service has VNet integration

## Secret Management Script

Use this script to set up all secrets at once:

```bash
#!/bin/bash

# GitHub repository (format: owner/repo)
REPO="TomizawaAtLogit/MyCRM"

# Add secrets using GitHub CLI
gh secret set AZURE_CREDENTIALS --body "$(cat azure-credentials.json)" --repo $REPO
gh secret set AZURE_SUBSCRIPTION_ID --body "your-subscription-id" --repo $REPO
gh secret set POSTGRES_ADMIN_PASSWORD --body "your-postgres-password" --repo $REPO
gh secret set STAGING_DB_CONNECTION_STRING --body "your-staging-connection-string" --repo $REPO
gh secret set PRODUCTION_DB_CONNECTION_STRING --body "your-production-connection-string" --repo $REPO
gh secret set APPLICATIONINSIGHTS_CONNECTION_STRING --body "your-appinsights-connection-string" --repo $REPO

echo "✅ All secrets configured successfully"
```

**Usage**:
```bash
# Install GitHub CLI if needed
# https://cli.github.com/

# Authenticate
gh auth login

# Run script
chmod +x setup-secrets.sh
./setup-secrets.sh
```

## Related Documentation

- [DEPLOYMENT.md](DEPLOYMENT.md) - Deployment procedures
- [Infrastructure README](infrastructure/README.md) - Infrastructure details
- [GitHub Actions Documentation](https://docs.github.com/en/actions)
