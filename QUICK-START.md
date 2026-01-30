# Quick Start Guide - Azure CI/CD Deployment

This guide will help you quickly set up and deploy the MyCRM application to Azure using the implemented CI/CD pipeline.

## Prerequisites

- [ ] Azure subscription with Owner or Contributor access
- [ ] GitHub repository admin access
- [ ] Azure CLI installed (`az --version`)
- [ ] GitHub CLI installed (`gh --version`)
- [ ] Basic understanding of Azure and GitHub Actions

## Step 1: Initial Azure Setup (5-10 minutes)

### Option A: Automated Setup (Recommended)

Run the automated setup script:

```bash
cd infrastructure
chmod +x setup-azure.sh
./setup-azure.sh
```

Follow the prompts:
1. Enter your Azure subscription ID
2. Enter GitHub repository (e.g., `TomizawaAtLogit/MyCRM`)
3. Choose environment (`staging` or `production`)
4. Enter Azure region (default: `eastus`)
5. Confirm and proceed

The script will:
- Create resource group
- Create Application Insights
- Generate secure PostgreSQL password
- Create Service Principal for GitHub Actions
- Configure all GitHub secrets

### Option B: Manual Setup

Follow the detailed instructions in `.github/SECRETS.md` to:
1. Create Azure Service Principal
2. Configure GitHub secrets
3. Set up Application Insights

## Step 2: Configure GitHub Environments (5 minutes)

1. Go to your GitHub repository Settings
2. Click "Environments"
3. Create these environments with protection rules:

   **staging**:
   - No required reviewers
   
   **staging-infrastructure**:
   - Required reviewers: 1 from DevOps team
   
   **production**:
   - Required reviewers: 2 from team
   - Wait timer: 5 minutes
   
   **production-infrastructure**:
   - Required reviewers: DevOps lead + Infrastructure team
   
   **production-migration-approval**:
   - Required reviewers: Database admin + DevOps lead
   
   **staging-rollback-approval** & **production-rollback-approval**:
   - Required reviewers: 1-2 from team

## Step 3: Deploy Infrastructure (10-15 minutes)

### For Staging:

1. Go to GitHub Actions
2. Select "Deploy Infrastructure" workflow
3. Click "Run workflow"
4. Choose environment: `staging`
5. Click "Run workflow"
6. Approve when prompted
7. Wait for completion (~10 minutes)

### For Production:

Repeat the above steps, choosing `production` environment.

## Step 4: Configure Network Access (15-30 minutes)

After infrastructure deployment, configure VNet peering:

```bash
# Get the VNet resource ID from the output
MYCRM_VNET=$(az network vnet show \
  --name vnet-mycrm-production \
  --resource-group rg-mycrm-production \
  --query id -o tsv)

# Create peering from your corporate VNet to MyCRM VNet
az network vnet peering create \
  --name corp-to-mycrm \
  --resource-group YOUR_CORP_RG \
  --vnet-name YOUR_CORP_VNET \
  --remote-vnet $MYCRM_VNET \
  --allow-vnet-access

# Create reverse peering
az network vnet peering create \
  --name mycrm-to-corp \
  --resource-group rg-mycrm-production \
  --vnet-name vnet-mycrm-production \
  --remote-vnet /subscriptions/{sub}/resourceGroups/YOUR_CORP_RG/providers/Microsoft.Network/virtualNetworks/YOUR_CORP_VNET \
  --allow-vnet-access
```

## Step 5: Set Database Connection Strings (2 minutes)

After infrastructure deployment, get the PostgreSQL connection details:

```bash
# For Staging
STAGING_CONNECTION="Host=psql-mycrm-staging.postgres.database.azure.com;Port=5432;Database=aspire_db;Username=pgadmin;Password=YOUR_PASSWORD;SSL Mode=Require;"

gh secret set STAGING_DB_CONNECTION_STRING \
  --body "$STAGING_CONNECTION" \
  --repo TomizawaAtLogit/MyCRM

# For Production  
PRODUCTION_CONNECTION="Host=psql-mycrm-production.postgres.database.azure.com;Port=5432;Database=aspire_db;Username=pgadmin;Password=YOUR_PASSWORD;SSL Mode=Require;"

gh secret set PRODUCTION_DB_CONNECTION_STRING \
  --body "$PRODUCTION_CONNECTION" \
  --repo TomizawaAtLogit/MyCRM
```

## Step 6: Initial Database Migration (5 minutes)

Run initial database setup:

