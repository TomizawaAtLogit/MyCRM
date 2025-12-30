-- Manual migration to update customer_sites table
-- Remove City and State columns, rename ZipCode to PostCode

-- Check if the table exists
DO $$
BEGIN
    -- Rename ZipCode to PostCode if it exists
    IF EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_name = 'customer_sites' AND column_name = 'ZipCode'
    ) THEN
        ALTER TABLE customer_sites RENAME COLUMN "ZipCode" TO "PostCode";
    END IF;

    -- Drop City column if it exists
    IF EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_name = 'customer_sites' AND column_name = 'City'
    ) THEN
        ALTER TABLE customer_sites DROP COLUMN "City";
    END IF;

    -- Drop State column if it exists
    IF EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_name = 'customer_sites' AND column_name = 'State'
    ) THEN
        ALTER TABLE customer_sites DROP COLUMN "State";
    END IF;
END $$;
