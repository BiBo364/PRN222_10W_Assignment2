-- ============================================================
-- Migration: Roles Refactor (4 → 3) + Soft-delete for Documents & Subjects
-- Author: RAG Edu Hub Team
-- Date: 2026-06-25
-- ============================================================

-- ------------------------------------------------------------
-- STEP 1: Merge role "Nguoi upload" (id=3) into "Student" (id=3 after reassign)
-- Current:  1=SuperAdmin, 2=Lecturer, 3=NguoiUpload, 4=Student
-- Target:   1=Admin,      2=Lecturer, 3=Student
-- ------------------------------------------------------------

-- 1a. Update all users with roleId=4 (Student) → roleId=3
UPDATE users
SET role_id = 3
WHERE role_id = 4;

-- 1b. Update role names to reflect new naming
UPDATE roles SET name = 'Admin',    label = 'Admin'    WHERE id = 1;
UPDATE roles SET name = 'Lecturer', label = 'Giang vien' WHERE id = 2;
UPDATE roles SET name = 'Student',  label = 'Sinh vien' WHERE id = 3;

-- 1c. Delete old role id=4 (now unused)
DELETE FROM roles WHERE id = 4;

-- ------------------------------------------------------------
-- STEP 2: Add soft-delete columns to Documents table
-- ------------------------------------------------------------

ALTER TABLE documents
    ADD is_deleted  BIT           NOT NULL DEFAULT 0,
        deleted_at  DATETIME      NULL,
        deleted_by  INT           NULL;

-- Optional FK: deleted_by → users.id
ALTER TABLE documents
    ADD CONSTRAINT FK__documents__deleted_by FOREIGN KEY (deleted_by) REFERENCES users(id);

-- ------------------------------------------------------------
-- STEP 3: Add soft-delete columns to Subjects table
-- ------------------------------------------------------------

ALTER TABLE subjects
    ADD is_deleted  BIT           NOT NULL DEFAULT 0,
        deleted_at  DATETIME      NULL,
        deleted_by  INT           NULL;

-- Optional FK: deleted_by → users.id
ALTER TABLE subjects
    ADD CONSTRAINT FK__subjects__deleted_by FOREIGN KEY (deleted_by) REFERENCES users(id);

-- ------------------------------------------------------------
-- Done
-- ------------------------------------------------------------
PRINT 'Migration completed successfully.';