```bash
# Connect to Azure via VPN/corporate network

# For Staging
dotnet ef database update \
  --project AspireApp1.DbApi/AspireApp1.BackEnd.csproj \
  --startup-project AspireApp1.DbApi/AspireApp1.BackEnd.csproj \
  --connection "$STAGING_CONNECTION"

# For Production (after staging is verified)
dotnet ef database update \
  --project AspireApp1.DbApi/AspireApp1.BackEnd.csproj \
  --startup-project AspireApp1.DbApi/AspireApp1.BackEnd.csproj \
  --connection "$PRODUCTION_CONNECTION"
```

## Step 7: First Deployment (5-10 minutes)

### Deploy to Staging:

1. Go to GitHub Actions
2. Select "Deploy to Staging" workflow
3. Click "Run workflow"
4. Select "Run database migrations": `false` (already done manually)
5. Click "Run workflow"
6. Wait for completion (~5-7 minutes)
7. Verify deployment:
   ```bash
   curl https://app-mycrm-backend-staging.azurewebsites.net/health
   curl https://app-mycrm-frontend-staging.azurewebsites.net/health
   ```

### Deploy to Production:

1. Go to GitHub Actions
2. Select "Deploy to Production" workflow
3. Click "Run workflow"
4. Select "Run database migrations": `false`
5. Select "Migration requires approval": `false`
6. Click "Run workflow"
7. Approve when prompted
8. Wait for completion (~8-10 minutes)
9. Verify deployment:
   ```bash
   curl https://app-mycrm-backend-prod.azurewebsites.net/health
   curl https://app-mycrm-frontend-prod.azurewebsites.net/health
   ```

## Step 8: Configure Monitoring (10 minutes)

Set up alerts in Azure Portal:

1. Navigate to Application Insights
2. Go to Alerts
3. Create alert rules as documented in `RUNBOOKS/ALERTS-AND-ESCALATION.md`
4. Configure action groups for notifications

## Verification Checklist

After setup, verify:

- [ ] All GitHub secrets are configured
- [ ] GitHub environments have correct protection rules
- [ ] Azure infrastructure is deployed (both staging and production)
- [ ] VNet peering is configured and working
- [ ] Database migrations completed successfully
- [ ] Applications are deployed and healthy
- [ ] Health check endpoints return 200 OK
- [ ] Application Insights is receiving telemetry
- [ ] Alerts are configured and tested
- [ ] You can access applications via corporate VPN/network

## Common Issues

### "Cannot connect to database"
- Ensure you're connected via corporate VPN
- Verify private endpoint is configured
- Check connection string is correct

### "Deployment timeout"
- Check App Service logs in Azure Portal
- Verify App Service Plan has enough resources
- Check if previous deployment is still running

### "Health check failed"
- Check application logs for startup errors
- Verify all environment variables are set
- Check database connectivity

## Next Steps

- Read the full documentation in `DEPLOYMENT.md`
- Review the operations runbook in `RUNBOOKS/ALERTS-AND-ESCALATION.md`
- Set up automated backups and disaster recovery testing
- Configure custom domain names (if needed)
- Set up SSL certificates (if not using default .azurewebsites.net)

## Getting Help

1. Check `DEPLOYMENT.md` for detailed procedures
2. Review `RUNBOOKS/ALERTS-AND-ESCALATION.md` for troubleshooting
3. Check `KNOWN-ISSUES.md` for known problems
4. Contact DevOps team for assistance

## Useful Commands

```bash
# Check deployment status
az webapp show --name app-mycrm-backend-prod --resource-group rg-mycrm-production

# View application logs
az webapp log tail --name app-mycrm-backend-prod --resource-group rg-mycrm-production

# Restart application
az webapp restart --name app-mycrm-backend-prod --resource-group rg-mycrm-production

# Check database status
az postgres flexible-server show --name psql-mycrm-production --resource-group rg-mycrm-production

# List GitHub secrets (names only)
gh secret list --repo TomizawaAtLogit/MyCRM
```

## Estimated Total Time

- **Initial setup**: 30-45 minutes
- **First staging deployment**: 10 minutes
- **First production deployment**: 15 minutes
- **Monitoring setup**: 10 minutes
- **Total**: ~1-1.5 hours

## Success Criteria

You've successfully completed the setup when:

✅ Applications are accessible via corporate network  
✅ Health checks return 200 OK  
✅ Database connectivity works  
✅ Application Insights shows telemetry  
✅ CI/CD pipelines run successfully  
✅ Rollback procedure tested  

---

**Congratulations!** Your Azure CI/CD pipeline is now ready for use. 🎉

For ongoing operations, refer to `DEPLOYMENT.md` and `RUNBOOKS/ALERTS-AND-ESCALATION.md`.
