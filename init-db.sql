-- Create customers table first (referenced by projects)
CREATE TABLE IF NOT EXISTS customers (
    id SERIAL PRIMARY KEY,
    name VARCHAR(200) NOT NULL,
    contact_person VARCHAR(200),
    email VARCHAR(200),
    phone VARCHAR(50),
    address VARCHAR(500),
    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP
);

-- Create projects table
CREATE TABLE IF NOT EXISTS projects (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    description TEXT,
    customer_id INTEGER NOT NULL REFERENCES customers(id) ON DELETE RESTRICT,
    status VARCHAR(50) NOT NULL DEFAULT 'Wip',
    created_at TIMESTAMP NOT NULL DEFAULT NOW()
);

-- Create users table
CREATE TABLE IF NOT EXISTS users (
    id SERIAL PRIMARY KEY,
    windows_username VARCHAR(200) NOT NULL UNIQUE,
    display_name VARCHAR(200) NOT NULL,
    email VARCHAR(200),
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMP NOT NULL DEFAULT NOW()
);

-- Create roles table
CREATE TABLE IF NOT EXISTS roles (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL UNIQUE,
    description VARCHAR(500),
    page_permissions VARCHAR(1000) NOT NULL,
    created_at TIMESTAMP NOT NULL DEFAULT NOW()
);

-- Create user_roles table
CREATE TABLE IF NOT EXISTS user_roles (
    id SERIAL PRIMARY KEY,
    user_id INTEGER NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    role_id INTEGER NOT NULL REFERENCES roles(id) ON DELETE CASCADE,
    assigned_at TIMESTAMP NOT NULL DEFAULT NOW(),
    UNIQUE(user_id, role_id)
);

-- Create customer_databases table
CREATE TABLE IF NOT EXISTS customer_databases (
    id SERIAL PRIMARY KEY,
    customer_id INTEGER NOT NULL REFERENCES customers(id) ON DELETE CASCADE,
    database_name VARCHAR(200) NOT NULL,
    database_type VARCHAR(100),
    server_name VARCHAR(200),
    port VARCHAR(10),
    version VARCHAR(50),
    description TEXT,
    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP
);

-- Create customer_sites table
CREATE TABLE IF NOT EXISTS customer_sites (
    id SERIAL PRIMARY KEY,
    customer_id INTEGER NOT NULL REFERENCES customers(id) ON DELETE CASCADE,
    site_name VARCHAR(200) NOT NULL,
    address VARCHAR(500),
    post_code VARCHAR(20),
    country VARCHAR(100),
    contact_person VARCHAR(200),
    phone VARCHAR(50),
    description TEXT,
    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP
);

-- Create customer_systems table
CREATE TABLE IF NOT EXISTS customer_systems (
    id SERIAL PRIMARY KEY,
    customer_id INTEGER NOT NULL REFERENCES customers(id) ON DELETE CASCADE,
    system_name VARCHAR(200) NOT NULL,
    component_type VARCHAR(100),
    manufacturer VARCHAR(200),
    model VARCHAR(200),
    serial_number VARCHAR(200),
    location VARCHAR(200),
    installation_date DATE,
    warranty_expiration DATE,
    description TEXT,
    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP
);

-- Create systems table (new normalized structure)
CREATE TABLE IF NOT EXISTS systems (
    id SERIAL PRIMARY KEY,
    customer_id INTEGER NOT NULL REFERENCES customers(id) ON DELETE CASCADE,
    system_name VARCHAR(200) NOT NULL,
    installation_date DATE,
    description TEXT,
    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP
);

-- Create system_components table
CREATE TABLE IF NOT EXISTS system_components (
    id SERIAL PRIMARY KEY,
    system_id INTEGER NOT NULL REFERENCES systems(id) ON DELETE CASCADE,
    component_type VARCHAR(100) NOT NULL,
    manufacturer VARCHAR(200),
    model VARCHAR(200),
    serial_number VARCHAR(200),
    location VARCHAR(200),
    warranty_expiration DATE,
    description TEXT,
    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP
);

