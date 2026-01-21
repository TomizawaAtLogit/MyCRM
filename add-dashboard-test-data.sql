-- Dashboard Test Data Script
-- Adds sample Pre-Sales Proposals, Cases, and Projects to populate dashboard charts
-- Using actual customer IDs from the database: 1, 3, 4, 5

-- ============================================================
-- Pre-Sales Proposals (Plan Phase) - Different Stages
-- ============================================================

-- Identification Stage (3 proposals)
INSERT INTO presales_proposals (customer_id, title, description, stage, status, estimated_value, created_at, updated_at)
VALUES 
  (1, 'E-commerce Platform Inquiry', 'Initial inquiry from customer for e-commerce system', 'InitialContact', 'Pending', 150000, NOW(), NOW()),
  (1, 'CRM Implementation Question', 'Customer asking about CRM capabilities', 'InitialContact', 'Pending', 120000, NOW(), NOW()),
  (3, 'AI Analytics Solution', 'Customer inquiring about AI analytics', 'InitialContact', 'Pending', 250000, NOW(), NOW());

-- Qualification Stage (4 proposals)
INSERT INTO presales_proposals (customer_id, title, description, stage, status, estimated_value, created_at, updated_at)
VALUES 
  (1, 'Budget Tracking System', 'Full budget tracking and analysis system', 'RequirementGathering', 'InReview', 95000, NOW(), NOW()),
  (3, 'Data Integration Hub', 'Integrate multiple data sources', 'RequirementGathering', 'InReview', 180000, NOW(), NOW()),
  (1, 'Customer Portal Development', 'Self-service portal for customers', 'RequirementGathering', 'Pending', 140000, NOW(), NOW()),
  (4, 'Mobile Analytics App', 'Mobile version of analytics dashboard', 'RequirementGathering', 'Approved', 110000, NOW(), NOW());

-- Proposal Stage (3 proposals)
INSERT INTO presales_proposals (customer_id, title, description, stage, status, estimated_value, created_at, updated_at)
VALUES 
  (1, 'Enterprise Search Solution', 'Advanced search capabilities for enterprise', 'ProposalDevelopment', 'InReview', 200000, NOW(), NOW()),
  (4, 'Security Compliance Module', 'GDPR and security compliance tools', 'ProposalDevelopment', 'InReview', 175000, NOW(), NOW()),
  (3, 'Reporting Dashboard Premium', 'Advanced reporting with custom dashboards', 'ProposalDevelopment', 'Pending', 130000, NOW(), NOW());

-- Negotiation Stage (2 proposals)
INSERT INTO presales_proposals (customer_id, title, description, stage, status, estimated_value, created_at, updated_at)
VALUES 
  (3, 'Cloud Migration Services', 'Full migration to cloud infrastructure', 'NegotiationInProgress', 'InReview', 320000, NOW(), NOW()),
  (4, 'Support Package Enhancement', 'Premium 24/7 support with dedicated team', 'NegotiationInProgress', 'InReview', 85000, NOW(), NOW());

-- Won (Closed Won) (2 proposals)
INSERT INTO presales_proposals (customer_id, title, description, stage, status, estimated_value, created_at, updated_at)
VALUES 
  (1, 'Performance Optimization Project', 'System performance tuning and optimization', 'Won', 'Approved', 110000, NOW(), NOW()),
  (5, 'Data Warehouse Implementation', 'Enterprise data warehouse setup', 'Won', 'Approved', 450000, NOW(), NOW());

-- Lost (Closed Lost) (1 proposal)
INSERT INTO presales_proposals (customer_id, title, description, stage, status, estimated_value, created_at, updated_at)
VALUES 
  (4, 'Legacy System Modernization', 'Modernize outdated legacy systems', 'Lost', 'Pending', 300000, NOW(), NOW());

-- ============================================================
-- Cases (Do Phase) - Different Statuses
-- ============================================================

