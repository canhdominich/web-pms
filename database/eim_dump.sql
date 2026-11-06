
/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;
DROP TABLE IF EXISTS `auth_roles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `auth_roles` (
  `id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `code` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `ix_auth_roles_code` (`code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

/*!40000 ALTER TABLE `auth_roles` DISABLE KEYS */;
INSERT INTO `auth_roles` (`id`, `code`, `name`) VALUES ('30e722bf-7b04-4a53-8383-e95ab87604af','ACCOUNTANT','Kế toán'),('5b187afb-24fa-4a14-9a5d-03b8fb6ddb66','DIRECTOR','Giám đốc'),('74cb55af-d72d-46d1-bc52-6ead1dd0f810','TEACHER','Giáo viên'),('8e2d598f-f8a4-4cf2-aa98-4621aa04a2f3','ACADEMIC','Học vụ'),('accf40c8-c753-42a6-9ca3-f12baa446bc5','SALES','Tuyển sinh'),('b7eede5d-2828-4a51-8409-aa5d80054c6d','ROOT','Quản trị hệ thống');
/*!40000 ALTER TABLE `auth_roles` ENABLE KEYS */;
DROP TABLE IF EXISTS `auth_users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `auth_users` (
  `id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `email` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `password_hash` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `full_name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `status` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT 'ACTIVE',
  `created_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  PRIMARY KEY (`id`),
  UNIQUE KEY `ix_auth_users_email` (`email`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

/*!40000 ALTER TABLE `auth_users` DISABLE KEYS */;
INSERT INTO `auth_users` (`id`, `email`, `password_hash`, `full_name`, `status`, `created_at`) VALUES ('1939cd58-d478-4c54-ba6e-f7fa50b8672c','teacher2@eim.local','AQAAAAIAAYagAAAAEGH4UMfK3YZh1En9l3UR/vh8sVNvtL9dBuRmEi4h6NPvmrPv5EPRwnJZlAmQs6SESQ==','Lê Quốc Bảo','ACTIVE','2026-11-05 23:59:32.146264'),('3ba96209-300d-43bd-afed-accdbac16782','sales@eim.local','AQAAAAIAAYagAAAAEAcsEKgzxC8d99GzqmbbMs+3X+FimDHBQiIUPlhWoJRxMxM8EULlur8tDjRBJLHlfQ==','Lê Hoàng Yến','ACTIVE','2026-11-05 23:59:31.807805'),('4b18f289-ea31-4d83-96d6-535500e39c2d','academic@eim.local','AQAAAAIAAYagAAAAEHaYWyPZhiYrqk5hQZqQ2xvh3lmzZeK1eNeHqbiL/jVKk2dvL+4fDI8bjEO5rqmTsQ==','Phạm Thu Hương','ACTIVE','2026-11-05 23:59:31.692882'),('4de4f0a0-8cfb-4796-9eca-6e4ce81d2aa0','director@eim.local','AQAAAAIAAYagAAAAENcbXmx9GDLh1OA1+BYZVNW7fEkE9IZZDkpTsMK5qphAQdSCF6IQyhrVmwP8iPda0A==','Nguyễn Minh Đức','ACTIVE','2026-11-05 23:59:31.570914'),('8e467049-0746-441d-8ca6-7439d7acda02','root@eim.local','AQAAAAIAAYagAAAAENAlrfgNhrCLSdB99+/9jhXUIqf4G7HLKiAuT3gh+iOvTs0qMEdXR9jhbWlT4SBy7g==','System Root','ACTIVE','2026-11-05 23:46:09.431871'),('a2786b12-9205-48eb-9427-d8d14b56004a','accountant@eim.local','AQAAAAIAAYagAAAAEMwgLRq32nhAavb2deEiP2y8knBuc0/J9vZMO1sS5wZeW5HX4p52bmeBClD0KqqiAQ==','Đỗ Thanh Tâm','ACTIVE','2026-11-05 23:59:31.919898'),('e5127222-da47-4e25-8f9f-457e8a09741b','teacher1@eim.local','AQAAAAIAAYagAAAAEHFkkmTYNYezDmfVQfuXdgtSoCh2bKLmWr6sMLiiBxwOcOZXixhhonhgxDWDwMB7vg==','Trần Thu Hà','ACTIVE','2026-11-05 23:59:32.034442');
/*!40000 ALTER TABLE `auth_users` ENABLE KEYS */;
DROP TABLE IF EXISTS `classes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `classes` (
  `id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `code` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `name` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `program_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `room` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `capacity` int NOT NULL DEFAULT '16',
  `start_date` date NOT NULL,
  `status` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `created_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  PRIMARY KEY (`id`),
  UNIQUE KEY `ix_classes_code` (`code`(50)),
  KEY `ix_classes_program_id` (`program_id`),
  KEY `ix_classes_status` (`status`(20)),
  CONSTRAINT `fk_classes_programs_program_id` FOREIGN KEY (`program_id`) REFERENCES `curriculum_programs` (`id`) ON DELETE RESTRICT,
  CONSTRAINT `ck_classes_status` CHECK ((`status` in (_utf8mb4'ACTIVE',_utf8mb4'PAUSED',_utf8mb4'CLOSED')))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

/*!40000 ALTER TABLE `classes` DISABLE KEYS */;
INSERT INTO `classes` (`id`, `code`, `name`, `program_id`, `room`, `capacity`, `start_date`, `status`, `created_at`) VALUES ('58abc75f-b941-404e-a206-74af63fc618e','EIM1-STARTERS-2026A','Starters Tối 3-5','c21b872c-fc62-4954-9bfb-ae2def69a81a','P101',16,'2026-10-06','ACTIVE','2026-11-05 23:59:32.151710'),('80e1a826-97f8-433d-9705-7a95ed8a93e3','EIM1-MOVERS-2026A','Movers Tối 2-4','04e8904e-16ff-495e-9fda-f9027c7d175c','P202',16,'2026-11-16','ACTIVE','2026-11-05 23:59:32.154080');
/*!40000 ALTER TABLE `classes` ENABLE KEYS */;
DROP TABLE IF EXISTS `curriculum_programs`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `curriculum_programs` (
  `id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `code` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `name` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `level` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `total_units` int NOT NULL,
  `lessons_per_unit` int NOT NULL DEFAULT '7',
  `sessions_per_week` int NOT NULL DEFAULT '2',
  `fee_plan_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `created_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  PRIMARY KEY (`id`),
  UNIQUE KEY `ix_curriculum_programs_code` (`code`(50)),
  KEY `ix_curriculum_programs_fee_plan_id` (`fee_plan_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

/*!40000 ALTER TABLE `curriculum_programs` DISABLE KEYS */;
INSERT INTO `curriculum_programs` (`id`, `code`, `name`, `level`, `total_units`, `lessons_per_unit`, `sessions_per_week`, `fee_plan_id`, `created_at`) VALUES ('04e8904e-16ff-495e-9fda-f9027c7d175c','MOVERS','Cambridge Movers','A1',12,7,2,'b22323c4-b5b9-422b-83de-33b9160f6dfb','2026-11-05 23:59:31.414267'),('15afd622-54c8-4c50-bf99-db36e8471e43','FLYERS','Cambridge Flyers','A2',12,7,2,'c23f0092-f4b6-41e7-bca9-6ed195b8ec59','2026-11-05 23:59:31.414267'),('18858f9e-8e85-47f8-9495-bf4f01db4373','KINDY','Kindy','Pre-Starters',10,7,2,'94b3c1e1-ca70-43bb-b4c5-997950588810','2026-11-05 23:59:31.414267'),('c21b872c-fc62-4954-9bfb-ae2def69a81a','STARTERS','Cambridge Starters','Pre-A1',12,7,2,'aca6b009-0388-4b8b-9152-784cb8c5dcc4','2026-11-05 23:59:31.414267');
/*!40000 ALTER TABLE `curriculum_programs` ENABLE KEYS */;
DROP TABLE IF EXISTS `enrollments`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `enrollments` (
  `id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `student_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `class_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `status` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `start_date` date NOT NULL,
  `end_date` date DEFAULT NULL,
  `created_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  PRIMARY KEY (`id`),
  KEY `ix_enrollments_class_id_status` (`class_id`,`status`(20)),
  KEY `ix_enrollments_status` (`status`(20)),
  KEY `ix_enrollments_student_id` (`student_id`),
  CONSTRAINT `fk_enrollments_classes_class_id` FOREIGN KEY (`class_id`) REFERENCES `classes` (`id`) ON DELETE RESTRICT,
  CONSTRAINT `fk_enrollments_students_student_id` FOREIGN KEY (`student_id`) REFERENCES `students` (`id`) ON DELETE RESTRICT,
  CONSTRAINT `ck_enrollments_status` CHECK ((`status` in (_utf8mb4'ACTIVE',_utf8mb4'PAUSED',_utf8mb4'DROPPED',_utf8mb4'TRANSFERRED',_utf8mb4'GRADUATED')))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

/*!40000 ALTER TABLE `enrollments` DISABLE KEYS */;
INSERT INTO `enrollments` (`id`, `student_id`, `class_id`, `status`, `start_date`, `end_date`, `created_at`) VALUES ('51ebc07b-bc59-4a89-8475-7d73c4aab7c9','b626af7c-4598-48ab-b9a0-4777709f09f8','80e1a826-97f8-433d-9705-7a95ed8a93e3','ACTIVE','2026-11-16',NULL,'2026-11-05 23:59:32.157125'),('655e7033-1cb1-460d-b565-f65608ae6adf','b7331574-8932-4acb-8469-ac82505e5c55','58abc75f-b941-404e-a206-74af63fc618e','ACTIVE','2026-10-06',NULL,'2026-11-05 23:59:32.157038'),('9a828caf-6049-479d-ae38-353e4f61b8d1','a7f623bc-58f6-487d-acff-fc7069fcd304','58abc75f-b941-404e-a206-74af63fc618e','ACTIVE','2026-10-06',NULL,'2026-11-05 23:59:32.157083'),('ce3631c7-c05f-44c6-88b4-bc81bb45d506','c37c2597-b724-4334-9aa5-63c77fce4f11','58abc75f-b941-404e-a206-74af63fc618e','ACTIVE','2026-10-06',NULL,'2026-11-05 23:59:32.156948'),('f4dbd94a-8b80-416d-a83f-c46dd8d217a1','cd6fdfb8-9ce0-4e1e-8dec-13baf50481eb','58abc75f-b941-404e-a206-74af63fc618e','ACTIVE','2026-10-06',NULL,'2026-11-05 23:59:32.157009'),('f53ca11a-f70a-4fe3-a545-6d982b030d4a','a4861778-772e-42ee-9f52-3d93958e6ca1','58abc75f-b941-404e-a206-74af63fc618e','ACTIVE','2026-10-06',NULL,'2026-11-05 23:59:32.156981'),('f58e8231-9e93-477c-90bb-275afd00a7eb','7b850206-7cf6-4514-9c64-654ab1b27980','80e1a826-97f8-433d-9705-7a95ed8a93e3','ACTIVE','2026-11-16',NULL,'2026-11-05 23:59:32.157152'),('fd0dc570-26e7-427a-a3e8-d2dfdfc06d84','e499904a-3666-47c4-b356-9b3cff43d038','58abc75f-b941-404e-a206-74af63fc618e','ACTIVE','2026-10-06',NULL,'2026-11-05 23:59:32.156880');
/*!40000 ALTER TABLE `enrollments` ENABLE KEYS */;
DROP TABLE IF EXISTS `finance_fee_plans`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `finance_fee_plans` (
  `id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `program_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `name` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `amount` int NOT NULL,
  `currency` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT (_utf8mb4'VND'),
  `sessions_per_week` int NOT NULL DEFAULT '2',
  `created_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  PRIMARY KEY (`id`),
  KEY `ix_finance_fee_plans_program_id` (`program_id`),
  CONSTRAINT `fk_finance_fee_plans_curriculum_programs_program_id` FOREIGN KEY (`program_id`) REFERENCES `curriculum_programs` (`id`) ON DELETE RESTRICT,
  CONSTRAINT `ck_finance_fee_plans_amount` CHECK ((`amount` > 0))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

/*!40000 ALTER TABLE `finance_fee_plans` DISABLE KEYS */;
INSERT INTO `finance_fee_plans` (`id`, `program_id`, `name`, `amount`, `currency`, `sessions_per_week`, `created_at`) VALUES ('94b3c1e1-ca70-43bb-b4c5-997950588810','18858f9e-8e85-47f8-9495-bf4f01db4373','Kindy - Gói 3 tháng',3600000,'VND',2,'2026-11-05 23:59:31.414267'),('aca6b009-0388-4b8b-9152-784cb8c5dcc4','c21b872c-fc62-4954-9bfb-ae2def69a81a','Cambridge Starters - Gói 3 tháng',4200000,'VND',2,'2026-11-05 23:59:31.414267'),('b22323c4-b5b9-422b-83de-33b9160f6dfb','04e8904e-16ff-495e-9fda-f9027c7d175c','Cambridge Movers - Gói 3 tháng',4800000,'VND',2,'2026-11-05 23:59:31.414267'),('c23f0092-f4b6-41e7-bca9-6ed195b8ec59','15afd622-54c8-4c50-bf99-db36e8471e43','Cambridge Flyers - Gói 3 tháng',5400000,'VND',2,'2026-11-05 23:59:31.414267');
/*!40000 ALTER TABLE `finance_fee_plans` ENABLE KEYS */;
DROP TABLE IF EXISTS `finance_invoices`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `finance_invoices` (
  `id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `enrollment_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `amount` int NOT NULL,
  `status` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `due_date` date NOT NULL,
  `issued_at` datetime(6) DEFAULT NULL,
  `created_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  PRIMARY KEY (`id`),
  KEY `ix_finance_invoices_due_date` (`due_date`),
  KEY `ix_finance_invoices_enrollment_id` (`enrollment_id`),
  KEY `ix_finance_invoices_status_due_date` (`status`(20),`due_date`),
  CONSTRAINT `fk_finance_invoices_enrollments_enrollment_id` FOREIGN KEY (`enrollment_id`) REFERENCES `enrollments` (`id`) ON DELETE RESTRICT,
  CONSTRAINT `ck_finance_invoices_amount` CHECK ((`amount` > 0)),
  CONSTRAINT `ck_finance_invoices_status` CHECK ((`status` in (_utf8mb4'DRAFT',_utf8mb4'ISSUED',_utf8mb4'PAID',_utf8mb4'OVERDUE',_utf8mb4'CANCELED')))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

/*!40000 ALTER TABLE `finance_invoices` DISABLE KEYS */;
INSERT INTO `finance_invoices` (`id`, `enrollment_id`, `amount`, `status`, `due_date`, `issued_at`, `created_at`) VALUES ('48ccba31-e919-40eb-82b5-995c77287de4','f4dbd94a-8b80-416d-a83f-c46dd8d217a1',4200000,'OVERDUE','2026-11-01','2026-10-11 23:59:31.414267','2026-10-11 23:59:31.414267'),('49e19156-3c5a-41e4-86d6-6d3d5e32b4af','ce3631c7-c05f-44c6-88b4-bc81bb45d506',4200000,'PAID','2026-10-17','2026-10-11 23:59:31.414267','2026-10-11 23:59:31.414267'),('89cb41e7-5a1a-4b89-8b16-15a671cc5ac7','fd0dc570-26e7-427a-a3e8-d2dfdfc06d84',4200000,'PAID','2026-10-17','2026-10-11 23:59:31.414267','2026-10-11 23:59:31.414267'),('8e954f91-63d8-48c8-8903-0a23b11bd3b3','655e7033-1cb1-460d-b565-f65608ae6adf',4200000,'ISSUED','2026-11-26','2026-10-11 23:59:31.414267','2026-10-11 23:59:31.414267'),('b5bdd2a3-3219-4b56-9d3a-1c7f107722bc','9a828caf-6049-479d-ae38-353e4f61b8d1',4200000,'DRAFT','2026-12-06',NULL,'2026-10-11 23:59:31.414267'),('d2542a31-2f27-4495-be53-4088360f008f','f53ca11a-f70a-4fe3-a545-6d982b030d4a',4200000,'ISSUED','2026-11-16','2026-10-11 23:59:31.414267','2026-10-11 23:59:31.414267');
/*!40000 ALTER TABLE `finance_invoices` ENABLE KEYS */;
DROP TABLE IF EXISTS `finance_payments`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `finance_payments` (
  `id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `invoice_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `amount` int NOT NULL,
  `method` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `paid_at` datetime(6) NOT NULL,
  `created_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  PRIMARY KEY (`id`),
  KEY `ix_finance_payments_invoice_id` (`invoice_id`),
  KEY `ix_finance_payments_paid_at` (`paid_at`),
  CONSTRAINT `fk_finance_payments_finance_invoices_invoice_id` FOREIGN KEY (`invoice_id`) REFERENCES `finance_invoices` (`id`) ON DELETE RESTRICT,
  CONSTRAINT `ck_finance_payments_amount` CHECK ((`amount` > 0)),
  CONSTRAINT `ck_finance_payments_method` CHECK ((`method` in (_utf8mb4'CASH',_utf8mb4'TRANSFER',_utf8mb4'CARD',_utf8mb4'OTHER')))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

/*!40000 ALTER TABLE `finance_payments` DISABLE KEYS */;
INSERT INTO `finance_payments` (`id`, `invoice_id`, `amount`, `method`, `paid_at`, `created_at`) VALUES ('4573c417-0d55-4c13-b0b7-13652836d036','49e19156-3c5a-41e4-86d6-6d3d5e32b4af',4200000,'CASH','2026-10-14 23:59:31.414267','2026-10-14 23:59:31.414267'),('77ecb4d9-fe75-43ce-9c68-38d3b3aedd38','89cb41e7-5a1a-4b89-8b16-15a671cc5ac7',4200000,'TRANSFER','2026-10-14 23:59:31.414267','2026-10-14 23:59:31.414267'),('f77b10b1-7ab3-4582-b669-0c4735b72acf','d2542a31-2f27-4495-be53-4088360f008f',2100000,'CARD','2026-11-02 23:59:31.414267','2026-11-02 23:59:31.414267');
/*!40000 ALTER TABLE `finance_payments` ENABLE KEYS */;
DROP TABLE IF EXISTS `session_feedback`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `session_feedback` (
  `id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `session_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `student_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `attendance` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `homework` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `participation` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `behavior` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `comment_text` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `teacher_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `created_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `updated_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  PRIMARY KEY (`id`),
  UNIQUE KEY `ix_session_feedback_session_id_student_id` (`session_id`,`student_id`),
  KEY `ix_session_feedback_student_id` (`student_id`),
  KEY `ix_session_feedback_teacher_id` (`teacher_id`),
  CONSTRAINT `fk_session_feedback_sessions_session_id` FOREIGN KEY (`session_id`) REFERENCES `sessions` (`id`) ON DELETE RESTRICT,
  CONSTRAINT `fk_session_feedback_students_student_id` FOREIGN KEY (`student_id`) REFERENCES `students` (`id`) ON DELETE RESTRICT,
  CONSTRAINT `fk_session_feedback_users_teacher_id` FOREIGN KEY (`teacher_id`) REFERENCES `auth_users` (`id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

/*!40000 ALTER TABLE `session_feedback` DISABLE KEYS */;
INSERT INTO `session_feedback` (`id`, `session_id`, `student_id`, `attendance`, `homework`, `participation`, `behavior`, `comment_text`, `teacher_id`, `created_at`, `updated_at`) VALUES ('024617a1-4c44-4639-8e4d-81c84a0bbe87','dfb95fea-1db3-40cb-9723-bf654fd3a297','cd6fdfb8-9ce0-4e1e-8dec-13baf50481eb','PRESENT','NOT_DONE','Tích cực phát biểu','Ngoan, tập trung','Hoàn thành tốt nội dung Unit 2','e5127222-da47-4e25-8f9f-457e8a09741b','2026-11-05 23:59:31.414267','2026-11-05 23:59:31.414267'),('07552722-d7bd-4a9b-8236-f1a3f3cfb2b4','8a1ef372-6137-4ea9-8d33-8c3b5c72248a','cd6fdfb8-9ce0-4e1e-8dec-13baf50481eb','PRESENT','DONE','Tích cực phát biểu','Ngoan, tập trung','Hoàn thành tốt nội dung Unit 3','e5127222-da47-4e25-8f9f-457e8a09741b','2026-11-05 23:59:31.414267','2026-11-05 23:59:31.414267'),('0dd6da9d-3014-49ec-9031-adf8020ce982','e66b1258-0a92-47df-9bd9-03ecbddd2686','a7f623bc-58f6-487d-acff-fc7069fcd304','LATE','NOT_DONE','Cần chủ động hơn','Ngoan, tập trung','Hoàn thành tốt nội dung Unit 1','e5127222-da47-4e25-8f9f-457e8a09741b','2026-11-05 23:59:31.414267','2026-11-05 23:59:31.414267'),('0ee8190e-bb72-486f-97f9-f3bd0b7f3251','81541df0-5ace-487c-9569-0426c2138967','a4861778-772e-42ee-9f52-3d93958e6ca1','LATE','DONE','Tích cực phát biểu','Ngoan, tập trung','Hoàn thành tốt nội dung Unit 2','e5127222-da47-4e25-8f9f-457e8a09741b','2026-11-05 23:59:31.414267','2026-11-05 23:59:31.414267'),('12981541-7ae8-462a-8082-41eebf594ffb','af70ed70-e76a-458c-9324-47b7c635f3ae','cd6fdfb8-9ce0-4e1e-8dec-13baf50481eb','PRESENT','DONE','Tích cực phát biểu','Ngoan, tập trung','Hoàn thành tốt nội dung Unit 1','e5127222-da47-4e25-8f9f-457e8a09741b','2026-11-05 23:59:31.414267','2026-11-05 23:59:31.414267'),('14633df4-8b99-45eb-9135-4650e7edea7d','dfb95fea-1db3-40cb-9723-bf654fd3a297','c37c2597-b724-4334-9aa5-63c77fce4f11','PRESENT','DONE','Tích cực phát biểu','Ngoan, tập trung','Hoàn thành tốt nội dung Unit 2','e5127222-da47-4e25-8f9f-457e8a09741b','2026-11-05 23:59:31.414267','2026-11-05 23:59:31.414267'),('15a5b7f3-93bc-4c2b-8d1f-fd7829068186','f4d989c2-eae5-4295-9b98-61f4b0e61c11','e499904a-3666-47c4-b356-9b3cff43d038','PRESENT','DONE','Tích cực phát biểu','Ngoan, tập trung','Hoàn thành tốt nội dung Unit 2','e5127222-da47-4e25-8f9f-457e8a09741b','2026-11-05 23:59:31.414267','2026-11-05 23:59:31.414267'),('178411c8-3029-4e3c-a5ee-04eed16de22b','e66b1258-0a92-47df-9bd9-03ecbddd2686','c37c2597-b724-4334-9aa5-63c77fce4f11','PRESENT','DONE','Cần chủ động hơn','Ngoan, tập trung','Hoàn thành tốt nội dung Unit 1','e5127222-da47-4e25-8f9f-457e8a09741b','2026-11-05 23:59:31.414267','2026-11-05 23:59:31.414267'),('19c43bd9-473a-41c5-8aa2-52febcbff3f5','c5de79bf-0f7e-45b7-b120-4bc1c5205d51','a7f623bc-58f6-487d-acff-fc7069fcd304','PRESENT','DONE','Tích cực phát biểu','Ngoan, tập trung','Hoàn thành tốt nội dung Unit 2','e5127222-da47-4e25-8f9f-457e8a09741b','2026-11-05 23:59:31.414267','2026-11-05 23:59:31.414267'),('1a08f9c6-6949-4c47-a02a-647a9931b446','af70ed70-e76a-458c-9324-47b7c635f3ae','e499904a-3666-47c4-b356-9b3cff43d038','PRESENT','DONE','Cần chủ động hơn','Ngoan, tập trung','Hoàn thành tốt nội dung Unit 1','e5127222-da47-4e25-8f9f-457e8a09741b','2026-11-05 23:59:31.414267','2026-11-05 23:59:31.414267'),('1f474320-e528-4e11-ac67-c3db27732b17','dfb95fea-1db3-40cb-9723-bf654fd3a297','a4861778-772e-42ee-9f52-3d93958e6ca1','ABSENT','DONE','Cần chủ động hơn','Ngoan, tập trung','Hoàn thành tốt nội dung Unit 2','e5127222-da47-4e25-8f9f-457e8a09741b','2026-11-05 23:59:31.414267','2026-11-05 23:59:31.414267'),('204ec2e0-a4ca-4b60-83a4-b12568843f5b','81541df0-5ace-487c-9569-0426c2138967','cd6fdfb8-9ce0-4e1e-8dec-13baf50481eb','PRESENT','NOT_DONE','Cần chủ động hơn','Ngoan, tập trung','Hoàn thành tốt nội dung Unit 2','e5127222-da47-4e25-8f9f-457e8a09741b','2026-11-05 23:59:31.414267','2026-11-05 23:59:31.414267'),('279c0454-2660-4254-92a7-8540942f6f7d','e66b1258-0a92-47df-9bd9-03ecbddd2686','b7331574-8932-4acb-8469-ac82505e5c55','PRESENT','DONE','Tích cực phát biểu','Ngoan, tập trung','Hoàn thành tốt nội dung Unit 1','e5127222-da47-4e25-8f9f-457e8a09741b','2026-11-05 23:59:31.414267','2026-11-05 23:59:31.414267'),('2c8176a7-c620-43af-8fb9-ff98e22e42cd','af70ed70-e76a-458c-9324-47b7c635f3ae','c37c2597-b724-4334-9aa5-63c77fce4f11','PRESENT','NOT_DONE','Tích cực phát biểu','Ngoan, tập trung','Hoàn thành tốt nội dung Unit 1','e5127222-da47-4e25-8f9f-457e8a09741b','2026-11-05 23:59:31.414267','2026-11-05 23:59:31.414267'),('3225f248-dce9-4b29-a97b-09ee471f7808','8a1ef372-6137-4ea9-8d33-8c3b5c72248a','e499904a-3666-47c4-b356-9b3cff43d038','PRESENT','DONE','Cần chủ động hơn','Ngoan, tập trung','Hoàn thành tốt nội dung Unit 3','e5127222-da47-4e25-8f9f-457e8a09741b','2026-11-05 23:59:31.414267','2026-11-05 23:59:31.414267'),('339c25c6-a5e5-4f36-9869-6ed809a796bc','e66b1258-0a92-47df-9bd9-03ecbddd2686','e499904a-3666-47c4-b356-9b3cff43d038','ABSENT','DONE','Tích cực phát biểu','Ngoan, tập trung','Hoàn thành tốt nội dung Unit 1','e5127222-da47-4e25-8f9f-457e8a09741b','2026-11-05 23:59:31.414267','2026-11-05 23:59:31.414267'),('37aa69a4-04dc-49df-85c6-7bf45ca34ae1','f4d989c2-eae5-4295-9b98-61f4b0e61c11','c37c2597-b724-4334-9aa5-63c77fce4f11','ABSENT','DONE','Cần chủ động hơn','Ngoan, tập trung','Hoàn thành tốt nội dung Unit 2','e5127222-da47-4e25-8f9f-457e8a09741b','2026-11-05 23:59:31.414267','2026-11-05 23:59:31.414267'),('49c6e1e1-6633-4ba2-8c51-5b772de1f6b8','81541df0-5ace-487c-9569-0426c2138967','e499904a-3666-47c4-b356-9b3cff43d038','PRESENT','NOT_DONE','Tích cực phát biểu','Ngoan, tập trung','Hoàn thành tốt nội dung Unit 2','e5127222-da47-4e25-8f9f-457e8a09741b','2026-11-05 23:59:31.414267','2026-11-05 23:59:31.414267'),('4a675750-a2ed-4d44-acef-214de1f5f244','3ef4910d-b89a-4151-a941-f875c240814c','b7331574-8932-4acb-8469-ac82505e5c55','PRESENT','NOT_DONE','Tích cực phát biểu','Ngoan, tập trung','Hoàn thành tốt nội dung Unit 2','e5127222-da47-4e25-8f9f-457e8a09741b','2026-11-05 23:59:31.414267','2026-11-05 23:59:31.414267'),('4dc8c4c4-5076-45fe-bee6-1fb1a2aa4e03','e66b1258-0a92-47df-9bd9-03ecbddd2686','cd6fdfb8-9ce0-4e1e-8dec-13baf50481eb','PRESENT','DONE','Cần chủ động hơn','Ngoan, tập trung','Hoàn thành tốt nội dung Unit 1','e5127222-da47-4e25-8f9f-457e8a09741b','2026-11-05 23:59:31.414267','2026-11-05 23:59:31.414267'),('4f909b3f-6db2-4283-aff2-127ce5d48340','81541df0-5ace-487c-9569-0426c2138967','a7f623bc-58f6-487d-acff-fc7069fcd304','PRESENT','DONE','Cần chủ động hơn','Ngoan, tập trung','Hoàn thành tốt nội dung Unit 2','e5127222-da47-4e25-8f9f-457e8a09741b','2026-11-05 23:59:31.414267','2026-11-05 23:59:31.414267'),('5398874f-b626-4095-9d6d-c1a584616788','ca366e12-6c33-4653-b4ad-6488d4eb7960','a4861778-772e-42ee-9f52-3d93958e6ca1','PRESENT','DONE','Tích cực phát biểu','Ngoan, tập trung','Hoàn thành tốt nội dung Unit 1','e5127222-da47-4e25-8f9f-457e8a09741b','2026-11-05 23:59:31.414267','2026-11-05 23:59:31.414267'),('58175d27-644c-49db-aab2-19d6422bc3f3','c5de79bf-0f7e-45b7-b120-4bc1c5205d51','b7331574-8932-4acb-8469-ac82505e5c55','PRESENT','NOT_DONE','Cần chủ động hơn','Ngoan, tập trung','Hoàn thành tốt nội dung Unit 2','e5127222-da47-4e25-8f9f-457e8a09741b','2026-11-05 23:59:31.414267','2026-11-05 23:59:31.414267'),('6243b08d-1b0c-41bd-8fe9-85dfb1a84095','3ef4910d-b89a-4151-a941-f875c240814c','c37c2597-b724-4334-9aa5-63c77fce4f11','LATE','NOT_DONE','Cần chủ động hơn','Ngoan, tập trung','Hoàn thành tốt nội dung Unit 2','e5127222-da47-4e25-8f9f-457e8a09741b','2026-11-05 23:59:31.414267','2026-11-05 23:59:31.414267'),('67f6f872-31e9-4885-9997-9f05371d1038','81541df0-5ace-487c-9569-0426c2138967','c37c2597-b724-4334-9aa5-63c77fce4f11','PRESENT','DONE','Cần chủ động hơn','Ngoan, tập trung','Hoàn thành tốt nội dung Unit 2','e5127222-da47-4e25-8f9f-457e8a09741b','2026-11-05 23:59:31.414267','2026-11-05 23:59:31.414267'),('6be24877-a5f6-4904-aa02-2ebef68a7a0c','ca366e12-6c33-4653-b4ad-6488d4eb7960','a7f623bc-58f6-487d-acff-fc7069fcd304','ABSENT','DONE','Cần chủ động hơn','Ngoan, tập trung','Hoàn thành tốt nội dung Unit 1','e5127222-da47-4e25-8f9f-457e8a09741b','2026-11-05 23:59:31.414267','2026-11-05 23:59:31.414267'),('6d72ce96-528d-4b8c-956c-13a20f3318fe','3ef4910d-b89a-4151-a941-f875c240814c','a4861778-772e-42ee-9f52-3d93958e6ca1','PRESENT','DONE','Tích cực phát biểu','Ngoan, tập trung','Hoàn thành tốt nội dung Unit 2','e5127222-da47-4e25-8f9f-457e8a09741b','2026-11-05 23:59:31.414267','2026-11-05 23:59:31.414267'),('6d926e41-a380-4eaa-91e9-7d7405f4d924','8ba54d52-1c7e-4fc3-a1f1-ab0756d5aaab','e499904a-3666-47c4-b356-9b3cff43d038','PRESENT','DONE','Cần chủ động hơn','Ngoan, tập trung','Hoàn thành tốt nội dung Unit 1','e5127222-da47-4e25-8f9f-457e8a09741b','2026-11-05 23:59:31.414267','2026-11-05 23:59:31.414267'),('6f68f65b-3e87-4cab-a03b-dcea845fc158','8a1ef372-6137-4ea9-8d33-8c3b5c72248a','b7331574-8932-4acb-8469-ac82505e5c55','PRESENT','DONE','Cần chủ động hơn','Ngoan, tập trung','Hoàn thành tốt nội dung Unit 3','e5127222-da47-4e25-8f9f-457e8a09741b','2026-11-05 23:59:31.414267','2026-11-05 23:59:31.414267'),('73aec562-feb1-4cbf-92ca-ea234128b738','8ba54d52-1c7e-4fc3-a1f1-ab0756d5aaab','cd6fdfb8-9ce0-4e1e-8dec-13baf50481eb','PRESENT','DONE','Tích cực phát biểu','Ngoan, tập trung','Hoàn thành tốt nội dung Unit 1','e5127222-da47-4e25-8f9f-457e8a09741b','2026-11-05 23:59:31.414267','2026-11-05 23:59:31.414267'),('857ad861-4199-477c-83fe-ba77b9074849','ca366e12-6c33-4653-b4ad-6488d4eb7960','e499904a-3666-47c4-b356-9b3cff43d038','PRESENT','NOT_DONE','Tích cực phát biểu','Ngoan, tập trung','Hoàn thành tốt nội dung Unit 1','e5127222-da47-4e25-8f9f-457e8a09741b','2026-11-05 23:59:31.414267','2026-11-05 23:59:31.414267'),('86f4dd37-1900-4a2e-b5e1-6d16af8bc986','af70ed70-e76a-458c-9324-47b7c635f3ae','b7331574-8932-4acb-8469-ac82505e5c55','LATE','NOT_DONE','Cần chủ động hơn','Ngoan, tập trung','Hoàn thành tốt nội dung Unit 1','e5127222-da47-4e25-8f9f-457e8a09741b','2026-11-05 23:59:31.414267','2026-11-05 23:59:31.414267'),('900e97f5-2a33-487b-8200-ebd4685f407f','ca366e12-6c33-4653-b4ad-6488d4eb7960','b7331574-8932-4acb-8469-ac82505e5c55','PRESENT','DONE','Tích cực phát biểu','Ngoan, tập trung','Hoàn thành tốt nội dung Unit 1','e5127222-da47-4e25-8f9f-457e8a09741b','2026-11-05 23:59:31.414267','2026-11-05 23:59:31.414267'),('9abdddfa-21e2-4c88-a8dd-a82100f4ba25','c5de79bf-0f7e-45b7-b120-4bc1c5205d51','a4861778-772e-42ee-9f52-3d93958e6ca1','PRESENT','DONE','Cần chủ động hơn','Ngoan, tập trung','Hoàn thành tốt nội dung Unit 2','e5127222-da47-4e25-8f9f-457e8a09741b','2026-11-05 23:59:31.414267','2026-11-05 23:59:31.414267'),('9ff3a0c9-c74c-4e20-b0d1-97a5653ca008','c5de79bf-0f7e-45b7-b120-4bc1c5205d51','e499904a-3666-47c4-b356-9b3cff43d038','ABSENT','DONE','Cần chủ động hơn','Ngoan, tập trung','Hoàn thành tốt nội dung Unit 2','e5127222-da47-4e25-8f9f-457e8a09741b','2026-11-05 23:59:31.414267','2026-11-05 23:59:31.414267'),('a319bf26-b160-48b7-b2c1-167d392316a7','c5de79bf-0f7e-45b7-b120-4bc1c5205d51','cd6fdfb8-9ce0-4e1e-8dec-13baf50481eb','LATE','DONE','Tích cực phát biểu','Ngoan, tập trung','Hoàn thành tốt nội dung Unit 2','e5127222-da47-4e25-8f9f-457e8a09741b','2026-11-05 23:59:31.414267','2026-11-05 23:59:31.414267'),('a5d3258c-6b59-4953-8fc8-ae710e7fa54f','e66b1258-0a92-47df-9bd9-03ecbddd2686','a4861778-772e-42ee-9f52-3d93958e6ca1','PRESENT','NOT_DONE','Tích cực phát biểu','Ngoan, tập trung','Hoàn thành tốt nội dung Unit 1','e5127222-da47-4e25-8f9f-457e8a09741b','2026-11-05 23:59:31.414267','2026-11-05 23:59:31.414267'),('ad2f78fd-d139-4c55-8882-4a5565f72185','c5de79bf-0f7e-45b7-b120-4bc1c5205d51','c37c2597-b724-4334-9aa5-63c77fce4f11','PRESENT','NOT_DONE','Tích cực phát biểu','Ngoan, tập trung','Hoàn thành tốt nội dung Unit 2','e5127222-da47-4e25-8f9f-457e8a09741b','2026-11-05 23:59:31.414267','2026-11-05 23:59:31.414267'),('ae203258-4021-4a1a-add2-99d6394c38e0','8ba54d52-1c7e-4fc3-a1f1-ab0756d5aaab','a4861778-772e-42ee-9f52-3d93958e6ca1','LATE','NOT_DONE','Cần chủ động hơn','Ngoan, tập trung','Hoàn thành tốt nội dung Unit 1','e5127222-da47-4e25-8f9f-457e8a09741b','2026-11-05 23:59:31.414267','2026-11-05 23:59:31.414267'),('af0919f2-5217-4719-bc3c-90fffba467a7','dfb95fea-1db3-40cb-9723-bf654fd3a297','e499904a-3666-47c4-b356-9b3cff43d038','LATE','NOT_DONE','Cần chủ động hơn','Ngoan, tập trung','Hoàn thành tốt nội dung Unit 2','e5127222-da47-4e25-8f9f-457e8a09741b','2026-11-05 23:59:31.414267','2026-11-05 23:59:31.414267'),('b56a9722-3d25-432d-b02e-fa7c95281e92','81541df0-5ace-487c-9569-0426c2138967','b7331574-8932-4acb-8469-ac82505e5c55','PRESENT','DONE','Tích cực phát biểu','Ngoan, tập trung','Hoàn thành tốt nội dung Unit 2','e5127222-da47-4e25-8f9f-457e8a09741b','2026-11-05 23:59:31.414267','2026-11-05 23:59:31.414267'),('b58c9d3a-c1c3-4a39-b399-f6b11d5d9b58','ca366e12-6c33-4653-b4ad-6488d4eb7960','c37c2597-b724-4334-9aa5-63c77fce4f11','PRESENT','DONE','Cần chủ động hơn','Ngoan, tập trung','Hoàn thành tốt nội dung Unit 1','e5127222-da47-4e25-8f9f-457e8a09741b','2026-11-05 23:59:31.414267','2026-11-05 23:59:31.414267'),('b72c82cf-215d-4590-8956-08797bd26392','3ef4910d-b89a-4151-a941-f875c240814c','a7f623bc-58f6-487d-acff-fc7069fcd304','PRESENT','DONE','Cần chủ động hơn','Ngoan, tập trung','Hoàn thành tốt nội dung Unit 2','e5127222-da47-4e25-8f9f-457e8a09741b','2026-11-05 23:59:31.414267','2026-11-05 23:59:31.414267'),('c30bd894-71ea-450d-b81a-3d7fbcf73e5b','dfb95fea-1db3-40cb-9723-bf654fd3a297','a7f623bc-58f6-487d-acff-fc7069fcd304','LATE','DONE','Tích cực phát biểu','Ngoan, tập trung','Hoàn thành tốt nội dung Unit 2','e5127222-da47-4e25-8f9f-457e8a09741b','2026-11-05 23:59:31.414267','2026-11-05 23:59:31.414267'),('c509d52a-ce41-45ac-8350-2878b4eb0ef8','8a1ef372-6137-4ea9-8d33-8c3b5c72248a','a7f623bc-58f6-487d-acff-fc7069fcd304','ABSENT','NOT_DONE','Tích cực phát biểu','Ngoan, tập trung','Hoàn thành tốt nội dung Unit 3','e5127222-da47-4e25-8f9f-457e8a09741b','2026-11-05 23:59:31.414267','2026-11-05 23:59:31.414267'),('d595aec1-374d-4c86-9239-aae06bbf4d55','8a1ef372-6137-4ea9-8d33-8c3b5c72248a','a4861778-772e-42ee-9f52-3d93958e6ca1','PRESENT','NOT_DONE','Cần chủ động hơn','Ngoan, tập trung','Hoàn thành tốt nội dung Unit 3','e5127222-da47-4e25-8f9f-457e8a09741b','2026-11-05 23:59:31.414267','2026-11-05 23:59:31.414267'),('d5e1e6a7-7277-4e5b-b52a-9dc92eda8faf','8ba54d52-1c7e-4fc3-a1f1-ab0756d5aaab','b7331574-8932-4acb-8469-ac82505e5c55','ABSENT','DONE','Cần chủ động hơn','Ngoan, tập trung','Hoàn thành tốt nội dung Unit 1','e5127222-da47-4e25-8f9f-457e8a09741b','2026-11-05 23:59:31.414267','2026-11-05 23:59:31.414267'),('d81d178d-08da-4e28-a73d-09ed3c712fab','f4d989c2-eae5-4295-9b98-61f4b0e61c11','a7f623bc-58f6-487d-acff-fc7069fcd304','PRESENT','NOT_DONE','Cần chủ động hơn','Ngoan, tập trung','Hoàn thành tốt nội dung Unit 2','e5127222-da47-4e25-8f9f-457e8a09741b','2026-11-05 23:59:31.414267','2026-11-05 23:59:31.414267'),('ddd30138-eccf-4d02-8c3b-d8064a06ed4a','f4d989c2-eae5-4295-9b98-61f4b0e61c11','a4861778-772e-42ee-9f52-3d93958e6ca1','PRESENT','NOT_DONE','Tích cực phát biểu','Ngoan, tập trung','Hoàn thành tốt nội dung Unit 2','e5127222-da47-4e25-8f9f-457e8a09741b','2026-11-05 23:59:31.414267','2026-11-05 23:59:31.414267'),('e16ffb03-780f-4174-a3a2-5d31cc37e4cd','f4d989c2-eae5-4295-9b98-61f4b0e61c11','b7331574-8932-4acb-8469-ac82505e5c55','LATE','DONE','Tích cực phát biểu','Ngoan, tập trung','Hoàn thành tốt nội dung Unit 2','e5127222-da47-4e25-8f9f-457e8a09741b','2026-11-05 23:59:31.414267','2026-11-05 23:59:31.414267'),('e5b11ada-a4a2-4d45-be66-e77c09982036','3ef4910d-b89a-4151-a941-f875c240814c','cd6fdfb8-9ce0-4e1e-8dec-13baf50481eb','ABSENT','DONE','Cần chủ động hơn','Ngoan, tập trung','Hoàn thành tốt nội dung Unit 2','e5127222-da47-4e25-8f9f-457e8a09741b','2026-11-05 23:59:31.414267','2026-11-05 23:59:31.414267'),('e6ec7652-0c56-467b-aa05-009f397770c2','8a1ef372-6137-4ea9-8d33-8c3b5c72248a','c37c2597-b724-4334-9aa5-63c77fce4f11','LATE','DONE','Tích cực phát biểu','Ngoan, tập trung','Hoàn thành tốt nội dung Unit 3','e5127222-da47-4e25-8f9f-457e8a09741b','2026-11-05 23:59:31.414267','2026-11-05 23:59:31.414267'),('e771f537-99c5-4888-a5a9-0f87d18e9144','8ba54d52-1c7e-4fc3-a1f1-ab0756d5aaab','a7f623bc-58f6-487d-acff-fc7069fcd304','PRESENT','NOT_DONE','Tích cực phát biểu','Ngoan, tập trung','Hoàn thành tốt nội dung Unit 1','e5127222-da47-4e25-8f9f-457e8a09741b','2026-11-05 23:59:31.414267','2026-11-05 23:59:31.414267'),('e9d4ab1d-b81d-4678-9989-4002daf344b4','dfb95fea-1db3-40cb-9723-bf654fd3a297','b7331574-8932-4acb-8469-ac82505e5c55','PRESENT','DONE','Cần chủ động hơn','Ngoan, tập trung','Hoàn thành tốt nội dung Unit 2','e5127222-da47-4e25-8f9f-457e8a09741b','2026-11-05 23:59:31.414267','2026-11-05 23:59:31.414267'),('ea020317-6237-4523-b4ee-c9ea028c5e08','3ef4910d-b89a-4151-a941-f875c240814c','e499904a-3666-47c4-b356-9b3cff43d038','PRESENT','DONE','Tích cực phát biểu','Ngoan, tập trung','Hoàn thành tốt nội dung Unit 2','e5127222-da47-4e25-8f9f-457e8a09741b','2026-11-05 23:59:31.414267','2026-11-05 23:59:31.414267'),('ead8d81d-e159-49ef-895e-139454839d57','8ba54d52-1c7e-4fc3-a1f1-ab0756d5aaab','c37c2597-b724-4334-9aa5-63c77fce4f11','PRESENT','DONE','Tích cực phát biểu','Ngoan, tập trung','Hoàn thành tốt nội dung Unit 1','e5127222-da47-4e25-8f9f-457e8a09741b','2026-11-05 23:59:31.414267','2026-11-05 23:59:31.414267'),('eb265960-cbc9-4e94-a900-dd2bdcd00c01','af70ed70-e76a-458c-9324-47b7c635f3ae','a4861778-772e-42ee-9f52-3d93958e6ca1','PRESENT','DONE','Cần chủ động hơn','Ngoan, tập trung','Hoàn thành tốt nội dung Unit 1','e5127222-da47-4e25-8f9f-457e8a09741b','2026-11-05 23:59:31.414267','2026-11-05 23:59:31.414267'),('ed2522d5-7ba6-4a8c-a1e8-aea1420b0065','f4d989c2-eae5-4295-9b98-61f4b0e61c11','cd6fdfb8-9ce0-4e1e-8dec-13baf50481eb','PRESENT','DONE','Cần chủ động hơn','Ngoan, tập trung','Hoàn thành tốt nội dung Unit 2','e5127222-da47-4e25-8f9f-457e8a09741b','2026-11-05 23:59:31.414267','2026-11-05 23:59:31.414267'),('f43d2fda-8aec-4a42-925f-5366c5445bc5','ca366e12-6c33-4653-b4ad-6488d4eb7960','cd6fdfb8-9ce0-4e1e-8dec-13baf50481eb','LATE','NOT_DONE','Cần chủ động hơn','Ngoan, tập trung','Hoàn thành tốt nội dung Unit 1','e5127222-da47-4e25-8f9f-457e8a09741b','2026-11-05 23:59:31.414267','2026-11-05 23:59:31.414267'),('f9b6528b-4116-4479-a5bc-1cf1bf7b10e9','af70ed70-e76a-458c-9324-47b7c635f3ae','a7f623bc-58f6-487d-acff-fc7069fcd304','PRESENT','DONE','Tích cực phát biểu','Ngoan, tập trung','Hoàn thành tốt nội dung Unit 1','e5127222-da47-4e25-8f9f-457e8a09741b','2026-11-05 23:59:31.414267','2026-11-05 23:59:31.414267');
/*!40000 ALTER TABLE `session_feedback` ENABLE KEYS */;
DROP TABLE IF EXISTS `session_scores`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `session_scores` (
  `id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `session_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `student_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `score_type` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `listening` int DEFAULT NULL,
  `reading` int DEFAULT NULL,
  `writing` int DEFAULT NULL,
  `speaking` int DEFAULT NULL,
  `total` int NOT NULL,
  `created_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `updated_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  PRIMARY KEY (`id`),
  UNIQUE KEY `ix_session_scores_session_id_student_id` (`session_id`,`student_id`),
  KEY `ix_session_scores_student_id` (`student_id`),
  CONSTRAINT `fk_session_scores_sessions_session_id` FOREIGN KEY (`session_id`) REFERENCES `sessions` (`id`) ON DELETE RESTRICT,
  CONSTRAINT `fk_session_scores_students_student_id` FOREIGN KEY (`student_id`) REFERENCES `students` (`id`) ON DELETE RESTRICT,
  CONSTRAINT `ck_session_scores_listening` CHECK ((`listening` between 0 and 100)),
  CONSTRAINT `ck_session_scores_reading` CHECK ((`reading` between 0 and 100)),
  CONSTRAINT `ck_session_scores_score_type` CHECK ((`score_type` in (_utf8mb4'TEST',_utf8mb4'MIDTERM',_utf8mb4'FINAL'))),
  CONSTRAINT `ck_session_scores_speaking` CHECK ((`speaking` between 0 and 100)),
  CONSTRAINT `ck_session_scores_total` CHECK ((`total` between 0 and 100)),
  CONSTRAINT `ck_session_scores_writing` CHECK ((`writing` between 0 and 100))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

/*!40000 ALTER TABLE `session_scores` DISABLE KEYS */;
INSERT INTO `session_scores` (`id`, `session_id`, `student_id`, `score_type`, `listening`, `reading`, `writing`, `speaking`, `total`, `created_at`, `updated_at`) VALUES ('7e449ca3-d0b7-480a-8f25-c67ad9a95c87','81541df0-5ace-487c-9569-0426c2138967','cd6fdfb8-9ce0-4e1e-8dec-13baf50481eb','TEST',81,76,71,86,79,'2026-11-05 23:59:31.414267','2026-11-05 23:59:31.414267'),('9f593e6e-4f06-4681-bace-960adda98329','81541df0-5ace-487c-9569-0426c2138967','a4861778-772e-42ee-9f52-3d93958e6ca1','TEST',80,75,70,85,78,'2026-11-05 23:59:31.414267','2026-11-05 23:59:31.414267'),('a8af4019-55ff-4c42-a85f-59dc49156622','81541df0-5ace-487c-9569-0426c2138967','e499904a-3666-47c4-b356-9b3cff43d038','TEST',78,73,68,83,76,'2026-11-05 23:59:31.414267','2026-11-05 23:59:31.414267'),('b9ae0e3e-bfc3-473d-9b51-34d5fc4f5ce6','81541df0-5ace-487c-9569-0426c2138967','a7f623bc-58f6-487d-acff-fc7069fcd304','TEST',83,78,73,88,81,'2026-11-05 23:59:31.414267','2026-11-05 23:59:31.414267'),('ce042481-79ae-4418-9995-6b42178814d1','81541df0-5ace-487c-9569-0426c2138967','c37c2597-b724-4334-9aa5-63c77fce4f11','TEST',79,74,69,84,77,'2026-11-05 23:59:31.414267','2026-11-05 23:59:31.414267'),('e9e2d257-20b2-4cbb-b490-2dd49157afe3','81541df0-5ace-487c-9569-0426c2138967','b7331574-8932-4acb-8469-ac82505e5c55','TEST',82,77,72,87,80,'2026-11-05 23:59:31.414267','2026-11-05 23:59:31.414267');
/*!40000 ALTER TABLE `session_scores` ENABLE KEYS */;
DROP TABLE IF EXISTS `sessions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `sessions` (
  `id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `class_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `session_date` date NOT NULL,
  `unit_no` int NOT NULL,
  `lesson_no` int NOT NULL,
  `session_type` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `main_teacher_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `cover_teacher_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `created_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  PRIMARY KEY (`id`),
  KEY `ix_sessions_class_id_session_date` (`class_id`,`session_date`),
  KEY `ix_sessions_cover_teacher_id` (`cover_teacher_id`),
  KEY `ix_sessions_main_teacher_id` (`main_teacher_id`),
  KEY `ix_sessions_session_date` (`session_date`),
  CONSTRAINT `fk_sessions_classes_class_id` FOREIGN KEY (`class_id`) REFERENCES `classes` (`id`) ON DELETE RESTRICT,
  CONSTRAINT `fk_sessions_users_cover_teacher_id` FOREIGN KEY (`cover_teacher_id`) REFERENCES `auth_users` (`id`) ON DELETE RESTRICT,
  CONSTRAINT `fk_sessions_users_main_teacher_id` FOREIGN KEY (`main_teacher_id`) REFERENCES `auth_users` (`id`) ON DELETE RESTRICT,
  CONSTRAINT `ck_sessions_session_type` CHECK ((`session_type` in (_utf8mb4'NORMAL',_utf8mb4'TEST',_utf8mb4'MIDTERM',_utf8mb4'FINAL')))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

/*!40000 ALTER TABLE `sessions` DISABLE KEYS */;
INSERT INTO `sessions` (`id`, `class_id`, `session_date`, `unit_no`, `lesson_no`, `session_type`, `main_teacher_id`, `cover_teacher_id`, `created_at`) VALUES ('01874380-a157-442b-83fb-20e945a2b01e','58abc75f-b941-404e-a206-74af63fc618e','2027-03-25',12,1,'NORMAL','e5127222-da47-4e25-8f9f-457e8a09741b',NULL,'2026-11-05 23:59:32.151855'),('04093998-5356-41c7-9961-c1595da6d916','80e1a826-97f8-433d-9705-7a95ed8a93e3','2026-11-25',1,6,'NORMAL','1939cd58-d478-4c54-ba6e-f7fa50b8672c',NULL,'2026-11-05 23:59:32.154096'),('04554794-b6b4-42d3-8f71-e967dd747698','58abc75f-b941-404e-a206-74af63fc618e','2026-12-31',6,6,'NORMAL','e5127222-da47-4e25-8f9f-457e8a09741b',NULL,'2026-11-05 23:59:32.151784'),('069d3591-480e-4253-8c1b-3ef7d87efac1','58abc75f-b941-404e-a206-74af63fc618e','2026-12-03',4,7,'TEST','e5127222-da47-4e25-8f9f-457e8a09741b',NULL,'2026-11-05 23:59:32.151763'),('06c4c437-5d19-4e1e-ba49-acc4a9515f47','58abc75f-b941-404e-a206-74af63fc618e','2026-11-17',3,6,'NORMAL','e5127222-da47-4e25-8f9f-457e8a09741b',NULL,'2026-11-05 23:59:32.151750'),('07aa1e2a-02d8-436d-a97f-7e6f5237a8c8','80e1a826-97f8-433d-9705-7a95ed8a93e3','2027-01-04',4,3,'NORMAL','1939cd58-d478-4c54-ba6e-f7fa50b8672c',NULL,'2026-11-05 23:59:32.154123'),('08808d58-2918-447b-8492-5cdc2f76cc66','80e1a826-97f8-433d-9705-7a95ed8a93e3','2027-01-25',5,4,'NORMAL','1939cd58-d478-4c54-ba6e-f7fa50b8672c',NULL,'2026-11-05 23:59:32.154139'),('08915c2b-b6a7-401e-a4c1-b21a2ca35fc6','58abc75f-b941-404e-a206-74af63fc618e','2027-02-09',9,1,'NORMAL','e5127222-da47-4e25-8f9f-457e8a09741b',NULL,'2026-11-05 23:59:32.151820'),('0900533a-2a98-4b58-ab3f-6732c3b84c91','58abc75f-b941-404e-a206-74af63fc618e','2027-03-02',10,4,'NORMAL','e5127222-da47-4e25-8f9f-457e8a09741b',NULL,'2026-11-05 23:59:32.151837'),('0dcbca82-8b92-4cde-9ff4-310400e59587','80e1a826-97f8-433d-9705-7a95ed8a93e3','2027-03-08',8,3,'NORMAL','1939cd58-d478-4c54-ba6e-f7fa50b8672c',NULL,'2026-11-05 23:59:32.154168'),('11d55788-f1d7-4cc3-9b22-68413c39cf2a','58abc75f-b941-404e-a206-74af63fc618e','2027-02-04',8,7,'TEST','e5127222-da47-4e25-8f9f-457e8a09741b',NULL,'2026-11-05 23:59:32.151818'),('136a6844-ab8d-429d-8dd5-a2498c90b958','80e1a826-97f8-433d-9705-7a95ed8a93e3','2027-03-03',8,1,'NORMAL','1939cd58-d478-4c54-ba6e-f7fa50b8672c',NULL,'2026-11-05 23:59:32.154166'),('13e14d75-536c-4156-ae1e-82d10087656e','80e1a826-97f8-433d-9705-7a95ed8a93e3','2027-05-10',12,3,'NORMAL','1939cd58-d478-4c54-ba6e-f7fa50b8672c',NULL,'2026-11-05 23:59:32.154214'),('18441049-4083-482a-9464-393a4c746b74','58abc75f-b941-404e-a206-74af63fc618e','2027-03-09',10,7,'TEST','e5127222-da47-4e25-8f9f-457e8a09741b',NULL,'2026-11-05 23:59:32.151842'),('1b002f44-eada-4a0f-9e26-698266b56c14','58abc75f-b941-404e-a206-74af63fc618e','2027-01-14',7,4,'NORMAL','e5127222-da47-4e25-8f9f-457e8a09741b',NULL,'2026-11-05 23:59:32.151802'),('1e51c9e6-98b0-4d66-a67a-585c4be13b9b','58abc75f-b941-404e-a206-74af63fc618e','2026-12-24',6,3,'NORMAL','e5127222-da47-4e25-8f9f-457e8a09741b',NULL,'2026-11-05 23:59:32.151779'),('22ab9d62-1644-4e6b-8f5e-33082432cc22','58abc75f-b941-404e-a206-74af63fc618e','2027-02-23',10,1,'NORMAL','e5127222-da47-4e25-8f9f-457e8a09741b',NULL,'2026-11-05 23:59:32.151831'),('23b04d9f-f21f-48d0-af14-66f57ba4a81e','58abc75f-b941-404e-a206-74af63fc618e','2026-11-10',3,3,'NORMAL','e5127222-da47-4e25-8f9f-457e8a09741b',NULL,'2026-11-05 23:59:32.151745'),('26ce0d8f-1ee3-4712-bd56-d61e3e3419a8','58abc75f-b941-404e-a206-74af63fc618e','2027-03-23',11,6,'NORMAL','e5127222-da47-4e25-8f9f-457e8a09741b',NULL,'2026-11-05 23:59:32.151852'),('293454d5-c420-469f-a3ee-e9e4387b4221','80e1a826-97f8-433d-9705-7a95ed8a93e3','2026-11-18',1,3,'NORMAL','1939cd58-d478-4c54-ba6e-f7fa50b8672c',NULL,'2026-11-05 23:59:32.154091'),('29aacc2b-dd63-471a-9ed6-4aa270de818f','58abc75f-b941-404e-a206-74af63fc618e','2026-12-01',4,6,'NORMAL','e5127222-da47-4e25-8f9f-457e8a09741b',NULL,'2026-11-05 23:59:32.151760'),('29dddaca-cb37-4cdd-ad91-213bd5457370','80e1a826-97f8-433d-9705-7a95ed8a93e3','2027-02-24',7,4,'NORMAL','1939cd58-d478-4c54-ba6e-f7fa50b8672c',NULL,'2026-11-05 23:59:32.154161'),('2ced9091-b33d-40e7-94e8-19c10c763347','80e1a826-97f8-433d-9705-7a95ed8a93e3','2027-04-07',10,3,'NORMAL','1939cd58-d478-4c54-ba6e-f7fa50b8672c',NULL,'2026-11-05 23:59:32.154192'),('2fa84521-d229-4497-9379-fd88fa6f7732','58abc75f-b941-404e-a206-74af63fc618e','2027-02-02',8,6,'NORMAL','e5127222-da47-4e25-8f9f-457e8a09741b',NULL,'2026-11-05 23:59:32.151816'),('30623cbf-9689-4aef-ad35-6abfc5b4cf68','80e1a826-97f8-433d-9705-7a95ed8a93e3','2027-01-06',4,4,'NORMAL','1939cd58-d478-4c54-ba6e-f7fa50b8672c',NULL,'2026-11-05 23:59:32.154126'),('31d1e02d-e47c-4e3e-b0d3-4b2ea90504bc','80e1a826-97f8-433d-9705-7a95ed8a93e3','2027-05-17',12,6,'NORMAL','1939cd58-d478-4c54-ba6e-f7fa50b8672c',NULL,'2026-11-05 23:59:32.154219'),('3388d1b3-ccf6-468c-a7c0-12e5a45a735f','58abc75f-b941-404e-a206-74af63fc618e','2027-03-16',11,3,'NORMAL','e5127222-da47-4e25-8f9f-457e8a09741b',NULL,'2026-11-05 23:59:32.151847'),('33c05f8a-1838-4042-aab6-9b37dc2f27d5','80e1a826-97f8-433d-9705-7a95ed8a93e3','2027-05-03',11,6,'NORMAL','1939cd58-d478-4c54-ba6e-f7fa50b8672c',NULL,'2026-11-05 23:59:32.154209'),('34d56057-042d-4324-9c6b-83d05bf63e09','80e1a826-97f8-433d-9705-7a95ed8a93e3','2027-04-14',10,6,'NORMAL','1939cd58-d478-4c54-ba6e-f7fa50b8672c',NULL,'2026-11-05 23:59:32.154197'),('35b5d23c-4d28-4bd0-8e82-3b62e6d49538','80e1a826-97f8-433d-9705-7a95ed8a93e3','2026-12-16',3,1,'NORMAL','1939cd58-d478-4c54-ba6e-f7fa50b8672c',NULL,'2026-11-05 23:59:32.154111'),('37e09e04-9c67-4ae3-b74f-42a1eb505f00','80e1a826-97f8-433d-9705-7a95ed8a93e3','2027-03-31',9,6,'NORMAL','1939cd58-d478-4c54-ba6e-f7fa50b8672c',NULL,'2026-11-05 23:59:32.154187'),('3c4846f3-438d-4b2c-aa51-2aded4d28b18','58abc75f-b941-404e-a206-74af63fc618e','2027-04-06',12,6,'NORMAL','e5127222-da47-4e25-8f9f-457e8a09741b',NULL,'2026-11-05 23:59:32.151862'),('3d1bccaa-783a-4f1d-9b00-a42b17981119','58abc75f-b941-404e-a206-74af63fc618e','2027-01-12',7,3,'NORMAL','e5127222-da47-4e25-8f9f-457e8a09741b',NULL,'2026-11-05 23:59:32.151792'),('3d420a47-8a9d-4d9c-b5e3-39f4219aaf4b','80e1a826-97f8-433d-9705-7a95ed8a93e3','2027-05-19',12,7,'FINAL','1939cd58-d478-4c54-ba6e-f7fa50b8672c',NULL,'2026-11-05 23:59:32.154222'),('3e7cf4db-40d9-40da-87c0-2b8e1d64c051','80e1a826-97f8-433d-9705-7a95ed8a93e3','2027-02-03',6,3,'NORMAL','1939cd58-d478-4c54-ba6e-f7fa50b8672c',NULL,'2026-11-05 23:59:32.154146'),('3e88123c-c8a7-476b-9bb6-63af57388b40','80e1a826-97f8-433d-9705-7a95ed8a93e3','2026-11-23',1,4,'NORMAL','1939cd58-d478-4c54-ba6e-f7fa50b8672c',NULL,'2026-11-05 23:59:32.154093'),('3ef4910d-b89a-4151-a941-f875c240814c','58abc75f-b941-404e-a206-74af63fc618e','2026-10-20',2,1,'NORMAL','e5127222-da47-4e25-8f9f-457e8a09741b',NULL,'2026-11-05 23:59:32.151728'),('3fc003de-8898-4c60-9e4c-cb8e26cd9bec','58abc75f-b941-404e-a206-74af63fc618e','2027-01-26',8,3,'NORMAL','e5127222-da47-4e25-8f9f-457e8a09741b',NULL,'2026-11-05 23:59:32.151810'),('406fed35-0822-424b-b8db-7b2e48897e79','80e1a826-97f8-433d-9705-7a95ed8a93e3','2026-12-14',2,7,'TEST','1939cd58-d478-4c54-ba6e-f7fa50b8672c',NULL,'2026-11-05 23:59:32.154109'),('40f5bcd7-b1df-4151-a65a-b237a09d6f02','80e1a826-97f8-433d-9705-7a95ed8a93e3','2026-12-07',2,4,'NORMAL','1939cd58-d478-4c54-ba6e-f7fa50b8672c',NULL,'2026-11-05 23:59:32.154104'),('4268522d-3a19-4922-8517-9310f83aa55c','58abc75f-b941-404e-a206-74af63fc618e','2026-11-19',4,1,'NORMAL','e5127222-da47-4e25-8f9f-457e8a09741b',NULL,'2026-11-05 23:59:32.151752'),('4714fe1c-5782-4cd7-899b-868963eba9a7','80e1a826-97f8-433d-9705-7a95ed8a93e3','2027-05-05',12,1,'NORMAL','1939cd58-d478-4c54-ba6e-f7fa50b8672c',NULL,'2026-11-05 23:59:32.154212'),('4b0cc76d-4663-4984-9b08-f3e6cd05ff83','58abc75f-b941-404e-a206-74af63fc618e','2027-03-18',11,4,'NORMAL','e5127222-da47-4e25-8f9f-457e8a09741b',NULL,'2026-11-05 23:59:32.151850'),('4f027559-34bd-40c8-8c9b-6b7decec041f','80e1a826-97f8-433d-9705-7a95ed8a93e3','2027-01-11',4,6,'NORMAL','1939cd58-d478-4c54-ba6e-f7fa50b8672c',NULL,'2026-11-05 23:59:32.154128'),('51d4a172-c583-49eb-b3ea-98acee6e4cfb','58abc75f-b941-404e-a206-74af63fc618e','2027-02-11',9,3,'NORMAL','e5127222-da47-4e25-8f9f-457e8a09741b',NULL,'2026-11-05 23:59:32.151823'),('54454ad0-000e-40b2-93cd-986de54adfb8','58abc75f-b941-404e-a206-74af63fc618e','2027-02-16',9,4,'NORMAL','e5127222-da47-4e25-8f9f-457e8a09741b',NULL,'2026-11-05 23:59:32.151825'),('604b9089-8874-4d7f-90b0-6b83d7cab15d','80e1a826-97f8-433d-9705-7a95ed8a93e3','2027-01-18',5,1,'NORMAL','1939cd58-d478-4c54-ba6e-f7fa50b8672c',NULL,'2026-11-05 23:59:32.154134'),('658e289b-fd68-41d2-9e08-968f11351fab','80e1a826-97f8-433d-9705-7a95ed8a93e3','2026-12-09',2,6,'NORMAL','1939cd58-d478-4c54-ba6e-f7fa50b8672c',NULL,'2026-11-05 23:59:32.154106'),('67204d01-bb53-4a87-ad7b-5e8b1f4f5fc9','58abc75f-b941-404e-a206-74af63fc618e','2026-11-12',3,4,'NORMAL','e5127222-da47-4e25-8f9f-457e8a09741b',NULL,'2026-11-05 23:59:32.151748'),('6bf40261-6f9d-4a39-bb11-530bc50a66e9','80e1a826-97f8-433d-9705-7a95ed8a93e3','2027-02-17',7,1,'NORMAL','1939cd58-d478-4c54-ba6e-f7fa50b8672c',NULL,'2026-11-05 23:59:32.154156'),('6c3362c5-d209-4d88-a804-a830cfd4590f','58abc75f-b941-404e-a206-74af63fc618e','2027-01-21',8,1,'NORMAL','e5127222-da47-4e25-8f9f-457e8a09741b',NULL,'2026-11-05 23:59:32.151808'),('71431147-2d3d-4208-b45e-577a2a770800','80e1a826-97f8-433d-9705-7a95ed8a93e3','2027-01-27',5,6,'NORMAL','1939cd58-d478-4c54-ba6e-f7fa50b8672c',NULL,'2026-11-05 23:59:32.154141'),('727f6c7b-ca10-482e-b4f0-9a8604633f55','80e1a826-97f8-433d-9705-7a95ed8a93e3','2027-04-28',11,4,'NORMAL','1939cd58-d478-4c54-ba6e-f7fa50b8672c',NULL,'2026-11-05 23:59:32.154207'),('730b27e6-b4c4-4e13-a38e-b813617869bf','58abc75f-b941-404e-a206-74af63fc618e','2027-04-01',12,4,'NORMAL','e5127222-da47-4e25-8f9f-457e8a09741b',NULL,'2026-11-05 23:59:32.151859'),('75f7790d-3ac8-4076-9712-67d1b1e3e472','58abc75f-b941-404e-a206-74af63fc618e','2027-01-19',7,6,'NORMAL','e5127222-da47-4e25-8f9f-457e8a09741b',NULL,'2026-11-05 23:59:32.151805'),('77303b63-2afd-4e73-b460-527d107a188d','80e1a826-97f8-433d-9705-7a95ed8a93e3','2027-03-17',8,7,'TEST','1939cd58-d478-4c54-ba6e-f7fa50b8672c',NULL,'2026-11-05 23:59:32.154177'),('7757c2e6-58e2-42e3-abd3-9a347f45d2d0','58abc75f-b941-404e-a206-74af63fc618e','2026-12-15',5,4,'NORMAL','e5127222-da47-4e25-8f9f-457e8a09741b',NULL,'2026-11-05 23:59:32.151772'),('79632d84-bfde-43f5-9341-dd70c68879df','58abc75f-b941-404e-a206-74af63fc618e','2026-11-24',4,3,'NORMAL','e5127222-da47-4e25-8f9f-457e8a09741b',NULL,'2026-11-05 23:59:32.151755'),('7d4332a8-fd48-4b44-8bb3-9b92e6ff74c8','80e1a826-97f8-433d-9705-7a95ed8a93e3','2027-03-24',9,3,'NORMAL','1939cd58-d478-4c54-ba6e-f7fa50b8672c',NULL,'2026-11-05 23:59:32.154182'),('7efa2c0a-8df2-459b-b128-0415e89c41ab','58abc75f-b941-404e-a206-74af63fc618e','2026-12-22',6,1,'NORMAL','e5127222-da47-4e25-8f9f-457e8a09741b',NULL,'2026-11-05 23:59:32.151777'),('81541df0-5ace-487c-9569-0426c2138967','58abc75f-b941-404e-a206-74af63fc618e','2026-11-03',2,7,'TEST','e5127222-da47-4e25-8f9f-457e8a09741b',NULL,'2026-11-05 23:59:32.151738'),('819b5c43-9d93-47b2-b9fd-2de7e89dd966','80e1a826-97f8-433d-9705-7a95ed8a93e3','2027-04-05',10,1,'NORMAL','1939cd58-d478-4c54-ba6e-f7fa50b8672c',NULL,'2026-11-05 23:59:32.154189'),('84960f60-0492-4582-99e5-28d1370b1797','80e1a826-97f8-433d-9705-7a95ed8a93e3','2027-04-26',11,3,'NORMAL','1939cd58-d478-4c54-ba6e-f7fa50b8672c',NULL,'2026-11-05 23:59:32.154204'),('84cb2cb5-fbb6-4cc9-8447-714e3906ee8d','80e1a826-97f8-433d-9705-7a95ed8a93e3','2027-04-19',10,7,'TEST','1939cd58-d478-4c54-ba6e-f7fa50b8672c',NULL,'2026-11-05 23:59:32.154199'),('85214595-8a94-4241-9381-41c224f1fcc7','58abc75f-b941-404e-a206-74af63fc618e','2026-12-17',5,6,'NORMAL','e5127222-da47-4e25-8f9f-457e8a09741b',NULL,'2026-11-05 23:59:32.151774'),('88eea661-40de-4c95-939f-12cdcaf00b26','58abc75f-b941-404e-a206-74af63fc618e','2026-12-10',5,3,'NORMAL','e5127222-da47-4e25-8f9f-457e8a09741b',NULL,'2026-11-05 23:59:32.151769'),('89921033-8f1c-469e-afbf-6dbf9b645922','58abc75f-b941-404e-a206-74af63fc618e','2027-01-07',7,1,'NORMAL','e5127222-da47-4e25-8f9f-457e8a09741b',NULL,'2026-11-05 23:59:32.151790'),('8a1ef372-6137-4ea9-8d33-8c3b5c72248a','58abc75f-b941-404e-a206-74af63fc618e','2026-11-05',3,1,'NORMAL','e5127222-da47-4e25-8f9f-457e8a09741b',NULL,'2026-11-05 23:59:32.151743'),('8a63f43d-70a3-4bfc-ba67-97414db711b6','58abc75f-b941-404e-a206-74af63fc618e','2026-12-29',6,4,'NORMAL','e5127222-da47-4e25-8f9f-457e8a09741b',NULL,'2026-11-05 23:59:32.151782'),('8aba6f68-0315-4348-bd0a-b074cced3b69','80e1a826-97f8-433d-9705-7a95ed8a93e3','2027-03-29',9,4,'NORMAL','1939cd58-d478-4c54-ba6e-f7fa50b8672c',NULL,'2026-11-05 23:59:32.154184'),('8ba54d52-1c7e-4fc3-a1f1-ab0756d5aaab','58abc75f-b941-404e-a206-74af63fc618e','2026-10-15',1,6,'NORMAL','e5127222-da47-4e25-8f9f-457e8a09741b',NULL,'2026-11-05 23:59:32.151725'),('8c4f32f4-c772-4731-a4c5-26b1ee539c42','80e1a826-97f8-433d-9705-7a95ed8a93e3','2027-01-13',4,7,'TEST','1939cd58-d478-4c54-ba6e-f7fa50b8672c',NULL,'2026-11-05 23:59:32.154131'),('953d1939-457c-4d8f-b590-f4d07edadcad','58abc75f-b941-404e-a206-74af63fc618e','2027-03-30',12,3,'NORMAL','e5127222-da47-4e25-8f9f-457e8a09741b',NULL,'2026-11-05 23:59:32.151857'),('9aa65523-21d7-4fc9-b3a2-42d3861de338','58abc75f-b941-404e-a206-74af63fc618e','2027-02-25',10,3,'NORMAL','e5127222-da47-4e25-8f9f-457e8a09741b',NULL,'2026-11-05 23:59:32.151834'),('9b911f0b-edc5-48e2-b5f4-e57303edfe7b','80e1a826-97f8-433d-9705-7a95ed8a93e3','2026-12-23',3,4,'NORMAL','1939cd58-d478-4c54-ba6e-f7fa50b8672c',NULL,'2026-11-05 23:59:32.154116'),('9e0eb7b1-32c9-4b32-add4-27b576b23f01','80e1a826-97f8-433d-9705-7a95ed8a93e3','2027-02-10',6,6,'NORMAL','1939cd58-d478-4c54-ba6e-f7fa50b8672c',NULL,'2026-11-05 23:59:32.154151'),('9f975241-23c4-4c82-bd69-0f30c1b354dd','80e1a826-97f8-433d-9705-7a95ed8a93e3','2026-12-02',2,3,'NORMAL','1939cd58-d478-4c54-ba6e-f7fa50b8672c',NULL,'2026-11-05 23:59:32.154101'),('a18e1c33-1295-415a-bb54-717b6571d9bf','58abc75f-b941-404e-a206-74af63fc618e','2027-02-18',9,6,'NORMAL','e5127222-da47-4e25-8f9f-457e8a09741b',NULL,'2026-11-05 23:59:32.151828'),('a19118a2-f8ad-4d29-83e8-6c184a81c8e0','58abc75f-b941-404e-a206-74af63fc618e','2026-12-08',5,1,'NORMAL','e5127222-da47-4e25-8f9f-457e8a09741b',NULL,'2026-11-05 23:59:32.151767'),('a321a3c7-576f-49cd-970e-d0a283b7c319','80e1a826-97f8-433d-9705-7a95ed8a93e3','2027-05-12',12,4,'NORMAL','1939cd58-d478-4c54-ba6e-f7fa50b8672c',NULL,'2026-11-05 23:59:32.154217'),('aa2e8221-e260-45b3-b96b-eece8b4ff3a5','58abc75f-b941-404e-a206-74af63fc618e','2027-04-08',12,7,'FINAL','e5127222-da47-4e25-8f9f-457e8a09741b',NULL,'2026-11-05 23:59:32.151864'),('add0f0fd-b1f0-479d-98cb-212a1c3a61f4','80e1a826-97f8-433d-9705-7a95ed8a93e3','2027-04-21',11,1,'NORMAL','1939cd58-d478-4c54-ba6e-f7fa50b8672c',NULL,'2026-11-05 23:59:32.154202'),('af70ed70-e76a-458c-9324-47b7c635f3ae','58abc75f-b941-404e-a206-74af63fc618e','2026-10-08',1,3,'NORMAL','e5127222-da47-4e25-8f9f-457e8a09741b',NULL,'2026-11-05 23:59:32.151721'),('b1861a1a-40bc-4b10-a7c3-2ea299508f0a','58abc75f-b941-404e-a206-74af63fc618e','2027-01-05',6,7,'MIDTERM','e5127222-da47-4e25-8f9f-457e8a09741b',NULL,'2026-11-05 23:59:32.151787'),('b6320857-eeeb-412f-b1b4-44deff6817a0','80e1a826-97f8-433d-9705-7a95ed8a93e3','2026-12-21',3,3,'NORMAL','1939cd58-d478-4c54-ba6e-f7fa50b8672c',NULL,'2026-11-05 23:59:32.154114'),('b8cfea4e-6b6d-49fa-b20e-491bd1123c52','80e1a826-97f8-433d-9705-7a95ed8a93e3','2026-11-30',2,1,'NORMAL','1939cd58-d478-4c54-ba6e-f7fa50b8672c',NULL,'2026-11-05 23:59:32.154098'),('c05fdfdb-c4ce-4e95-bca7-fbec95b75c0b','58abc75f-b941-404e-a206-74af63fc618e','2027-01-28',8,4,'NORMAL','e5127222-da47-4e25-8f9f-457e8a09741b',NULL,'2026-11-05 23:59:32.151813'),('c1695acd-a658-4667-96a8-76ac5cc38a88','58abc75f-b941-404e-a206-74af63fc618e','2027-03-11',11,1,'NORMAL','e5127222-da47-4e25-8f9f-457e8a09741b',NULL,'2026-11-05 23:59:32.151845'),('c4624611-a420-4676-8852-c80f4b03061c','80e1a826-97f8-433d-9705-7a95ed8a93e3','2027-01-20',5,3,'NORMAL','1939cd58-d478-4c54-ba6e-f7fa50b8672c',NULL,'2026-11-05 23:59:32.154136'),('c56597f4-2f69-4f12-a6c7-ef40dcb9d14a','80e1a826-97f8-433d-9705-7a95ed8a93e3','2027-02-01',6,1,'NORMAL','1939cd58-d478-4c54-ba6e-f7fa50b8672c',NULL,'2026-11-05 23:59:32.154144'),('c5de79bf-0f7e-45b7-b120-4bc1c5205d51','58abc75f-b941-404e-a206-74af63fc618e','2026-10-29',2,6,'NORMAL','e5127222-da47-4e25-8f9f-457e8a09741b',NULL,'2026-11-05 23:59:32.151736'),('c6ab5db9-f51d-47ea-b3cd-27f04d819bf9','80e1a826-97f8-433d-9705-7a95ed8a93e3','2027-03-15',8,6,'NORMAL','1939cd58-d478-4c54-ba6e-f7fa50b8672c',NULL,'2026-11-05 23:59:32.154174'),('ca366e12-6c33-4653-b4ad-6488d4eb7960','58abc75f-b941-404e-a206-74af63fc618e','2026-10-13',1,4,'NORMAL','e5127222-da47-4e25-8f9f-457e8a09741b',NULL,'2026-11-05 23:59:32.151723'),('cd5b715b-3db0-4557-973e-34c0ba6f17a6','58abc75f-b941-404e-a206-74af63fc618e','2027-03-04',10,6,'NORMAL','e5127222-da47-4e25-8f9f-457e8a09741b',NULL,'2026-11-05 23:59:32.151840'),('d9441a94-6641-4040-a9a8-0ad19acdeda5','58abc75f-b941-404e-a206-74af63fc618e','2026-11-26',4,4,'NORMAL','e5127222-da47-4e25-8f9f-457e8a09741b',NULL,'2026-11-05 23:59:32.151758'),('db338d51-2dbe-408b-8080-888261fe1ccf','80e1a826-97f8-433d-9705-7a95ed8a93e3','2026-12-30',4,1,'NORMAL','1939cd58-d478-4c54-ba6e-f7fa50b8672c',NULL,'2026-11-05 23:59:32.154121'),('dbbf8cd1-e859-464f-9208-b021293b9d95','80e1a826-97f8-433d-9705-7a95ed8a93e3','2027-02-15',6,7,'MIDTERM','1939cd58-d478-4c54-ba6e-f7fa50b8672c',NULL,'2026-11-05 23:59:32.154153'),('deaefdfc-a5b2-450d-9320-f7aac96473f7','80e1a826-97f8-433d-9705-7a95ed8a93e3','2027-03-22',9,1,'NORMAL','1939cd58-d478-4c54-ba6e-f7fa50b8672c',NULL,'2026-11-05 23:59:32.154179'),('dfb95fea-1db3-40cb-9723-bf654fd3a297','58abc75f-b941-404e-a206-74af63fc618e','2026-10-22',2,3,'NORMAL','e5127222-da47-4e25-8f9f-457e8a09741b',NULL,'2026-11-05 23:59:32.151731'),('e392f246-6253-4666-9af7-e2960ebad385','80e1a826-97f8-433d-9705-7a95ed8a93e3','2027-02-08',6,4,'NORMAL','1939cd58-d478-4c54-ba6e-f7fa50b8672c',NULL,'2026-11-05 23:59:32.154149'),('e66b1258-0a92-47df-9bd9-03ecbddd2686','58abc75f-b941-404e-a206-74af63fc618e','2026-10-06',1,1,'NORMAL','e5127222-da47-4e25-8f9f-457e8a09741b',NULL,'2026-11-05 23:59:32.151717'),('e7bf6a61-8eb7-4600-ae30-d722749a6890','80e1a826-97f8-433d-9705-7a95ed8a93e3','2027-03-01',7,6,'NORMAL','1939cd58-d478-4c54-ba6e-f7fa50b8672c',NULL,'2026-11-05 23:59:32.154163'),('e7e8e278-f542-4e9d-8336-ec9922e05eb7','80e1a826-97f8-433d-9705-7a95ed8a93e3','2027-04-12',10,4,'NORMAL','1939cd58-d478-4c54-ba6e-f7fa50b8672c',NULL,'2026-11-05 23:59:32.154194'),('e89f42cc-d857-4e7f-aedc-4c2b2c201c87','80e1a826-97f8-433d-9705-7a95ed8a93e3','2027-02-22',7,3,'NORMAL','1939cd58-d478-4c54-ba6e-f7fa50b8672c',NULL,'2026-11-05 23:59:32.154158'),('f0474dde-9492-4d6c-9bf6-52291133f491','80e1a826-97f8-433d-9705-7a95ed8a93e3','2027-03-10',8,4,'NORMAL','1939cd58-d478-4c54-ba6e-f7fa50b8672c',NULL,'2026-11-05 23:59:32.154172'),('f226c314-da00-4139-98aa-dd6446b8a367','80e1a826-97f8-433d-9705-7a95ed8a93e3','2026-11-16',1,1,'NORMAL','1939cd58-d478-4c54-ba6e-f7fa50b8672c',NULL,'2026-11-05 23:59:32.154086'),('f4d989c2-eae5-4295-9b98-61f4b0e61c11','58abc75f-b941-404e-a206-74af63fc618e','2026-10-27',2,4,'NORMAL','e5127222-da47-4e25-8f9f-457e8a09741b',NULL,'2026-11-05 23:59:32.151733'),('ff2eb2a2-07a0-4ae8-ae42-e096eccadc3b','80e1a826-97f8-433d-9705-7a95ed8a93e3','2026-12-28',3,6,'NORMAL','1939cd58-d478-4c54-ba6e-f7fa50b8672c',NULL,'2026-11-05 23:59:32.154119');
/*!40000 ALTER TABLE `sessions` ENABLE KEYS */;
DROP TABLE IF EXISTS `students`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `students` (
  `id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `full_name` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `dob` date DEFAULT NULL,
  `gender` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `phone` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `email` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `guardian_name` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `guardian_phone` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `address` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `created_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  PRIMARY KEY (`id`),
  KEY `ix_students_email` (`email`(100)),
  KEY `ix_students_phone` (`phone`(20))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

/*!40000 ALTER TABLE `students` DISABLE KEYS */;
INSERT INTO `students` (`id`, `full_name`, `dob`, `gender`, `phone`, `email`, `guardian_name`, `guardian_phone`, `address`, `created_at`) VALUES ('7b850206-7cf6-4514-9c64-654ab1b27980','Bùi Thảo Vy','2017-08-17','FEMALE','0901000008',NULL,'Bùi Thị Lan','0901000008','Hà Nội','2026-11-05 23:59:31.414267'),('a4861778-772e-42ee-9f52-3d93958e6ca1','Lê Minh Khang','2018-03-12','MALE','0901000003',NULL,'Lê Văn Phúc','0901000003','Hà Nội','2026-11-05 23:59:31.414267'),('a7f623bc-58f6-487d-acff-fc7069fcd304','Vũ Khánh Linh','2018-06-15','FEMALE','0901000006',NULL,'Vũ Thị Hạnh','0901000006','Hà Nội','2026-11-05 23:59:31.414267'),('b626af7c-4598-48ab-b9a0-4777709f09f8','Đặng Quốc Việt','2016-07-16','MALE','0901000007',NULL,'Đặng Văn Long','0901000007','Hà Nội','2026-11-05 23:59:31.414267'),('b7331574-8932-4acb-8469-ac82505e5c55','Hoàng Đức Anh','2017-05-14','MALE','0901000005',NULL,'Hoàng Văn Nam','0901000005','Hà Nội','2026-11-05 23:59:31.414267'),('c37c2597-b724-4334-9aa5-63c77fce4f11','Trần Bảo Ngọc','2017-02-11','FEMALE','0901000002',NULL,'Trần Thị Mai','0901000002','Hà Nội','2026-11-05 23:59:31.414267'),('cd6fdfb8-9ce0-4e1e-8dec-13baf50481eb','Phạm An Nhiên','2016-04-13','FEMALE','0901000004',NULL,'Phạm Thu Trang','0901000004','Hà Nội','2026-11-05 23:59:31.414267'),('e499904a-3666-47c4-b356-9b3cff43d038','Nguyễn Gia Huy','2016-01-10','MALE','0901000001',NULL,'Nguyễn Văn Hùng','0901000001','Hà Nội','2026-11-05 23:59:31.414267');
/*!40000 ALTER TABLE `students` ENABLE KEYS */;
DROP TABLE IF EXISTS `system_audit_logs`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `system_audit_logs` (
  `id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `actor_user_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `action` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `entity` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `entity_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci DEFAULT NULL,
  `meta` json NOT NULL DEFAULT (json_object()),
  `created_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  PRIMARY KEY (`id`),
  KEY `ix_system_audit_logs_actor_user_id_action_created_at` (`actor_user_id`,`action`(20),`created_at`),
  KEY `ix_system_audit_logs_created_at` (`created_at`),
  KEY `ix_system_audit_logs_entity_entity_id` (`entity`(50),`entity_id`),
  CONSTRAINT `fk_system_audit_logs_users_actor_user_id` FOREIGN KEY (`actor_user_id`) REFERENCES `auth_users` (`id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

/*!40000 ALTER TABLE `system_audit_logs` DISABLE KEYS */;
INSERT INTO `system_audit_logs` (`id`, `actor_user_id`, `action`, `entity`, `entity_id`, `meta`, `created_at`) VALUES ('24dc16e9-0182-45ea-80b5-2f4c25d909ac','8e467049-0746-441d-8ca6-7439d7acda02','ASSIGN_ROLE','auth_users','1939cd58-d478-4c54-ba6e-f7fa50b8672c','{\"role\": \"TEACHER\"}','2026-11-05 23:59:32.146313'),('335c968b-4a8d-438c-a0c4-b54f187c9e9d','8e467049-0746-441d-8ca6-7439d7acda02','ASSIGN_ROLE','auth_users','3ba96209-300d-43bd-afed-accdbac16782','{\"role\": \"SALES\"}','2026-11-05 23:59:31.807855'),('665fc15c-6e6b-44c4-8705-e1cc3502231a','8e467049-0746-441d-8ca6-7439d7acda02','ASSIGN_ROLE','auth_users','e5127222-da47-4e25-8f9f-457e8a09741b','{\"role\": \"TEACHER\"}','2026-11-05 23:59:32.034494'),('754a70bc-310a-49d0-8d87-97114fc82036','8e467049-0746-441d-8ca6-7439d7acda02','ASSIGN_ROLE','auth_users','a2786b12-9205-48eb-9427-d8d14b56004a','{\"role\": \"ACCOUNTANT\"}','2026-11-05 23:59:31.919954'),('a90c2ee0-c516-4ec8-a3c0-c66cc37de210','8e467049-0746-441d-8ca6-7439d7acda02','ASSIGN_ROLE','auth_users','4de4f0a0-8cfb-4796-9eca-6e4ce81d2aa0','{\"role\": \"DIRECTOR\"}','2026-11-05 23:59:31.570967'),('abcba853-7d9a-47da-9d60-3db02dbe7953',NULL,'ASSIGN_ROLE','auth_users','8e467049-0746-441d-8ca6-7439d7acda02','{\"role\": \"ROOT\"}','2026-11-05 23:46:09.699393'),('bd6e138a-5dfd-4d04-94b2-469491d34b49','8e467049-0746-441d-8ca6-7439d7acda02','RESET','system',NULL,'{\"counts\": {\"users\": 7, \"classes\": 2, \"invoices\": 6, \"programs\": 4, \"sessions\": 108, \"students\": 8, \"trialLeads\": 3, \"enrollments\": 8}}','2026-11-05 23:59:32.203373'),('ca1e36d8-799f-4048-835b-94deb5fec313','8e467049-0746-441d-8ca6-7439d7acda02','ASSIGN_ROLE','auth_users','4b18f289-ea31-4d83-96d6-535500e39c2d','{\"role\": \"ACADEMIC\"}','2026-11-05 23:59:31.692924'),('de6624be-50f9-4f3f-87d5-59044cdad0a4','8e467049-0746-441d-8ca6-7439d7acda02','TRIAL_SCHEDULE','trial_leads','b8632e00-4088-462d-9a21-eff43e354d83','{\"note\": \"Phụ huynh đăng ký học thử\", \"classId\": \"80e1a826-97f8-433d-9705-7a95ed8a93e3\", \"sessionId\": \"f226c314-da00-4139-98aa-dd6446b8a367\", \"sessionDate\": \"2026-11-16\"}','2026-11-05 23:59:32.160046');
/*!40000 ALTER TABLE `system_audit_logs` ENABLE KEYS */;
DROP TABLE IF EXISTS `system_notifications`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `system_notifications` (
  `id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `user_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `title` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `body` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `is_read` tinyint(1) NOT NULL DEFAULT '0',
  `created_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  PRIMARY KEY (`id`),
  KEY `ix_system_notifications_user_id_is_read_created_at` (`user_id`,`is_read`,`created_at`),
  CONSTRAINT `fk_system_notifications_auth_users_user_id` FOREIGN KEY (`user_id`) REFERENCES `auth_users` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

/*!40000 ALTER TABLE `system_notifications` DISABLE KEYS */;
INSERT INTO `system_notifications` (`id`, `user_id`, `title`, `body`, `is_read`, `created_at`) VALUES ('09adc7e1-8f3c-4abb-aaf0-012aeafaeb71','a2786b12-9205-48eb-9427-d8d14b56004a','Hóa đơn quá hạn','Có 1 hóa đơn lớp EIM1-STARTERS-2026A đã quá hạn thanh toán.',0,'2026-11-05 23:59:31.414267'),('b054f2b1-fc09-4204-a962-6f7f1c25c657','1939cd58-d478-4c54-ba6e-f7fa50b8672c','Phân công giảng dạy','Bạn được phân công dạy chính lớp EIM1-MOVERS-2026A.',0,'2026-11-05 23:59:31.414267'),('bac54c26-48d4-423c-82c0-3901d6a033cb','4b18f289-ea31-4d83-96d6-535500e39c2d','Lớp mới sắp khai giảng','Lớp EIM1-MOVERS-2026A khai giảng ngày 16/11/2026.',0,'2026-11-05 23:59:31.414267');
/*!40000 ALTER TABLE `system_notifications` ENABLE KEYS */;
DROP TABLE IF EXISTS `trial_leads`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `trial_leads` (
  `id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `full_name` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `phone` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `email` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `source` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `status` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `note` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `created_by` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `created_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  PRIMARY KEY (`id`),
  KEY `ix_trial_leads_created_by` (`created_by`),
  KEY `ix_trial_leads_phone` (`phone`(20)),
  KEY `ix_trial_leads_status` (`status`(20)),
  CONSTRAINT `fk_trial_leads_auth_users_created_by` FOREIGN KEY (`created_by`) REFERENCES `auth_users` (`id`) ON DELETE RESTRICT,
  CONSTRAINT `ck_trial_leads_status` CHECK ((`status` in (_utf8mb4'NEW',_utf8mb4'CONTACTED',_utf8mb4'SCHEDULED',_utf8mb4'ATTENDED',_utf8mb4'NO_SHOW',_utf8mb4'CONVERTED',_utf8mb4'CLOSED')))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

/*!40000 ALTER TABLE `trial_leads` DISABLE KEYS */;
INSERT INTO `trial_leads` (`id`, `full_name`, `phone`, `email`, `source`, `status`, `note`, `created_by`, `created_at`) VALUES ('79b98bd7-53f9-45e6-b62e-d4efe9b0d41f','Ngô Bảo Châu','0912000001',NULL,'Facebook','NEW',NULL,'3ba96209-300d-43bd-afed-accdbac16782','2026-11-05 23:59:31.414267'),('a6ee4f1e-4f72-46f4-ac15-f40a38ffc13c','Trịnh Minh Thư','0912000002','parent.thu@example.com','Website','CONTACTED','Phụ huynh muốn học buổi tối','3ba96209-300d-43bd-afed-accdbac16782','2026-11-05 23:59:31.414267'),('b8632e00-4088-462d-9a21-eff43e354d83','Mai Gia Bảo','0912000003',NULL,'Giới thiệu','SCHEDULED',NULL,'3ba96209-300d-43bd-afed-accdbac16782','2026-11-05 23:59:31.414267');
/*!40000 ALTER TABLE `trial_leads` ENABLE KEYS */;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

