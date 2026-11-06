ALTER DATABASE CHARACTER SET utf8mb4;


CREATE TABLE `auth_users` (
    `id` char(36) COLLATE ascii_general_ci NOT NULL,
    `email` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `password_hash` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `full_name` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `role` varchar(50) CHARACTER SET utf8mb4 NOT NULL,
    `status` varchar(50) CHARACTER SET utf8mb4 NOT NULL DEFAULT 'ACTIVE',
    `created_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    CONSTRAINT `pk_auth_users` PRIMARY KEY (`id`),
    CONSTRAINT `ck_auth_users_role` CHECK (role IN ('ROOT', 'DIRECTOR', 'ACADEMIC', 'SALES', 'ACCOUNTANT', 'TEACHER'))
) CHARACTER SET=utf8mb4;


CREATE TABLE `curriculum_programs` (
    `id` char(36) COLLATE ascii_general_ci NOT NULL,
    `code` text CHARACTER SET utf8mb4 NOT NULL,
    `name` text CHARACTER SET utf8mb4 NOT NULL,
    `level` text CHARACTER SET utf8mb4 NOT NULL,
    `total_units` int NOT NULL,
    `lessons_per_unit` int NOT NULL DEFAULT 7,
    `sessions_per_week` int NOT NULL DEFAULT 2,
    `fee_plan_id` char(36) COLLATE ascii_general_ci NULL,
    `created_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    CONSTRAINT `pk_curriculum_programs` PRIMARY KEY (`id`)
) CHARACTER SET=utf8mb4;


CREATE TABLE `students` (
    `id` char(36) COLLATE ascii_general_ci NOT NULL,
    `full_name` text CHARACTER SET utf8mb4 NOT NULL,
    `dob` date NULL,
    `gender` text CHARACTER SET utf8mb4 NULL,
    `phone` text CHARACTER SET utf8mb4 NULL,
    `email` text CHARACTER SET utf8mb4 NULL,
    `guardian_name` text CHARACTER SET utf8mb4 NULL,
    `guardian_phone` text CHARACTER SET utf8mb4 NULL,
    `address` text CHARACTER SET utf8mb4 NULL,
    `created_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    CONSTRAINT `pk_students` PRIMARY KEY (`id`)
) CHARACTER SET=utf8mb4;


CREATE TABLE `system_audit_logs` (
    `id` char(36) COLLATE ascii_general_ci NOT NULL,
    `actor_user_id` char(36) COLLATE ascii_general_ci NULL,
    `action` text CHARACTER SET utf8mb4 NOT NULL,
    `entity` text CHARACTER SET utf8mb4 NOT NULL,
    `entity_id` char(36) COLLATE ascii_general_ci NULL,
    `meta` json NOT NULL DEFAULT (JSON_OBJECT()),
    `created_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    CONSTRAINT `pk_system_audit_logs` PRIMARY KEY (`id`),
    CONSTRAINT `fk_system_audit_logs_users_actor_user_id` FOREIGN KEY (`actor_user_id`) REFERENCES `auth_users` (`id`) ON DELETE SET NULL
) CHARACTER SET=utf8mb4;


