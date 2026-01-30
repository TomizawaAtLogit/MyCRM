#!/bin/bash

# MyCRM Azure Infrastructure Setup Script
# This script helps set up the Azure infrastructure and GitHub secrets for CI/CD

set -e

echo "🚀 MyCRM Azure Infrastructure Setup"
echo "===================================="
echo ""

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Check prerequisites
echo "📋 Checking prerequisites..."

if ! command -v az &> /dev/null; then
    echo -e "${RED}❌ Azure CLI not found. Please install: https://docs.microsoft.com/en-us/cli/azure/install-azure-cli${NC}"
    exit 1
fi

if ! command -v gh &> /dev/null; then
    echo -e "${RED}❌ GitHub CLI not found. Please install: https://cli.github.com/${NC}"
    exit 1
fi

echo -e "${GREEN}✅ Prerequisites check passed${NC}"
echo ""

# Configuration
read -p "Enter your Azure subscription ID: " SUBSCRIPTION_ID
read -p "Enter GitHub repository (owner/repo): " GITHUB_REPO
read -p "Enter environment (staging/production): " ENVIRONMENT
read -p "Enter Azure region (default: eastus): " AZURE_REGION
AZURE_REGION=${AZURE_REGION:-eastus}

RESOURCE_GROUP="rg-mycrm-${ENVIRONMENT}"
APP_INSIGHTS_NAME="mycrm-appinsights-${ENVIRONMENT}"

echo ""
echo "Configuration:"
echo "  Subscription: ${SUBSCRIPTION_ID}"
echo "  Repository: ${GITHUB_REPO}"
echo "  Environment: ${ENVIRONMENT}"
echo "  Resource Group: ${RESOURCE_GROUP}"
echo "  Region: ${AZURE_REGION}"
echo ""

read -p "Continue with this configuration? (y/n): " CONFIRM
if [ "$CONFIRM" != "y" ]; then
    echo "Aborted."
    exit 0
fi

# Login to Azure
echo ""
echo "🔐 Logging into Azure..."
az login
az account set --subscription "${SUBSCRIPTION_ID}"

# Create resource group
echo ""
echo "📦 Creating resource group: ${RESOURCE_GROUP}..."
az group create \
    --name "${RESOURCE_GROUP}" \
    --location "${AZURE_REGION}" \
    --tags "Environment=${ENVIRONMENT}" "Project=MyCRM"

echo -e "${GREEN}✅ Resource group created${NC}"

# Create Application Insights
echo ""
echo "📊 Creating Application Insights: ${APP_INSIGHTS_NAME}..."
az monitor app-insights component create \
    --app "${APP_INSIGHTS_NAME}" \
    --location "${AZURE_REGION}" \
    --resource-group "${RESOURCE_GROUP}" \
    --tags "Environment=${ENVIRONMENT}" "Project=MyCRM"

APPINSIGHTS_CONNECTION_STRING=$(az monitor app-insights component show \
    --app "${APP_INSIGHTS_NAME}" \
    --resource-group "${RESOURCE_GROUP}" \
    --query connectionString \
    --output tsv)

echo -e "${GREEN}✅ Application Insights created${NC}"

# Create Service Principal for GitHub Actions
echo ""
echo "🔑 Creating Service Principal for GitHub Actions..."
SP_NAME="github-actions-mycrm-${ENVIRONMENT}"

SP_OUTPUT=$(az ad sp create-for-rbac \
    --name "${SP_NAME}" \
    --role contributor \
    --scopes "/subscriptions/${SUBSCRIPTION_ID}/resourceGroups/${RESOURCE_GROUP}" \
    --sdk-auth)

echo -e "${GREEN}✅ Service Principal created${NC}"

# Generate PostgreSQL password
echo ""
echo "🔐 Generating PostgreSQL admin password..."
POSTGRES_PASSWORD=$(openssl rand -base64 24 | tr -d "=+/" | cut -c1-20)
echo -e "${GREEN}✅ PostgreSQL password generated${NC}"

# Set GitHub secrets
echo ""
echo "🔧 Configuring GitHub secrets..."

# Login to GitHub
gh auth login

# Set secrets
echo "Setting AZURE_CREDENTIALS..."
echo "${SP_OUTPUT}" | gh secret set AZURE_CREDENTIALS --repo "${GITHUB_REPO}"

echo "Setting AZURE_SUBSCRIPTION_ID..."
echo "${SUBSCRIPTION_ID}" | gh secret set AZURE_SUBSCRIPTION_ID --repo "${GITHUB_REPO}"

echo "Setting POSTGRES_ADMIN_PASSWORD..."
echo "${POSTGRES_PASSWORD}" | gh secret set POSTGRES_ADMIN_PASSWORD --repo "${GITHUB_REPO}"

echo "Setting APPLICATIONINSIGHTS_CONNECTION_STRING..."
echo "${APPINSIGHTS_CONNECTION_STRING}" | gh secret set APPLICATIONINSIGHTS_CONNECTION_STRING --repo "${GITHUB_REPO}"

echo -e "${GREEN}✅ GitHub secrets configured${NC}"

# Deploy infrastructure
echo ""
echo "🏗️  Ready to deploy infrastructure!"
echo ""
echo "Next steps:"
echo "1. Review the configuration in infrastructure/main.bicep"
echo "2. Run the 'Deploy Infrastructure' GitHub Action workflow"
echo "3. After infrastructure is deployed, configure database connection strings:"
echo ""
echo "   Staging: ${RESOURCE_GROUP}"
echo "   Database server will be: psql-mycrm-${ENVIRONMENT}.postgres.database.azure.com"
echo ""
echo "4. Set the database connection string secrets:"
echo ""
if [ "${ENVIRONMENT}" == "staging" ]; then
    echo "   gh secret set STAGING_DB_CONNECTION_STRING --repo ${GITHUB_REPO}"
else
    echo "   gh secret set PRODUCTION_DB_CONNECTION_STRING --repo ${GITHUB_REPO}"
fi
echo ""
echo "   Format: Host=psql-mycrm-${ENVIRONMENT}.postgres.database.azure.com;Port=5432;Database=aspire_db;Username=pgadmin;Password=${POSTGRES_PASSWORD};SSL Mode=Require;"
echo ""
echo -e "${GREEN}✅ Setup complete!${NC}"
echo ""
echo "⚠️  Important: Save the following information securely:"
echo ""
echo "PostgreSQL Admin Password: ${POSTGRES_PASSWORD}"
echo "Application Insights Connection String: ${APPINSIGHTS_CONNECTION_STRING}"
echo ""
echo "Service Principal Details:"
echo "${SP_OUTPUT}"
echo ""