-- Open Cases (5 cases)
INSERT INTO cases (customer_id, title, description, status, priority, issue_type, created_at, updated_at)
VALUES 
  (1, 'Login timeout issues', 'Users experiencing timeout after 15 minutes of inactivity', 'Open', 'Critical', 'Incident', NOW() - INTERVAL '5 days', NOW()),
  (3, 'Report export failing', 'PDF export not working for large reports', 'Open', 'High', 'Incident', NOW() - INTERVAL '3 days', NOW()),
  (4, 'Performance degradation', 'System running slow during peak hours', 'Open', 'High', 'Incident', NOW() - INTERVAL '2 days', NOW()),
  (4, 'Email notification delays', 'Notifications arriving hours late', 'Open', 'Medium', 'ServiceRequest', NOW() - INTERVAL '1 day', NOW()),
  (1, 'UI rendering bug', 'Charts not displaying correctly on mobile', 'Open', 'Low', 'Bug', NOW() - INTERVAL '4 hours', NOW());

-- In Progress Cases (4 cases)
INSERT INTO cases (customer_id, title, description, status, priority, issue_type, created_at, updated_at)
VALUES 
  (3, 'Database connection pool exhaustion', 'Too many open connections causing errors', 'InProgress', 'Critical', 'Incident', NOW() - INTERVAL '6 days', NOW()),
  (1, 'API rate limiting issues', 'Legitimate requests being throttled', 'InProgress', 'High', 'Maintenance', NOW() - INTERVAL '4 days', NOW()),
  (4, 'Data sync inconsistencies', 'Master and replica databases out of sync', 'InProgress', 'High', 'Incident', NOW() - INTERVAL '3 days', NOW()),
  (5, 'User session management', 'Sessions not persisting across page refreshes', 'InProgress', 'Medium', 'Bug', NOW() - INTERVAL '2 days', NOW());

-- Resolved Cases (3 cases)
INSERT INTO cases (customer_id, title, description, status, priority, issue_type, created_at, updated_at, resolved_at, sla_deadline)
VALUES 
  (3, 'Memory leak in background service', 'Fixed memory leak causing service crashes', 'Resolved', 'Critical', 'Incident', NOW() - INTERVAL '7 days', NOW() - INTERVAL '1 day', NOW() - INTERVAL '1 day', NOW() - INTERVAL '2 days'),
  (1, 'Configuration validation error', 'Incorrect validation preventing valid configs', 'Resolved', 'Medium', 'Bug', NOW() - INTERVAL '5 days', NOW() - INTERVAL '1 day', NOW() - INTERVAL '1 day', NOW() - INTERVAL '4 days'),
  (4, 'SSL certificate expiration', 'Renewed expired SSL certificates', 'Resolved', 'High', 'Maintenance', NOW() - INTERVAL '3 days', NOW(), NOW(), NOW() + INTERVAL '1 day');

-- Closed Cases (2 cases)
INSERT INTO cases (customer_id, title, description, status, priority, issue_type, created_at, updated_at, resolved_at, closed_at, sla_deadline)
VALUES 
  (5, 'Feature request: Dark mode', 'Added dark mode UI theme', 'Closed', 'Low', 'ServiceRequest', NOW() - INTERVAL '15 days', NOW() - INTERVAL '5 days', NOW() - INTERVAL '5 days', NOW() - INTERVAL '4 days', NOW() - INTERVAL '10 days'),
  (1, 'Documentation update needed', 'Updated API documentation for new endpoints', 'Closed', 'Low', 'PlannedWork', NOW() - INTERVAL '10 days', NOW() - INTERVAL '4 days', NOW() - INTERVAL '4 days', NOW() - INTERVAL '3 days', NOW() - INTERVAL '7 days');

-- ============================================================
-- Projects (Act Phase) - Different Statuses
-- ============================================================

-- Active Projects (Wip) (3 projects)
INSERT INTO projects (customer_id, name, description, status, created_at)
VALUES 
  (1, 'Website Redesign Phase 2', 'Continuation of website redesign with new features', 'Wip', NOW() - INTERVAL '20 days'),
  (3, 'Mobile App Development', 'Native mobile app for iOS and Android platforms', 'Wip', NOW() - INTERVAL '30 days'),
  (4, 'API Gateway Implementation', 'Build API gateway for microservices', 'Wip', NOW() - INTERVAL '10 days');

-- Completed Projects (Closed) (2 projects)
INSERT INTO projects (customer_id, name, description, status, created_at)
VALUES 
  (5, 'Database Migration to PostgreSQL', 'Successfully migrated legacy database', 'Closed', NOW() - INTERVAL '60 days'),
  (1, 'Security Audit and Compliance', 'Completed security audit and implemented fixes', 'Closed', NOW() - INTERVAL '45 days');