-- Create customer_orders table
CREATE TABLE IF NOT EXISTS customer_orders (
    id SERIAL PRIMARY KEY,
    customer_id INTEGER NOT NULL REFERENCES customers(id) ON DELETE CASCADE,
    order_number VARCHAR(100) NOT NULL UNIQUE,
    contract_type VARCHAR(100) NOT NULL,
    billing_frequency VARCHAR(50),
    status VARCHAR(50),
    contract_value DECIMAL(18, 2),
    start_date DATE,
    end_date DATE,
    description TEXT,
    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP
);

-- Create project_activities table (CustomerId removed - moved to projects table)
CREATE TABLE IF NOT EXISTS project_activities (
    id SERIAL PRIMARY KEY,
    project_id INTEGER NOT NULL REFERENCES projects(id) ON DELETE CASCADE,
    summary VARCHAR(500) NOT NULL,
    description VARCHAR(5000),
    next_action VARCHAR(1000),
    activity_type VARCHAR(100),
    performed_by VARCHAR(200),
    activity_date TIMESTAMP NOT NULL,
    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP
);

-- Create audit_logs table
CREATE TABLE IF NOT EXISTS audit_logs (
    id SERIAL PRIMARY KEY,
    timestamp TIMESTAMP NOT NULL DEFAULT NOW(),
    user_id INTEGER REFERENCES users(id) ON DELETE SET NULL,
    username VARCHAR(200) NOT NULL,
    action VARCHAR(50) NOT NULL,
    entity_type VARCHAR(100) NOT NULL,
    entity_id INTEGER,
    entity_snapshot TEXT,
    retention_until TIMESTAMP
);

-- Create entity_files table
CREATE TABLE IF NOT EXISTS entity_files (
    id SERIAL PRIMARY KEY,
    entity_type VARCHAR(50) NOT NULL,
    entity_id INTEGER NOT NULL,
    file_name VARCHAR(500) NOT NULL,
    original_file_name VARCHAR(500) NOT NULL,
    storage_path VARCHAR(1000) NOT NULL,
    file_size_bytes BIGINT NOT NULL,
    content_type VARCHAR(200) NOT NULL,
    description VARCHAR(2000),
    tags VARCHAR(2000),
    thumbnail_path VARCHAR(1000),
    uploaded_at TIMESTAMP NOT NULL DEFAULT NOW(),
    uploaded_by VARCHAR(200) NOT NULL,
    last_accessed_at TIMESTAMP,
    access_count INTEGER DEFAULT 0,
    retention_until TIMESTAMP,
    is_compressed BOOLEAN DEFAULT FALSE,
    original_size_bytes BIGINT
);

-- Create indexes
CREATE INDEX IF NOT EXISTS idx_customer_databases_customerid ON customer_databases(customer_id);
CREATE INDEX IF NOT EXISTS idx_customer_sites_customerid ON customer_sites(customer_id);
CREATE INDEX IF NOT EXISTS idx_customer_systems_customerid ON customer_systems(customer_id);
CREATE INDEX IF NOT EXISTS idx_customer_orders_customerid ON customer_orders(customer_id);
CREATE INDEX IF NOT EXISTS idx_customer_orders_ordernumber ON customer_orders(order_number);
CREATE INDEX IF NOT EXISTS idx_systems_customerid ON systems(customer_id);
CREATE INDEX IF NOT EXISTS idx_system_components_systemid ON system_components(system_id);
CREATE INDEX IF NOT EXISTS idx_system_components_serialnumber ON system_components(serial_number);
CREATE INDEX IF NOT EXISTS idx_projects_customerid ON projects(customer_id);
CREATE INDEX IF NOT EXISTS idx_project_activities_projectid ON project_activities(project_id);
CREATE INDEX IF NOT EXISTS idx_project_activities_activitydate ON project_activities(activity_date);
CREATE INDEX IF NOT EXISTS idx_audit_logs_timestamp ON audit_logs(timestamp);
CREATE INDEX IF NOT EXISTS idx_audit_logs_userid ON audit_logs(user_id);
CREATE INDEX IF NOT EXISTS idx_audit_logs_entitytype ON audit_logs(entity_type);
CREATE INDEX IF NOT EXISTS idx_audit_logs_entityid ON audit_logs(entity_id);
CREATE INDEX IF NOT EXISTS idx_audit_logs_retentionuntil ON audit_logs(retention_until);
CREATE INDEX IF NOT EXISTS idx_entity_files_entity ON entity_files(entity_type, entity_id);
CREATE INDEX IF NOT EXISTS idx_entity_files_uploadedat ON entity_files(uploaded_at);
CREATE INDEX IF NOT EXISTS idx_entity_files_retentionuntil ON entity_files(retention_until);

