-- Pre-Migration Validation Script
-- Run this script BEFORE applying the migration to identify data conflicts
-- This identifies projects that have activities with multiple different customer_ids
-- or projects with activities that have NULL customer_ids

-- Find projects with conflicting customer assignments
SELECT 
    p.id AS project_id,
    p.name AS project_name,
    COUNT(DISTINCT pa.customer_id) AS distinct_customer_count,
    STRING_AGG(DISTINCT pa.customer_id::text, ', ') AS customer_ids
FROM projects p
LEFT JOIN project_activities pa ON pa.project_id = p.id
GROUP BY p.id, p.name
HAVING 
    COUNT(DISTINCT pa.customer_id) > 1  -- Multiple different customers
    OR (COUNT(DISTINCT pa.customer_id) = 1 AND COUNT(*) FILTER (WHERE pa.customer_id IS NULL) > 0)  -- Mix of NULL and non-NULL
    OR (COUNT(*) > 0 AND COUNT(DISTINCT pa.customer_id) = 0);  -- All activities have NULL customer_id

-- Find projects with no activities at all
SELECT 
    p.id AS project_id,
    p.name AS project_name,
    'No activities found' AS issue
FROM projects p
LEFT JOIN project_activities pa ON pa.project_id = p.id
WHERE pa.id IS NULL;

-- Summary counts
SELECT 
    'Total Projects' AS metric,
    COUNT(*) AS count
FROM projects
UNION ALL
SELECT 
    'Projects with Activities' AS metric,
    COUNT(DISTINCT project_id) AS count
FROM project_activities
UNION ALL
SELECT 
    'Projects needing manual review' AS metric,
    COUNT(DISTINCT p.id)
FROM projects p
LEFT JOIN project_activities pa ON pa.project_id = p.id
GROUP BY p.id
HAVING 
    COUNT(DISTINCT pa.customer_id) > 1
    OR (COUNT(DISTINCT pa.customer_id) = 1 AND COUNT(*) FILTER (WHERE pa.customer_id IS NULL) > 0)
    OR (COUNT(*) > 0 AND COUNT(DISTINCT pa.customer_id) = 0)
    OR COUNT(pa.id) = 0;
