# Alerts and Escalation Runbook

This document describes the alert configuration, escalation procedures, and response protocols for the MyCRM application.

## Table of Contents

1. [Alert Categories](#alert-categories)
2. [Alert Configuration](#alert-configuration)
3. [Response Procedures](#response-procedures)
4. [Escalation Matrix](#escalation-matrix)
5. [Common Issues](#common-issues)

## Alert Categories

### Critical Alerts (P0)

**Response Time**: Immediate (15 minutes)

- Application completely down (health check failures)
- Database unavailable
- Data corruption detected
- Security breach detected

### High Priority (P1)

**Response Time**: 30 minutes

- High error rate (>5% of requests)
- Significant performance degradation (>2x normal)
- Staging slot swap failed
- Database connection pool exhausted

### Medium Priority (P2)

**Response Time**: 2 hours

- Elevated error rate (1-5% of requests)
- Minor performance degradation
- High memory usage (>80%)
- Disk space warning (>70%)

### Low Priority (P3)

**Response Time**: Next business day

- Informational alerts
- Warning thresholds reached
- Non-critical service degradation

## Alert Configuration

### Application Insights Alerts

Configure these alerts in Azure Application Insights:

#### 1. Application Availability Alert

**Metric**: Availability test results  
**Threshold**: < 95% over 5 minutes  
**Severity**: Critical (P0)

```yaml
Alert Name: MyCRM - Application Unavailable
Condition: Availability < 95%
Time Aggregation: Average
Period: 5 minutes
Frequency: 1 minute
Actions: Notify on-call engineer, DevOps team
```

**Response Procedure**:
1. Check health endpoints: `/health` and `/alive`
2. Review Application Insights live metrics
3. Check App Service status in Azure Portal
4. Review recent deployments
5. Consider rollback if recent deployment
6. Escalate to P0 if not resolved in 15 minutes

#### 2. High Error Rate Alert

**Metric**: Failed request rate  
**Threshold**: > 5% over 10 minutes  
**Severity**: High (P1)

```yaml
Alert Name: MyCRM - High Error Rate
Condition: Failed Requests > 5%
Time Aggregation: Average
Period: 10 minutes
Frequency: 5 minutes
Actions: Notify DevOps team
```

**Response Procedure**:
1. Identify error patterns in Application Insights
2. Check for common error codes (500, 503, etc.)
3. Review application logs
4. Identify affected endpoints
5. Check database performance
6. Implement temporary rate limiting if needed
7. Escalate to P0 if error rate continues to increase

#### 3. Database Connection Failures

**Metric**: Database connection errors  
**Threshold**: > 10 failures in 5 minutes  
**Severity**: Critical (P0)

```yaml
Alert Name: MyCRM - Database Connection Failures
Condition: SQL Exceptions > 10
Time Aggregation: Count
Period: 5 minutes
Frequency: 1 minute
Actions: Notify on-call engineer, Database admin
```

**Response Procedure**:
1. Check PostgreSQL server status
2. Verify private endpoint connectivity
3. Check connection pool settings
4. Review database server metrics (CPU, memory, connections)
5. Check for long-running queries blocking connections
6. Consider scaling up database if resource-constrained
7. Escalate to database admin if infrastructure issue

#### 4. Response Time Degradation

**Metric**: Average response time  
**Threshold**: > 2000ms (P95) over 15 minutes  
**Severity**: Medium (P2)

```yaml
Alert Name: MyCRM - Slow Response Times
Condition: Response Time P95 > 2000ms
Time Aggregation: Percentile (95th)
Period: 15 minutes
Frequency: 5 minutes
Actions: Notify DevOps team
```

**Response Procedure**:
1. Identify slow endpoints using Application Insights
2. Check database query performance
3. Review recent code changes
4. Check for N+1 query problems
5. Verify external service dependencies
6. Consider caching improvements
7. Scale App Service if CPU/Memory high

#### 5. Storage Access Failures

**Metric**: Blob storage operation failures  
**Threshold**: > 5 failures in 10 minutes  
**Severity**: High (P1)

```yaml
Alert Name: MyCRM - Storage Access Issues
Condition: Storage Exceptions > 5
Time Aggregation: Count
Period: 10 minutes
Frequency: 5 minutes
Actions: Notify DevOps team
```

**Response Procedure**:
1. Check storage account status
2. Verify private endpoint connectivity
3. Check Managed Identity permissions
4. Review storage account metrics
5. Verify SAS tokens if used
6. Check for throttling

#### 6. Memory Usage Alert

**Metric**: App Service memory usage  
**Threshold**: > 80% over 10 minutes  
**Severity**: Medium (P2)

```yaml
Alert Name: MyCRM - High Memory Usage
Condition: Memory Percentage > 80%
Time Aggregation: Average
Period: 10 minutes
Frequency: 5 minutes
Actions: Notify DevOps team
```

**Response Procedure**:
1. Check for memory leaks in Application Insights
2. Review recent deployments
3. Analyze memory dump if available
4. Consider scaling up App Service Plan
5. Review caching strategies
6. Check for large object retention

### PostgreSQL Alerts

Configure these alerts in Azure Monitor for PostgreSQL:

#### 1. High CPU Usage

**Metric**: CPU percentage  
**Threshold**: > 80% over 10 minutes  
**Severity**: Medium (P2)

**Response Procedure**:
1. Identify expensive queries using Query Performance Insight
2. Check for missing indexes
3. Review query execution plans
4. Consider query optimization
5. Scale up database tier if needed

#### 2. High Connection Count

**Metric**: Active connections  
**Threshold**: > 80% of max connections  
**Severity**: High (P1)

**Response Procedure**:
1. Check for connection leaks in application
2. Review connection pool settings
3. Identify long-running transactions
4. Kill idle connections if necessary
5. Increase max connections if needed

#### 3. Replication Lag (Production Only)

**Metric**: Replication lag seconds  
**Threshold**: > 30 seconds  
**Severity**: High (P1)

**Response Procedure**:
1. Check replication status
2. Verify network connectivity
3. Check for high write load
4. Consider scaling up primary server

#### 4. Storage Usage

**Metric**: Storage percentage  
**Threshold**: > 80%  
**Severity**: Medium (P2)

**Response Procedure**:
1. Review database size growth
2. Check for table bloat
3. Run VACUUM ANALYZE
4. Archive old data if appropriate
5. Increase storage size

## Response Procedures

### Initial Response Checklist

When an alert fires:

1. **Acknowledge Alert** (within SLA time)
   - Update incident tracking system
   - Notify team in communication channel

2. **Assess Severity**
   - Verify alert is not false positive
   - Check impact scope (all users, some users, specific features)
   - Upgrade/downgrade severity as needed

3. **Gather Information**
   - Check Application Insights for errors
   - Review recent deployments
   - Check Azure service health
   - Review related metrics

4. **Initial Mitigation**
   - Implement immediate fixes if available
   - Consider rollback if recent deployment
   - Scale resources if capacity issue
   - Enable feature flags to disable problematic features

5. **Communication**
   - Update incident status
   - Notify stakeholders
   - Provide estimated resolution time

6. **Resolution**
   - Implement fix
   - Verify resolution
   - Monitor for recurrence
   - Update incident tracking

7. **Post-Incident Review**
   - Document root cause
   - Identify preventive measures
   - Update runbooks
   - Schedule post-mortem if critical incident

### Deployment-Related Issues

If alert occurs within 30 minutes of deployment:

1. **Immediately consider rollback**
   ```bash
   # Follow rollback procedure in DEPLOYMENT.md
   # Navigate to GitHub Actions > Rollback Deployment
   ```

2. **Check deployment logs**
   - Review GitHub Actions workflow logs
   - Check Azure deployment logs
   - Verify all services started successfully

3. **Verify configuration**
   - Check environment variables
   - Verify connection strings
   - Check application settings

4. **Database migration issues**
   - Verify migrations completed successfully
   - Check for schema conflicts
   - Review migration logs

### Database Issues

#### Connection Pool Exhausted

```bash
# Check active connections
SELECT count(*) FROM pg_stat_activity WHERE datname = 'aspire_db';

# Kill idle connections
SELECT pg_terminate_backend(pid) 
FROM pg_stat_activity 
WHERE datname = 'aspire_db' 
  AND state = 'idle' 
  AND state_change < now() - interval '10 minutes';

# Update connection pool settings in App Service
az webapp config appsettings set \
  --name app-mycrm-backend-prod \
  --resource-group rg-mycrm-production \
  --settings "ConnectionStrings__DefaultConnection__MaxPoolSize=50"
```

#### Slow Queries

```bash
# Identify slow queries
SELECT 
  query,
  calls,
  mean_exec_time,
  max_exec_time
FROM pg_stat_statements 
ORDER BY mean_exec_time DESC 
LIMIT 10;

# Check for missing indexes
SELECT 
  schemaname,
  tablename,
  indexname,
  idx_scan,
  idx_tup_read
FROM pg_stat_user_indexes 
WHERE idx_scan = 0;
```

### Storage Issues

#### High Latency

```bash
# Check storage metrics
az monitor metrics list \
  --resource /subscriptions/{sub}/resourceGroups/rg-mycrm-production/providers/Microsoft.Storage/storageAccounts/stmycrmproduction \
  --metric "SuccessE2ELatency" \
  --start-time 2024-01-20T00:00:00Z

# Check for throttling
az monitor metrics list \
  --resource /subscriptions/{sub}/resourceGroups/rg-mycrm-production/providers/Microsoft.Storage/storageAccounts/stmycrmproduction \
  --metric "Transactions" \
  --filter "ResponseType eq 'ClientThrottlingError' or ResponseType eq 'ServerBusyError'"
```

## Escalation Matrix

### On-Call Rotation

| Role | Primary Contact | Backup Contact | Escalation Time |
|------|----------------|----------------|-----------------|
| DevOps Engineer | [Contact] | [Contact] | 15 min (P0), 30 min (P1) |
| Database Admin | [Contact] | [Contact] | 30 min (P0/P1) |
| Security Team | [Contact] | [Contact] | Immediate (security) |
| Infrastructure | [Contact] | [Contact] | 30 min (P0/P1) |
| Development Lead | [Contact] | [Contact] | 1 hour (P0/P1) |

### Escalation Paths

#### P0 - Critical Issues

```
Alert Fires
    ↓
On-Call DevOps Engineer (immediate)
    ↓ (15 minutes if not resolved)
DevOps Team Lead + Database Admin
    ↓ (30 minutes if not resolved)
Development Lead + Infrastructure Manager
    ↓ (1 hour if not resolved)
CTO/VP Engineering
```

#### P1 - High Priority

```
Alert Fires
    ↓
On-Call DevOps Engineer (30 minutes)
    ↓ (1 hour if not resolved)
DevOps Team Lead
    ↓ (2 hours if not resolved)
Development Lead
```

#### P2 - Medium Priority

```
Alert Fires
    ↓
DevOps Team (2 hours)
    ↓ (4 hours if not resolved)
DevOps Team Lead
```

### Communication Channels

- **Immediate**: Phone call
- **P0**: Slack #incidents + phone
- **P1**: Slack #incidents
- **P2/P3**: Slack #alerts
- **Stakeholder Updates**: Email + Slack #announcements

## Common Issues

### Issue: Application Not Accessible

**Symptoms**: Health check failing, 502/503 errors

**Common Causes**:
1. App Service stopped or restarting
2. VNet integration misconfigured
3. Private endpoint issues
4. Application crash/startup failure

**Resolution**:
```bash
# Check App Service status
az webapp show --name app-mycrm-backend-prod --resource-group rg-mycrm-production --query "state"

# Restart if needed
az webapp restart --name app-mycrm-backend-prod --resource-group rg-mycrm-production

# Check logs
az webapp log tail --name app-mycrm-backend-prod --resource-group rg-mycrm-production
```

### Issue: High Response Times

**Symptoms**: Slow page loads, timeout errors

**Common Causes**:
1. Database query performance
2. N+1 query problems
3. Resource constraints (CPU/Memory)
4. External service delays

**Resolution**:
1. Identify slow endpoints in Application Insights
2. Review SQL query performance
3. Check App Service metrics
4. Scale up if resource-constrained
5. Implement caching if appropriate

### Issue: Database Connection Errors

**Symptoms**: "Connection refused", "Too many connections"

**Common Causes**:
1. Connection pool exhausted
2. Database server overloaded
3. Network connectivity issues
4. Private endpoint misconfigured

**Resolution**:
```bash
# Check PostgreSQL status
az postgres flexible-server show \
  --name psql-mycrm-production \
  --resource-group rg-mycrm-production

# Check active connections
# Connect via psql and run:
SELECT count(*) FROM pg_stat_activity WHERE datname = 'aspire_db';

# Scale up if needed
az postgres flexible-server update \
  --name psql-mycrm-production \
  --resource-group rg-mycrm-production \
  --tier GeneralPurpose \
  --sku-name Standard_D4s_v3
```

### Issue: File Upload Failures

**Symptoms**: Upload errors, 403/404 on blob operations

**Common Causes**:
1. Managed Identity missing permissions
2. Private endpoint misconfigured
3. Storage account throttling
4. Network connectivity issues

**Resolution**:
```bash
# Check Managed Identity permissions
az role assignment list \
  --assignee <app-managed-identity-id> \
  --scope /subscriptions/{sub}/resourceGroups/rg-mycrm-production/providers/Microsoft.Storage/storageAccounts/stmycrmproduction

# Grant Storage Blob Data Contributor role if missing
az role assignment create \
  --assignee <app-managed-identity-id> \
  --role "Storage Blob Data Contributor" \
  --scope /subscriptions/{sub}/resourceGroups/rg-mycrm-production/providers/Microsoft.Storage/storageAccounts/stmycrmproduction
```

## Post-Incident Procedures

After resolving an incident:

1. **Document Resolution**
   - Update incident ticket with root cause
   - Document steps taken to resolve
   - Note any temporary workarounds

2. **Monitor for Recurrence**
   - Watch related metrics for 24 hours
   - Set up additional alerts if needed

3. **Schedule Post-Mortem** (for P0/P1)
   - Within 48 hours of resolution
   - Include all involved parties
   - Focus on prevention, not blame

4. **Update Documentation**
   - Update runbooks with lessons learned
   - Add new troubleshooting steps
   - Update alert configurations if needed

5. **Implement Preventive Measures**
   - Create action items from post-mortem
   - Assign owners and deadlines
   - Track in project management system

## Related Documentation

- [DEPLOYMENT.md](../DEPLOYMENT.md) - Deployment procedures and rollback
- [Infrastructure Configuration](../infrastructure/main.bicep) - Infrastructure as code
- [GitHub Actions Workflows](../.github/workflows/) - CI/CD workflows