-- Create requirement_definitions table for pre-sales catalog
CREATE TABLE IF NOT EXISTS requirement_definitions (
    id SERIAL PRIMARY KEY,
    title VARCHAR(500) NOT NULL,
    description TEXT,
    customer_id INTEGER NOT NULL REFERENCES customers(id) ON DELETE RESTRICT,
    category VARCHAR(100),
    priority VARCHAR(50),
    status VARCHAR(50),
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE
);

CREATE INDEX IF NOT EXISTS idx_requirement_definitions_category ON requirement_definitions(category);
CREATE INDEX IF NOT EXISTS idx_requirement_definitions_customerid ON requirement_definitions(customer_id);
CREATE INDEX IF NOT EXISTS idx_requirement_definitions_status ON requirement_definitions(status);

-- Pre-sales proposals and activities tables
CREATE TABLE IF NOT EXISTS presales_proposals (
    id SERIAL PRIMARY KEY,
    title VARCHAR(500) NOT NULL,
    description TEXT,
    customer_id INTEGER NOT NULL REFERENCES customers(id) ON DELETE RESTRICT,
    requirement_definition_id INTEGER REFERENCES requirement_definitions(id) ON DELETE SET NULL,
    status VARCHAR(50) NOT NULL,
    stage VARCHAR(50) NOT NULL,
    assigned_to_user_id INTEGER REFERENCES users(id) ON DELETE SET NULL,
    estimated_value NUMERIC(18, 2),
    probability_percentage INTEGER,
    expected_close_date TIMESTAMP WITH TIME ZONE,
    closed_at TIMESTAMP WITH TIME ZONE,
    notes TEXT,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE
);

CREATE TABLE IF NOT EXISTS presales_activities (
    id SERIAL PRIMARY KEY,
    presales_proposal_id INTEGER NOT NULL REFERENCES presales_proposals(id) ON DELETE CASCADE,
    activity_date TIMESTAMP WITH TIME ZONE NOT NULL,
    summary VARCHAR(500) NOT NULL,
    description VARCHAR(5000),
    next_action VARCHAR(1000),
    activity_type VARCHAR(100),
    performed_by VARCHAR(200),
    previous_assigned_to_user_id INTEGER REFERENCES users(id) ON DELETE SET NULL,
    new_assigned_to_user_id INTEGER REFERENCES users(id) ON DELETE SET NULL,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE
);

CREATE INDEX IF NOT EXISTS idx_presales_proposals_assigned_to_user_id ON presales_proposals(assigned_to_user_id);
CREATE INDEX IF NOT EXISTS idx_presales_proposals_customer_id ON presales_proposals(customer_id);
CREATE INDEX IF NOT EXISTS idx_presales_proposals_expected_close_date ON presales_proposals(expected_close_date);
CREATE INDEX IF NOT EXISTS idx_presales_proposals_requirement_definition_id ON presales_proposals(requirement_definition_id);
CREATE INDEX IF NOT EXISTS idx_presales_proposals_stage ON presales_proposals(stage);
CREATE INDEX IF NOT EXISTS idx_presales_proposals_status ON presales_proposals(status);
CREATE INDEX IF NOT EXISTS idx_presales_activities_activity_date ON presales_activities(activity_date);
CREATE INDEX IF NOT EXISTS idx_presales_activities_presales_proposal_id ON presales_activities(presales_proposal_id);