-- On Hold Projects (Pending) (2 projects)
INSERT INTO projects (customer_id, name, description, status, created_at)
VALUES 
  (3, 'Advanced Analytics Platform', 'Planned analytics platform for future development', 'Pending', NOW() - INTERVAL '25 days'),
  (4, 'AI/ML Integration Phase 1', 'Initial AI/ML integration planning phase', 'Pending', NOW() - INTERVAL '15 days');

-- ============================================================
-- Project Activities (for project tracking)
-- ============================================================

-- Activities for Website Redesign Phase 2 (project ID will be assigned by DB)
-- Using a dynamic approach - activities are inserted after getting project IDs
-- For now, we use a simpler approach by getting the max project IDs

-- Get the latest project IDs and insert activities
INSERT INTO project_activities (project_id, activity_date, summary, description, activity_type, performed_by)
VALUES 
  ((SELECT id FROM projects WHERE name = 'Website Redesign Phase 2' LIMIT 1), NOW() - INTERVAL '15 days' + INTERVAL '1 hour', 'Project kickoff', 'Initial kickoff meeting with stakeholders', 'Meeting', 'Admin'),
  ((SELECT id FROM projects WHERE name = 'Website Redesign Phase 2' LIMIT 1), NOW() - INTERVAL '10 days', 'Design finalization', 'All design mockups approved by client', 'Design', 'Admin'),
  ((SELECT id FROM projects WHERE name = 'Website Redesign Phase 2' LIMIT 1), NOW() - INTERVAL '7 days', 'Development started', 'Development team began implementation', 'Development', 'Admin'),
  ((SELECT id FROM projects WHERE name = 'Website Redesign Phase 2' LIMIT 1), NOW() - INTERVAL '2 days', 'Testing phase initiated', 'QA team started testing', 'Testing', 'Admin');

-- Activities for Mobile App Development
INSERT INTO project_activities (project_id, activity_date, summary, description, activity_type, performed_by)
VALUES 
  ((SELECT id FROM projects WHERE name = 'Mobile App Development' LIMIT 1), NOW() - INTERVAL '28 days' + INTERVAL '1 hour', 'Architecture review', 'Approved application architecture', 'Planning', 'Admin'),
  ((SELECT id FROM projects WHERE name = 'Mobile App Development' LIMIT 1), NOW() - INTERVAL '20 days', 'Sprint 1 completed', 'First sprint with core features done', 'Development', 'Admin'),
  ((SELECT id FROM projects WHERE name = 'Mobile App Development' LIMIT 1), NOW() - INTERVAL '10 days', 'Sprint 2 in progress', 'Working on push notifications', 'Development', 'Admin'),
  ((SELECT id FROM projects WHERE name = 'Mobile App Development' LIMIT 1), NOW() - INTERVAL '3 days', 'Internal testing', 'Beta testing with internal team', 'Testing', 'Admin');

-- Activities for API Gateway Implementation
INSERT INTO project_activities (project_id, activity_date, summary, description, activity_type, performed_by)
VALUES 
  ((SELECT id FROM projects WHERE name = 'API Gateway Implementation' LIMIT 1), NOW() - INTERVAL '8 days' + INTERVAL '2 hours', 'Requirements gathering', 'Collected requirements from all teams', 'Planning', 'Admin'),
  ((SELECT id FROM projects WHERE name = 'API Gateway Implementation' LIMIT 1), NOW() - INTERVAL '5 days', 'Implementation started', 'Core gateway functionality development', 'Development', 'Admin'),
  ((SELECT id FROM projects WHERE name = 'API Gateway Implementation' LIMIT 1), NOW() - INTERVAL '1 day', 'Integration testing', 'Testing gateway with existing services', 'Testing', 'Admin');

-- ============================================================
-- Summary of Added Data:
-- ============================================================
-- Pre-Sales Proposals:
--   Identification: 3
--   Qualification: 4
--   Proposal: 3
--   Negotiation: 2
--   Won: 2
--   Lost: 1
--   Total: 15 proposals
--
-- Cases:
--   Open: 5 (2 Critical, 2 High, 1 Medium, 1 Low)
--   In Progress: 4 (1 Critical, 2 High, 1 Medium)
--   Resolved: 3
--   Closed: 2
--   Total: 14 cases
--
-- Projects:
--   Active (Wip): 3
--   Completed (Closed): 2
--   On Hold (Pending): 2
--   Total: 7 projects
--
-- Total Activities: 10
-- ============================================================