CREATE TABLE `system_notifications` (
    `id` char(36) COLLATE ascii_general_ci NOT NULL,
    `user_id` char(36) COLLATE ascii_general_ci NOT NULL,
    `title` text CHARACTER SET utf8mb4 NOT NULL,
    `body` text CHARACTER SET utf8mb4 NOT NULL,
    `is_read` tinyint(1) NOT NULL DEFAULT FALSE,
    `created_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    CONSTRAINT `pk_system_notifications` PRIMARY KEY (`id`),
    CONSTRAINT `fk_system_notifications_auth_users_user_id` FOREIGN KEY (`user_id`) REFERENCES `auth_users` (`id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;


CREATE TABLE `trial_leads` (
    `id` char(36) COLLATE ascii_general_ci NOT NULL,
    `full_name` text CHARACTER SET utf8mb4 NOT NULL,
    `phone` text CHARACTER SET utf8mb4 NOT NULL,
    `email` text CHARACTER SET utf8mb4 NULL,
    `source` text CHARACTER SET utf8mb4 NULL,
    `status` text CHARACTER SET utf8mb4 NOT NULL,
    `note` text CHARACTER SET utf8mb4 NULL,
    `created_by` char(36) COLLATE ascii_general_ci NOT NULL,
    `created_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    CONSTRAINT `pk_trial_leads` PRIMARY KEY (`id`),
    CONSTRAINT `ck_trial_leads_status` CHECK (status IN ('NEW', 'CONTACTED', 'SCHEDULED', 'ATTENDED', 'NO_SHOW', 'CONVERTED', 'CLOSED')),
    CONSTRAINT `fk_trial_leads_auth_users_created_by` FOREIGN KEY (`created_by`) REFERENCES `auth_users` (`id`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4;


CREATE TABLE `classes` (
    `id` char(36) COLLATE ascii_general_ci NOT NULL,
    `code` text CHARACTER SET utf8mb4 NOT NULL,
    `name` text CHARACTER SET utf8mb4 NOT NULL,
    `program_id` char(36) COLLATE ascii_general_ci NOT NULL,
    `room` text CHARACTER SET utf8mb4 NULL,
    `capacity` int NOT NULL DEFAULT 16,
    `start_date` date NOT NULL,
    `status` text CHARACTER SET utf8mb4 NOT NULL,
    `created_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    CONSTRAINT `pk_classes` PRIMARY KEY (`id`),
    CONSTRAINT `ck_classes_status` CHECK (status IN ('ACTIVE', 'PAUSED', 'CLOSED')),
    CONSTRAINT `fk_classes_programs_program_id` FOREIGN KEY (`program_id`) REFERENCES `curriculum_programs` (`id`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4;


CREATE TABLE `finance_fee_plans` (
    `id` char(36) COLLATE ascii_general_ci NOT NULL,
    `program_id` char(36) COLLATE ascii_general_ci NOT NULL,
    `name` text CHARACTER SET utf8mb4 NOT NULL,
    `amount` int NOT NULL,
    `currency` text CHARACTER SET utf8mb4 NOT NULL DEFAULT ('VND'),
    `sessions_per_week` int NOT NULL DEFAULT 2,
    `created_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    CONSTRAINT `pk_finance_fee_plans` PRIMARY KEY (`id`),
    CONSTRAINT `ck_finance_fee_plans_amount` CHECK (amount > 0),
    CONSTRAINT `fk_finance_fee_plans_curriculum_programs_program_id` FOREIGN KEY (`program_id`) REFERENCES `curriculum_programs` (`id`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4;


CREATE TABLE `enrollments` (
    `id` char(36) COLLATE ascii_general_ci NOT NULL,
    `student_id` char(36) COLLATE ascii_general_ci NOT NULL,
    `class_id` char(36) COLLATE ascii_general_ci NOT NULL,
    `status` text CHARACTER SET utf8mb4 NOT NULL,
    `start_date` date NOT NULL,
    `end_date` date NULL,
    `created_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    CONSTRAINT `pk_enrollments` PRIMARY KEY (`id`),
    CONSTRAINT `ck_enrollments_status` CHECK (status IN ('ACTIVE', 'PAUSED', 'DROPPED', 'TRANSFERRED', 'GRADUATED')),
    CONSTRAINT `fk_enrollments_classes_class_id` FOREIGN KEY (`class_id`) REFERENCES `classes` (`id`) ON DELETE RESTRICT,
    CONSTRAINT `fk_enrollments_students_student_id` FOREIGN KEY (`student_id`) REFERENCES `students` (`id`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4;


CREATE TABLE `sessions` (
    `id` char(36) COLLATE ascii_general_ci NOT NULL,
    `class_id` char(36) COLLATE ascii_general_ci NOT NULL,
    `session_date` date NOT NULL,
    `unit_no` int NOT NULL,
    `lesson_no` int NOT NULL,
    `session_type` text CHARACTER SET utf8mb4 NOT NULL,
    `main_teacher_id` char(36) COLLATE ascii_general_ci NULL,
    `cover_teacher_id` char(36) COLLATE ascii_general_ci NULL,
    `created_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    CONSTRAINT `pk_sessions` PRIMARY KEY (`id`),
    CONSTRAINT `ck_sessions_session_type` CHECK (session_type IN ('NORMAL', 'TEST', 'MIDTERM', 'FINAL')),
    CONSTRAINT `fk_sessions_classes_class_id` FOREIGN KEY (`class_id`) REFERENCES `classes` (`id`) ON DELETE RESTRICT,
    CONSTRAINT `fk_sessions_users_cover_teacher_id` FOREIGN KEY (`cover_teacher_id`) REFERENCES `auth_users` (`id`) ON DELETE RESTRICT,
    CONSTRAINT `fk_sessions_users_main_teacher_id` FOREIGN KEY (`main_teacher_id`) REFERENCES `auth_users` (`id`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4;


CREATE TABLE `finance_invoices` (
    `id` char(36) COLLATE ascii_general_ci NOT NULL,
    `enrollment_id` char(36) COLLATE ascii_general_ci NOT NULL,
    `amount` int NOT NULL,
    `status` text CHARACTER SET utf8mb4 NOT NULL,
    `due_date` date NOT NULL,
    `issued_at` datetime(6) NULL,
    `created_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    CONSTRAINT `pk_finance_invoices` PRIMARY KEY (`id`),
    CONSTRAINT `ck_finance_invoices_amount` CHECK (amount > 0),
    CONSTRAINT `ck_finance_invoices_status` CHECK (status IN ('DRAFT', 'ISSUED', 'PAID', 'OVERDUE', 'CANCELED')),
    CONSTRAINT `fk_finance_invoices_enrollments_enrollment_id` FOREIGN KEY (`enrollment_id`) REFERENCES `enrollments` (`id`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4;


CREATE TABLE `session_feedback` (
    `id` char(36) COLLATE ascii_general_ci NOT NULL,
    `session_id` char(36) COLLATE ascii_general_ci NOT NULL,
    `student_id` char(36) COLLATE ascii_general_ci NOT NULL,
    `attendance` text CHARACTER SET utf8mb4 NULL,
    `homework` text CHARACTER SET utf8mb4 NULL,
    `participation` text CHARACTER SET utf8mb4 NULL,
    `behavior` text CHARACTER SET utf8mb4 NULL,
    `comment_text` text CHARACTER SET utf8mb4 NULL,
    `teacher_id` char(36) COLLATE ascii_general_ci NOT NULL,
    `created_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    `updated_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    CONSTRAINT `pk_session_feedback` PRIMARY KEY (`id`),
    CONSTRAINT `fk_session_feedback_sessions_session_id` FOREIGN KEY (`session_id`) REFERENCES `sessions` (`id`) ON DELETE RESTRICT,
    CONSTRAINT `fk_session_feedback_students_student_id` FOREIGN KEY (`student_id`) REFERENCES `students` (`id`) ON DELETE RESTRICT,
    CONSTRAINT `fk_session_feedback_users_teacher_id` FOREIGN KEY (`teacher_id`) REFERENCES `auth_users` (`id`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4;


CREATE TABLE `session_scores` (
    `id` char(36) COLLATE ascii_general_ci NOT NULL,
    `session_id` char(36) COLLATE ascii_general_ci NOT NULL,
    `student_id` char(36) COLLATE ascii_general_ci NOT NULL,
    `score_type` text CHARACTER SET utf8mb4 NOT NULL,
    `language` int NULL,
    `cognitive` int NULL,
    `physical` int NULL,
    `social` int NULL,
    `total` int NOT NULL,
    `created_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    `updated_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    CONSTRAINT `pk_session_scores` PRIMARY KEY (`id`),
    CONSTRAINT `ck_session_scores_cognitive` CHECK (cognitive BETWEEN 0 AND 100),
    CONSTRAINT `ck_session_scores_language` CHECK (language BETWEEN 0 AND 100),
    CONSTRAINT `ck_session_scores_physical` CHECK (physical BETWEEN 0 AND 100),
    CONSTRAINT `ck_session_scores_score_type` CHECK (score_type IN ('TEST', 'MIDTERM', 'FINAL')),
    CONSTRAINT `ck_session_scores_social` CHECK (social BETWEEN 0 AND 100),
    CONSTRAINT `ck_session_scores_total` CHECK (total BETWEEN 0 AND 100),
    CONSTRAINT `fk_session_scores_sessions_session_id` FOREIGN KEY (`session_id`) REFERENCES `sessions` (`id`) ON DELETE RESTRICT,
    CONSTRAINT `fk_session_scores_students_student_id` FOREIGN KEY (`student_id`) REFERENCES `students` (`id`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4;


CREATE TABLE `finance_payments` (
    `id` char(36) COLLATE ascii_general_ci NOT NULL,
    `invoice_id` char(36) COLLATE ascii_general_ci NOT NULL,
    `amount` int NOT NULL,
    `method` text CHARACTER SET utf8mb4 NOT NULL,
    `paid_at` datetime(6) NOT NULL,
    `created_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    CONSTRAINT `pk_finance_payments` PRIMARY KEY (`id`),
    CONSTRAINT `ck_finance_payments_amount` CHECK (amount > 0),
    CONSTRAINT `ck_finance_payments_method` CHECK (method IN ('CASH', 'TRANSFER', 'CARD', 'OTHER')),
    CONSTRAINT `fk_finance_payments_finance_invoices_invoice_id` FOREIGN KEY (`invoice_id`) REFERENCES `finance_invoices` (`id`) ON DELETE RESTRICT
) CHARACTER SET=utf8mb4;


CREATE UNIQUE INDEX `ix_auth_users_email` ON `auth_users` (`email`);


CREATE UNIQUE INDEX `ix_classes_code` ON `classes` (`code`(50));


CREATE INDEX `ix_classes_program_id` ON `classes` (`program_id`);


CREATE INDEX `ix_classes_status` ON `classes` (`status`(20));


CREATE UNIQUE INDEX `ix_curriculum_programs_code` ON `curriculum_programs` (`code`(50));


CREATE INDEX `ix_curriculum_programs_fee_plan_id` ON `curriculum_programs` (`fee_plan_id`);


CREATE INDEX `ix_enrollments_class_id_status` ON `enrollments` (`class_id`, `status`(20));


CREATE INDEX `ix_enrollments_status` ON `enrollments` (`status`(20));


CREATE INDEX `ix_enrollments_student_id` ON `enrollments` (`student_id`);


CREATE INDEX `ix_finance_fee_plans_program_id` ON `finance_fee_plans` (`program_id`);


CREATE INDEX `ix_finance_invoices_due_date` ON `finance_invoices` (`due_date`);


CREATE INDEX `ix_finance_invoices_enrollment_id` ON `finance_invoices` (`enrollment_id`);


CREATE INDEX `ix_finance_invoices_status_due_date` ON `finance_invoices` (`status`(20), `due_date`);


CREATE INDEX `ix_finance_payments_invoice_id` ON `finance_payments` (`invoice_id`);


CREATE INDEX `ix_finance_payments_paid_at` ON `finance_payments` (`paid_at`);


CREATE UNIQUE INDEX `ix_session_feedback_session_id_student_id` ON `session_feedback` (`session_id`, `student_id`);


CREATE INDEX `ix_session_feedback_student_id` ON `session_feedback` (`student_id`);


CREATE INDEX `ix_session_feedback_teacher_id` ON `session_feedback` (`teacher_id`);


CREATE UNIQUE INDEX `ix_session_scores_session_id_student_id` ON `session_scores` (`session_id`, `student_id`);


CREATE INDEX `ix_session_scores_student_id` ON `session_scores` (`student_id`);


CREATE INDEX `ix_sessions_class_id_session_date` ON `sessions` (`class_id`, `session_date`);


CREATE INDEX `ix_sessions_cover_teacher_id` ON `sessions` (`cover_teacher_id`);


CREATE INDEX `ix_sessions_main_teacher_id` ON `sessions` (`main_teacher_id`);


CREATE INDEX `ix_sessions_session_date` ON `sessions` (`session_date`);


CREATE INDEX `ix_students_email` ON `students` (`email`(100));


CREATE INDEX `ix_students_phone` ON `students` (`phone`(20));


CREATE INDEX `ix_system_audit_logs_actor_user_id_action_created_at` ON `system_audit_logs` (`actor_user_id`, `action`(20), `created_at`);


CREATE INDEX `ix_system_audit_logs_created_at` ON `system_audit_logs` (`created_at`);


CREATE INDEX `ix_system_audit_logs_entity_entity_id` ON `system_audit_logs` (`entity`(50), `entity_id`);


CREATE INDEX `ix_system_notifications_user_id_is_read_created_at` ON `system_notifications` (`user_id`, `is_read`, `created_at`);


CREATE INDEX `ix_trial_leads_created_by` ON `trial_leads` (`created_by`);


CREATE INDEX `ix_trial_leads_phone` ON `trial_leads` (`phone`(20));


CREATE INDEX `ix_trial_leads_status` ON `trial_leads` (`status`(20));


