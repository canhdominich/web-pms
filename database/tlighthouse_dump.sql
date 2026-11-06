-- MySQL dump 10.13  Distrib 8.4.9, for Linux (x86_64)
--
-- Host: localhost    Database: tlighthouse
-- ------------------------------------------------------
-- Server version	8.4.9

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

--
-- Table structure for table `auth_roles`
--

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

--
-- Dumping data for table `auth_roles`
--

LOCK TABLES `auth_roles` WRITE;
/*!40000 ALTER TABLE `auth_roles` DISABLE KEYS */;
INSERT INTO `auth_roles` VALUES ('0bdc0e93-2108-4d51-906c-3be2a0f85c6c','TEACHER','Giáo viên'),('11bbd5c2-a508-4b04-a9cb-bcbc2c348876','SALES','Tuyển sinh'),('482ebb06-128f-4c28-a032-a45a4c1aea2e','DIRECTOR','Hiệu trưởng'),('70b2bc67-91c0-4930-a5a8-13b43a41e431','ACCOUNTANT','Kế toán'),('72e5630d-f865-4d31-a543-33f1f0f5b044','ROOT','Quản trị hệ thống'),('be497942-fd2e-4182-b33c-8821f6b44d5f','ACADEMIC','Giáo vụ');
/*!40000 ALTER TABLE `auth_roles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `auth_users`
--

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

--
-- Dumping data for table `auth_users`
--

LOCK TABLES `auth_users` WRITE;
/*!40000 ALTER TABLE `auth_users` DISABLE KEYS */;
INSERT INTO `auth_users` VALUES ('63f7f44e-0cd0-4b4a-aed2-19c77eca8ad9','giaovu@tlighthouse.local','AQAAAAIAAYagAAAAEHDtBiw7gOKf2F4qMSp8oRBjGsk797bTXW7FxAMuvi7J6OwZPnBZG+igojxraRsMOQ==','Phạm Thu Hương','ACTIVE','2026-11-06 14:37:25.310725'),('6a76758d-560f-4057-a231-3b9ed014a0dc','tuyensinh@tlighthouse.local','AQAAAAIAAYagAAAAELE+r/xNSAv3J3aVrEU70GY3zg3RONdLYYz1Zx2MjtJNJI4bPgDSFztEg6jyHP0zpg==','Lê Hoàng Yến','ACTIVE','2026-11-06 14:37:25.520333'),('6db6f765-0251-434c-b5a2-13f6b9af8dde','giaovien2@tlighthouse.local','AQAAAAIAAYagAAAAEPieIvpNfsSE7twsdeerOeK/zSJxhYxbCw/qQ3tk1BJy/uF9v5ERdOCjNKRMbRoibg==','Lê Quốc Bảo','ACTIVE','2026-11-06 14:37:26.140825'),('92f96ce5-3644-42e0-94c0-699a5d6e4d00','ketoan@tlighthouse.local','AQAAAAIAAYagAAAAEETyCj/q9LXxsxeedS3opepwivAkKje+CFVNCSYWZopOcx2/2K7nni1QnDTxP0gYAw==','Đỗ Thanh Tâm','ACTIVE','2026-11-06 14:37:25.724229'),('9c599cba-54c3-48f8-82f8-e0b43c9126b4','giaovien1@tlighthouse.local','AQAAAAIAAYagAAAAEPiAcMzII3a01GfgYfzqPQUT4+KPUG0kYUlhG2m8NxSPr4V2jJzgqc/VRL3TmH6iWA==','Trần Thu Hà','ACTIVE','2026-11-06 14:37:25.918488'),('b0d991f9-b4ce-4fb7-a857-4b42c327a47e','hieutruong@tlighthouse.local','AQAAAAIAAYagAAAAEPAxFQM5G4R67YbE5FNmCrM19XRxWXT2Gr9l19J7vuVPtZPza9bwcVaTfUyNARGnOg==','Nguyễn Minh Đức','ACTIVE','2026-11-06 14:37:25.118185'),('bb4d9fa1-a193-4575-9710-c93e8a51b62f','root@tlighthouse.local','AQAAAAIAAYagAAAAEF11qeuM22+vz2M8U/kaEWoNdfYJ+FaKPlwdJkM8Ooq+QpSIEb+Qx6QQs0sdXqteLg==','System Root','ACTIVE','2026-11-06 14:37:24.248285');
/*!40000 ALTER TABLE `auth_users` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `classes`
--

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

--
-- Dumping data for table `classes`
--

LOCK TABLES `classes` WRITE;
/*!40000 ALTER TABLE `classes` DISABLE KEYS */;
INSERT INTO `classes` VALUES ('5c7b6dae-a17d-448a-a31e-a57039e1e5d5','TLH-MAM1-2026A','Mầm 1 - Mặt Trời','54fe4404-15eb-4500-ab58-e207ed0ed3d5','Phòng Mặt Trời',20,'2026-11-13','ACTIVE','2026-11-06 14:37:26.507418'),('c1ec3be8-d7b3-438b-9eb3-f982e5018891','TLH-CHOI1-2026A','Chồi 1 - Cầu Vồng','d1fcd5fd-f89e-4bbb-8e04-6b88eea96fe1','Phòng Cầu Vồng',20,'2026-10-02','ACTIVE','2026-11-06 14:37:26.359628');
/*!40000 ALTER TABLE `classes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `curriculum_programs`
--

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

--
-- Dumping data for table `curriculum_programs`
--

LOCK TABLES `curriculum_programs` WRITE;
/*!40000 ALTER TABLE `curriculum_programs` DISABLE KEYS */;
INSERT INTO `curriculum_programs` VALUES ('1ba1c9f3-3f56-4238-9302-c62c4ac07e40','NHATRE','Nhà trẻ Ánh Sao','24 - 36 tháng tuổi',10,7,5,'51a34c34-5b45-4bc4-b572-01306af56384','2026-11-06 14:37:24.917304'),('54fe4404-15eb-4500-ab58-e207ed0ed3d5','MAM','Lớp Mầm Mặt Trời','3 - 4 tuổi',12,7,5,'8cf176fc-4a2a-4b64-91c4-f5ed9bd3639e','2026-11-06 14:37:24.917304'),('7d96aebb-eeab-47e6-ada9-82ceaa0684ad','LA','Lớp Lá Hải Đăng','5 - 6 tuổi',12,7,5,'d108b51a-ecd0-4e85-a85b-4539c5e538dd','2026-11-06 14:37:24.917304'),('d1fcd5fd-f89e-4bbb-8e04-6b88eea96fe1','CHOI','Lớp Chồi Cầu Vồng','4 - 5 tuổi',12,7,5,'da020e29-3e88-4196-95d0-576c21f34951','2026-11-06 14:37:24.917304');
/*!40000 ALTER TABLE `curriculum_programs` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `enrollments`
--

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

--
-- Dumping data for table `enrollments`
--

LOCK TABLES `enrollments` WRITE;
/*!40000 ALTER TABLE `enrollments` DISABLE KEYS */;
INSERT INTO `enrollments` VALUES ('0e11add0-11aa-42dd-8c93-e4e5f9dc6f18','f277ae6e-4895-43db-98bf-ae0c1493586c','c1ec3be8-d7b3-438b-9eb3-f982e5018891','ACTIVE','2026-10-02',NULL,'2026-11-06 14:37:26.580057'),('21f42718-cf2f-4012-850c-86cf16d19378','9926b55b-dc45-46b8-8061-c9795625a11c','c1ec3be8-d7b3-438b-9eb3-f982e5018891','ACTIVE','2026-10-02',NULL,'2026-11-06 14:37:26.626409'),('21fee7a7-1377-47b8-87df-2bf320cb75d2','466477d9-2724-4a4e-909e-f312720ea4e7','5c7b6dae-a17d-448a-a31e-a57039e1e5d5','ACTIVE','2026-11-13',NULL,'2026-11-06 14:37:26.626993'),('7286d72f-e53c-4fc4-9679-38089682acfd','8aa6c2cd-e347-4afb-8102-d90a4be147f7','5c7b6dae-a17d-448a-a31e-a57039e1e5d5','ACTIVE','2026-11-13',NULL,'2026-11-06 14:37:26.627201'),('734d175d-b815-443b-bd06-7bffd20657a4','51bd7a04-f28f-4813-8312-88c20180a97f','c1ec3be8-d7b3-438b-9eb3-f982e5018891','ACTIVE','2026-10-02',NULL,'2026-11-06 14:37:26.626320'),('8eb87d6e-8374-474d-b85a-94ab2e4e2bc2','787be491-3d9a-4475-960c-9d3fbbc99a32','c1ec3be8-d7b3-438b-9eb3-f982e5018891','ACTIVE','2026-10-02',NULL,'2026-11-06 14:37:26.626492'),('c82b816f-d630-4903-866b-b57231850d90','8dd66899-c8dc-4e92-a8d3-86b0e3dc54b7','c1ec3be8-d7b3-438b-9eb3-f982e5018891','ACTIVE','2026-10-02',NULL,'2026-11-06 14:37:26.625977'),('fdb5e4f3-2161-48c5-8dad-1fdee8eb86ca','dbdd2d24-bc46-46a8-815d-f0dfae35c2a2','c1ec3be8-d7b3-438b-9eb3-f982e5018891','ACTIVE','2026-10-02',NULL,'2026-11-06 14:37:26.626202');
/*!40000 ALTER TABLE `enrollments` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `finance_fee_plans`
--

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

--
-- Dumping data for table `finance_fee_plans`
--

LOCK TABLES `finance_fee_plans` WRITE;
/*!40000 ALTER TABLE `finance_fee_plans` DISABLE KEYS */;
INSERT INTO `finance_fee_plans` VALUES ('51a34c34-5b45-4bc4-b572-01306af56384','1ba1c9f3-3f56-4238-9302-c62c4ac07e40','Nhà trẻ Ánh Sao - Học phí học kỳ (3 tháng)',9000000,'VND',5,'2026-11-06 14:37:24.917304'),('8cf176fc-4a2a-4b64-91c4-f5ed9bd3639e','54fe4404-15eb-4500-ab58-e207ed0ed3d5','Lớp Mầm Mặt Trời - Học phí học kỳ (3 tháng)',10500000,'VND',5,'2026-11-06 14:37:24.917304'),('d108b51a-ecd0-4e85-a85b-4539c5e538dd','7d96aebb-eeab-47e6-ada9-82ceaa0684ad','Lớp Lá Hải Đăng - Học phí học kỳ (3 tháng)',12000000,'VND',5,'2026-11-06 14:37:24.917304'),('da020e29-3e88-4196-95d0-576c21f34951','d1fcd5fd-f89e-4bbb-8e04-6b88eea96fe1','Lớp Chồi Cầu Vồng - Học phí học kỳ (3 tháng)',11400000,'VND',5,'2026-11-06 14:37:24.917304');
/*!40000 ALTER TABLE `finance_fee_plans` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `finance_invoices`
--

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

--
-- Dumping data for table `finance_invoices`
--

LOCK TABLES `finance_invoices` WRITE;
/*!40000 ALTER TABLE `finance_invoices` DISABLE KEYS */;
INSERT INTO `finance_invoices` VALUES ('6998cfd6-94f3-446c-b4a2-9c042d1f524d','734d175d-b815-443b-bd06-7bffd20657a4',11400000,'OVERDUE','2026-11-01','2026-10-12 14:37:24.917304','2026-10-12 14:37:24.917304'),('85b2cc00-f224-4deb-adeb-687203fb98b0','8eb87d6e-8374-474d-b85a-94ab2e4e2bc2',11400000,'DRAFT','2026-12-06',NULL,'2026-10-12 14:37:24.917304'),('a458cc68-9946-48e8-9b49-4b96fb86b174','0e11add0-11aa-42dd-8c93-e4e5f9dc6f18',11400000,'PAID','2026-10-17','2026-10-12 14:37:24.917304','2026-10-12 14:37:24.917304'),('c2ec2b65-d782-4eae-b2a8-75fe880fe160','c82b816f-d630-4903-866b-b57231850d90',11400000,'PAID','2026-10-17','2026-10-12 14:37:24.917304','2026-10-12 14:37:24.917304'),('ca0be158-9993-4ba5-bd75-950905bf2590','fdb5e4f3-2161-48c5-8dad-1fdee8eb86ca',11400000,'ISSUED','2026-11-16','2026-10-12 14:37:24.917304','2026-10-12 14:37:24.917304'),('f59e9e59-7675-4226-b086-55bc4495fa5d','21f42718-cf2f-4012-850c-86cf16d19378',11400000,'ISSUED','2026-11-26','2026-10-12 14:37:24.917304','2026-10-12 14:37:24.917304');
/*!40000 ALTER TABLE `finance_invoices` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `finance_payments`
--

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

--
-- Dumping data for table `finance_payments`
--

LOCK TABLES `finance_payments` WRITE;
/*!40000 ALTER TABLE `finance_payments` DISABLE KEYS */;
INSERT INTO `finance_payments` VALUES ('15e75563-34c4-47af-9dc4-4f800e0a5e18','ca0be158-9993-4ba5-bd75-950905bf2590',5700000,'CARD','2026-11-03 14:37:24.917304','2026-11-03 14:37:24.917304'),('7ccddf4f-5887-4786-b4c9-3dbdb563134b','c2ec2b65-d782-4eae-b2a8-75fe880fe160',11400000,'CASH','2026-10-15 14:37:24.917304','2026-10-15 14:37:24.917304'),('f5f5715d-39a7-450c-8098-395eb1ab502a','a458cc68-9946-48e8-9b49-4b96fb86b174',11400000,'TRANSFER','2026-10-15 14:37:24.917304','2026-10-15 14:37:24.917304');
/*!40000 ALTER TABLE `finance_payments` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `session_feedback`
--

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

--
-- Dumping data for table `session_feedback`
--

LOCK TABLES `session_feedback` WRITE;
/*!40000 ALTER TABLE `session_feedback` DISABLE KEYS */;
INSERT INTO `session_feedback` VALUES ('00f40057-f199-4501-94ff-21f487be8f81','a57b0138-243e-48cc-9e93-6bad1cda7a62','9926b55b-dc45-46b8-8061-c9795625a11c','PRESENT','DONE','Hào hứng tham gia hoạt động','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 2','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('02f1fdf3-fc0c-4bb4-aadf-d1e90cca14b9','24db0296-7762-42b6-99b1-7bb564b707c1','f277ae6e-4895-43db-98bf-ae0c1493586c','PRESENT','DONE','Cần khuyến khích tham gia hơn','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 4','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('04a6c4de-62b4-44c5-82e3-e5547c9143b9','24db0296-7762-42b6-99b1-7bb564b707c1','8dd66899-c8dc-4e92-a8d3-86b0e3dc54b7','ABSENT','NOT_DONE','Hào hứng tham gia hoạt động','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 4','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('072d56b1-ec44-46ea-a08f-e132d3c9c539','0a757c99-07b4-4e71-b44f-e6993fb69018','dbdd2d24-bc46-46a8-815d-f0dfae35c2a2','PRESENT','DONE','Hào hứng tham gia hoạt động','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 6','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('085ede80-c402-43a8-a871-11246d6f8493','3991101f-415f-4f95-bc52-0d0d344d5df4','51bd7a04-f28f-4813-8312-88c20180a97f','PRESENT','DONE','Cần khuyến khích tham gia hơn','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 1','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('0995ebcc-d649-498c-9ad7-22bdaba4cb80','0a757c99-07b4-4e71-b44f-e6993fb69018','51bd7a04-f28f-4813-8312-88c20180a97f','LATE','DONE','Cần khuyến khích tham gia hơn','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 6','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('09bdcbc4-5bc6-4c6e-8ee0-67c74325e51b','af6919e7-8b4c-479a-a44c-077b36d4e7bc','51bd7a04-f28f-4813-8312-88c20180a97f','PRESENT','DONE','Cần khuyến khích tham gia hơn','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 6','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('0c65787a-dbb3-430b-abef-3573d33b6293','3c3e3ddf-9359-423f-91b3-2692dd37e6bb','9926b55b-dc45-46b8-8061-c9795625a11c','LATE','DONE','Hào hứng tham gia hoạt động','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 2','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('106ecaf4-9b51-4716-bbd4-a8d866b8aff9','17717904-06c0-4326-ae62-6cf74ef643cd','dbdd2d24-bc46-46a8-815d-f0dfae35c2a2','PRESENT','DONE','Cần khuyến khích tham gia hơn','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 1','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('10808dad-cc72-471c-950a-170e03987273','3f3be23c-a430-4fd2-9405-770cf5f4e4ab','f277ae6e-4895-43db-98bf-ae0c1493586c','LATE','NOT_DONE','Cần khuyến khích tham gia hơn','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 2','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('10d44f71-5668-4f31-a440-d2fd9f845f45','96157490-73c5-49ed-b227-a2ef1fc33d2d','8dd66899-c8dc-4e92-a8d3-86b0e3dc54b7','LATE','NOT_DONE','Cần khuyến khích tham gia hơn','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 2','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('1644e21b-3608-4898-a995-22548a088cfc','e9bcc72a-e633-4419-9d61-bc0888d4feaa','8dd66899-c8dc-4e92-a8d3-86b0e3dc54b7','PRESENT','DONE','Hào hứng tham gia hoạt động','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 5','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('16caa80c-e3e1-45ec-bfe9-b6cd3581b34d','51e7dccd-8ca2-495a-8845-3999e325a2ef','9926b55b-dc45-46b8-8061-c9795625a11c','PRESENT','DONE','Cần khuyến khích tham gia hơn','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 3','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('1763909c-27fe-4e30-95a2-4846c3e70b79','884504e2-2663-48a0-8d48-cd5c375c6b10','787be491-3d9a-4475-960c-9d3fbbc99a32','PRESENT','DONE','Hào hứng tham gia hoạt động','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 5','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('18acd710-25af-4af7-b5e6-4f38a634709c','3f3be23c-a430-4fd2-9405-770cf5f4e4ab','9926b55b-dc45-46b8-8061-c9795625a11c','PRESENT','DONE','Cần khuyến khích tham gia hơn','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 2','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('1c0c8fd3-af08-42b9-8e06-85ebc13fd30d','3f3be23c-a430-4fd2-9405-770cf5f4e4ab','8dd66899-c8dc-4e92-a8d3-86b0e3dc54b7','PRESENT','DONE','Hào hứng tham gia hoạt động','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 2','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('1c740146-4d19-485f-a757-0d7dfe57696b','4a46e5f3-25d2-4908-b02b-1bb06418a7c5','dbdd2d24-bc46-46a8-815d-f0dfae35c2a2','PRESENT','DONE','Cần khuyến khích tham gia hơn','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 2','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('1ca3771d-10e4-4631-98c4-c198111deb1c','ce2cdf85-482a-4885-80e2-64a7237b23f1','f277ae6e-4895-43db-98bf-ae0c1493586c','ABSENT','NOT_DONE','Hào hứng tham gia hoạt động','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 4','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('1e662925-2b80-4683-9821-e0a9f404d3eb','51e7dccd-8ca2-495a-8845-3999e325a2ef','787be491-3d9a-4475-960c-9d3fbbc99a32','ABSENT','NOT_DONE','Hào hứng tham gia hoạt động','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 3','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('1fe5bbf2-eeee-4193-8df6-998999cb830e','5e69f33f-8ec9-4480-a792-b39e35c6b369','dbdd2d24-bc46-46a8-815d-f0dfae35c2a2','PRESENT','DONE','Cần khuyến khích tham gia hơn','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 3','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('20a88b40-1b3d-4233-887d-6fda9005aabd','c6e2a726-d23f-422b-be29-a8f6e1abbe35','51bd7a04-f28f-4813-8312-88c20180a97f','PRESENT','DONE','Hào hứng tham gia hoạt động','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 1','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('21d98935-8102-411f-8a2c-df896d1cf924','a57b0138-243e-48cc-9e93-6bad1cda7a62','51bd7a04-f28f-4813-8312-88c20180a97f','PRESENT','NOT_DONE','Cần khuyến khích tham gia hơn','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 2','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('229841ca-0d09-49f9-a845-232eb70d5c03','96157490-73c5-49ed-b227-a2ef1fc33d2d','51bd7a04-f28f-4813-8312-88c20180a97f','ABSENT','DONE','Cần khuyến khích tham gia hơn','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 2','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('240b4cd7-3342-4f24-a841-bffd8496e1fd','3f3be23c-a430-4fd2-9405-770cf5f4e4ab','51bd7a04-f28f-4813-8312-88c20180a97f','PRESENT','NOT_DONE','Hào hứng tham gia hoạt động','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 2','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('24e016a1-3fd6-4d37-ba3c-a7ed28048a93','99c23cd1-9a07-4a83-a486-818c6bccd0bb','f277ae6e-4895-43db-98bf-ae0c1493586c','PRESENT','NOT_DONE','Cần khuyến khích tham gia hơn','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 6','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('258c2562-0f18-43e3-b293-d6ac533717c9','4f7544c6-e93a-4e26-8a83-723a6cdd6228','f277ae6e-4895-43db-98bf-ae0c1493586c','LATE','DONE','Cần khuyến khích tham gia hơn','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 4','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('262e015d-f682-4068-862b-e18da666e52c','0a757c99-07b4-4e71-b44f-e6993fb69018','f277ae6e-4895-43db-98bf-ae0c1493586c','PRESENT','DONE','Hào hứng tham gia hoạt động','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 6','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('26c10bbd-df53-4d2e-8089-2d18e5d0629d','3c3e3ddf-9359-423f-91b3-2692dd37e6bb','dbdd2d24-bc46-46a8-815d-f0dfae35c2a2','PRESENT','NOT_DONE','Hào hứng tham gia hoạt động','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 2','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('26e05a1d-00d0-4e9c-b2a5-05e3e7f43306','1154c952-b710-45d1-9149-c22526a4ff1a','dbdd2d24-bc46-46a8-815d-f0dfae35c2a2','PRESENT','DONE','Cần khuyến khích tham gia hơn','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 4','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('28ce68fb-f652-4c54-aa70-473ee46110ef','52dbdab2-9652-4f7e-b985-0a8616caa2bd','f277ae6e-4895-43db-98bf-ae0c1493586c','PRESENT','DONE','Hào hứng tham gia hoạt động','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 5','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('2a1cb3e1-3ac9-4410-81e3-3657611b55f0','17717904-06c0-4326-ae62-6cf74ef643cd','f277ae6e-4895-43db-98bf-ae0c1493586c','PRESENT','DONE','Cần khuyến khích tham gia hơn','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 1','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('34d78c50-af39-4ba0-86a6-f7d56c958780','b8568717-f78f-40d1-985c-dcd963a4b636','51bd7a04-f28f-4813-8312-88c20180a97f','LATE','DONE','Cần khuyến khích tham gia hơn','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 3','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('3a95015c-a117-4ede-952c-25fb79e57888','3991101f-415f-4f95-bc52-0d0d344d5df4','787be491-3d9a-4475-960c-9d3fbbc99a32','LATE','NOT_DONE','Cần khuyến khích tham gia hơn','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 1','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('3fc8dab2-7ba7-4728-be78-eee05edbe9c4','52dbdab2-9652-4f7e-b985-0a8616caa2bd','787be491-3d9a-4475-960c-9d3fbbc99a32','PRESENT','NOT_DONE','Cần khuyến khích tham gia hơn','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 5','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('416f1196-cc98-469d-8931-74ab75597731','3f3be23c-a430-4fd2-9405-770cf5f4e4ab','787be491-3d9a-4475-960c-9d3fbbc99a32','LATE','DONE','Hào hứng tham gia hoạt động','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 2','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('41a240b7-610e-4b4d-9dc0-26d7cb400394','e9bcc72a-e633-4419-9d61-bc0888d4feaa','f277ae6e-4895-43db-98bf-ae0c1493586c','ABSENT','DONE','Cần khuyến khích tham gia hơn','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 5','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('4541389e-d03a-4738-b338-8df31c145fa4','c6e2a726-d23f-422b-be29-a8f6e1abbe35','f277ae6e-4895-43db-98bf-ae0c1493586c','PRESENT','DONE','Cần khuyến khích tham gia hơn','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 1','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('462186a2-3dfd-4071-bb82-c8d150e9f82c','c6e2a726-d23f-422b-be29-a8f6e1abbe35','787be491-3d9a-4475-960c-9d3fbbc99a32','PRESENT','NOT_DONE','Hào hứng tham gia hoạt động','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 1','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('4a649253-049c-4b9d-a7e9-203f4a9bcb62','36624795-e289-41e3-a1d7-558dcaeb3c10','dbdd2d24-bc46-46a8-815d-f0dfae35c2a2','PRESENT','DONE','Hào hứng tham gia hoạt động','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 1','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('4a6c51f8-80e6-4d07-8e2f-a4eafd548bb2','b4d5b8d2-7b32-4546-8a67-71ca36b7841a','9926b55b-dc45-46b8-8061-c9795625a11c','LATE','NOT_DONE','Hào hứng tham gia hoạt động','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 4','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('4d263b73-29d5-4bd3-988a-bd57942afbc1','ba957152-32e2-469d-b071-3292207507ba','9926b55b-dc45-46b8-8061-c9795625a11c','ABSENT','NOT_DONE','Hào hứng tham gia hoạt động','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 3','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('4e00979d-81bb-4514-bb55-52641de00cdc','ba957152-32e2-469d-b071-3292207507ba','51bd7a04-f28f-4813-8312-88c20180a97f','PRESENT','DONE','Cần khuyến khích tham gia hơn','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 3','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('509cfa67-2523-4fb5-aa01-3bfb9b097522','e9bcc72a-e633-4419-9d61-bc0888d4feaa','9926b55b-dc45-46b8-8061-c9795625a11c','LATE','DONE','Cần khuyến khích tham gia hơn','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 5','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('512eec4e-203f-40f2-ba6e-9d2df4b45e9f','b4d5b8d2-7b32-4546-8a67-71ca36b7841a','787be491-3d9a-4475-960c-9d3fbbc99a32','ABSENT','DONE','Cần khuyến khích tham gia hơn','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 4','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('52a1877a-468f-410d-9460-e9e30bc41068','4f7544c6-e93a-4e26-8a83-723a6cdd6228','dbdd2d24-bc46-46a8-815d-f0dfae35c2a2','PRESENT','NOT_DONE','Cần khuyến khích tham gia hơn','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 4','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('55b58f43-5ec4-4a47-b853-17bd61741d93','a57b0138-243e-48cc-9e93-6bad1cda7a62','787be491-3d9a-4475-960c-9d3fbbc99a32','PRESENT','DONE','Cần khuyến khích tham gia hơn','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 2','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('5604ec58-e50d-4bdf-b600-537bc48806e9','5ea4474e-aed8-4034-ba08-4a0e4aab2cb2','787be491-3d9a-4475-960c-9d3fbbc99a32','LATE','DONE','Cần khuyến khích tham gia hơn','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 5','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('5d508ed8-6ac4-45a8-a0a9-df32f107e34b','e9bcc72a-e633-4419-9d61-bc0888d4feaa','51bd7a04-f28f-4813-8312-88c20180a97f','PRESENT','DONE','Hào hứng tham gia hoạt động','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 5','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('5dd7f9d5-c555-4db0-8b09-ab063150be15','4a46e5f3-25d2-4908-b02b-1bb06418a7c5','f277ae6e-4895-43db-98bf-ae0c1493586c','ABSENT','DONE','Cần khuyến khích tham gia hơn','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 2','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('5f187343-743e-4c86-a530-acc002fe5081','36624795-e289-41e3-a1d7-558dcaeb3c10','787be491-3d9a-4475-960c-9d3fbbc99a32','ABSENT','DONE','Cần khuyến khích tham gia hơn','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 1','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('5f7af762-3e4e-4581-acb4-0be5e4e1380c','884504e2-2663-48a0-8d48-cd5c375c6b10','8dd66899-c8dc-4e92-a8d3-86b0e3dc54b7','LATE','NOT_DONE','Hào hứng tham gia hoạt động','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 5','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('6933bfc5-7b0c-4cb9-afc3-91d4e8e06385','24db0296-7762-42b6-99b1-7bb564b707c1','51bd7a04-f28f-4813-8312-88c20180a97f','PRESENT','DONE','Hào hứng tham gia hoạt động','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 4','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('6986506a-5285-4567-946c-acae375e1470','52dbdab2-9652-4f7e-b985-0a8616caa2bd','9926b55b-dc45-46b8-8061-c9795625a11c','PRESENT','DONE','Hào hứng tham gia hoạt động','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 5','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('69a184ae-23a5-4f2f-a4c8-ceebeb3068d4','a57b0138-243e-48cc-9e93-6bad1cda7a62','dbdd2d24-bc46-46a8-815d-f0dfae35c2a2','LATE','DONE','Hào hứng tham gia hoạt động','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 2','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('69e1f188-0424-45f4-aef1-c8aaf59b0661','884504e2-2663-48a0-8d48-cd5c375c6b10','51bd7a04-f28f-4813-8312-88c20180a97f','PRESENT','DONE','Hào hứng tham gia hoạt động','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 5','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('6b9ab0d3-379f-44bf-a407-4d6350c5249b','51e7dccd-8ca2-495a-8845-3999e325a2ef','dbdd2d24-bc46-46a8-815d-f0dfae35c2a2','PRESENT','NOT_DONE','Cần khuyến khích tham gia hơn','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 3','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('6b9b92da-cf47-4eb3-bf05-d4df51b0c00f','b8568717-f78f-40d1-985c-dcd963a4b636','8dd66899-c8dc-4e92-a8d3-86b0e3dc54b7','PRESENT','DONE','Cần khuyến khích tham gia hơn','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 3','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('6f5b0b95-7fdb-4c0b-b312-266648d5a92a','96157490-73c5-49ed-b227-a2ef1fc33d2d','f277ae6e-4895-43db-98bf-ae0c1493586c','PRESENT','DONE','Hào hứng tham gia hoạt động','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 2','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('73e7719e-9aeb-420d-89eb-010e29196d12','5e69f33f-8ec9-4480-a792-b39e35c6b369','f277ae6e-4895-43db-98bf-ae0c1493586c','PRESENT','NOT_DONE','Cần khuyến khích tham gia hơn','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 3','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('73fa9bab-025a-4ce8-b63b-8fe68c37cfd3','1154c952-b710-45d1-9149-c22526a4ff1a','787be491-3d9a-4475-960c-9d3fbbc99a32','PRESENT','DONE','Hào hứng tham gia hoạt động','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 4','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('7713a5b7-62cc-405e-b944-77fc7f41e846','ba957152-32e2-469d-b071-3292207507ba','787be491-3d9a-4475-960c-9d3fbbc99a32','LATE','DONE','Cần khuyến khích tham gia hơn','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 3','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('774a1e58-d88e-4547-bdd4-9fe5042c954c','af6919e7-8b4c-479a-a44c-077b36d4e7bc','f277ae6e-4895-43db-98bf-ae0c1493586c','PRESENT','DONE','Hào hứng tham gia hoạt động','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 6','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('796e9659-1a8f-40c7-9b2b-4566de24220a','24db0296-7762-42b6-99b1-7bb564b707c1','9926b55b-dc45-46b8-8061-c9795625a11c','PRESENT','NOT_DONE','Cần khuyến khích tham gia hơn','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 4','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('7c1adc2b-9a8f-4f0c-b828-661978f4812c','3c3e3ddf-9359-423f-91b3-2692dd37e6bb','787be491-3d9a-4475-960c-9d3fbbc99a32','PRESENT','NOT_DONE','Cần khuyến khích tham gia hơn','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 2','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('7c45d85a-a474-4085-867d-c4687327683f','4f7544c6-e93a-4e26-8a83-723a6cdd6228','9926b55b-dc45-46b8-8061-c9795625a11c','PRESENT','DONE','Cần khuyến khích tham gia hơn','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 4','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('7cca4283-5594-4a29-8aaa-ebdc3f50a8fc','52dbdab2-9652-4f7e-b985-0a8616caa2bd','51bd7a04-f28f-4813-8312-88c20180a97f','ABSENT','DONE','Cần khuyến khích tham gia hơn','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 5','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('7d861b35-2492-46c1-a295-0b6cc7042700','4a46e5f3-25d2-4908-b02b-1bb06418a7c5','9926b55b-dc45-46b8-8061-c9795625a11c','PRESENT','NOT_DONE','Cần khuyến khích tham gia hơn','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 2','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('7e400f3f-9102-47bf-8577-4beb86f97199','3991101f-415f-4f95-bc52-0d0d344d5df4','dbdd2d24-bc46-46a8-815d-f0dfae35c2a2','PRESENT','NOT_DONE','Hào hứng tham gia hoạt động','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 1','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('7f604581-241a-4d14-aa32-66a59e43931d','ba957152-32e2-469d-b071-3292207507ba','8dd66899-c8dc-4e92-a8d3-86b0e3dc54b7','PRESENT','NOT_DONE','Cần khuyến khích tham gia hơn','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 3','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('801082c1-219f-4008-8614-b2bf66c5a880','5e69f33f-8ec9-4480-a792-b39e35c6b369','51bd7a04-f28f-4813-8312-88c20180a97f','ABSENT','NOT_DONE','Hào hứng tham gia hoạt động','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 3','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('83aa7209-4ab3-4a33-9ae0-4d66de40acd6','4f7544c6-e93a-4e26-8a83-723a6cdd6228','51bd7a04-f28f-4813-8312-88c20180a97f','PRESENT','DONE','Hào hứng tham gia hoạt động','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 4','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('84479555-0319-4f51-b1a6-4d2ea7d28374','af6919e7-8b4c-479a-a44c-077b36d4e7bc','8dd66899-c8dc-4e92-a8d3-86b0e3dc54b7','LATE','DONE','Cần khuyến khích tham gia hơn','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 6','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('854b4db2-f275-423e-9c53-a1410f8f948b','5ea4474e-aed8-4034-ba08-4a0e4aab2cb2','51bd7a04-f28f-4813-8312-88c20180a97f','PRESENT','NOT_DONE','Cần khuyến khích tham gia hơn','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 5','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('88f1fb63-a5b2-4b4f-99d7-036ec492e1ae','52dbdab2-9652-4f7e-b985-0a8616caa2bd','8dd66899-c8dc-4e92-a8d3-86b0e3dc54b7','PRESENT','DONE','Cần khuyến khích tham gia hơn','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 5','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('89554c86-9a65-46b8-ae68-4f2d6297f6eb','5ea4474e-aed8-4034-ba08-4a0e4aab2cb2','f277ae6e-4895-43db-98bf-ae0c1493586c','LATE','NOT_DONE','Hào hứng tham gia hoạt động','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 5','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('8b7080d4-81d7-4a66-878a-51b53d890a9c','b4d5b8d2-7b32-4546-8a67-71ca36b7841a','f277ae6e-4895-43db-98bf-ae0c1493586c','PRESENT','DONE','Hào hứng tham gia hoạt động','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 4','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('8c163cc1-04f2-49c4-a016-c1c781354548','5e69f33f-8ec9-4480-a792-b39e35c6b369','9926b55b-dc45-46b8-8061-c9795625a11c','LATE','DONE','Cần khuyến khích tham gia hơn','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 3','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('8d7e5e34-bc2b-4523-9715-4333321c49a1','c6e2a726-d23f-422b-be29-a8f6e1abbe35','dbdd2d24-bc46-46a8-815d-f0dfae35c2a2','LATE','NOT_DONE','Cần khuyến khích tham gia hơn','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 1','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('8eecaee2-aff5-45bc-bbbd-55ea38c7850a','1154c952-b710-45d1-9149-c22526a4ff1a','51bd7a04-f28f-4813-8312-88c20180a97f','LATE','NOT_DONE','Hào hứng tham gia hoạt động','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 4','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('8f43ddcf-29dc-40bc-a69c-8356938cf4fa','3991101f-415f-4f95-bc52-0d0d344d5df4','8dd66899-c8dc-4e92-a8d3-86b0e3dc54b7','PRESENT','DONE','Cần khuyến khích tham gia hơn','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 1','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('911bafab-46f1-485d-817e-7d4a0296515b','4a46e5f3-25d2-4908-b02b-1bb06418a7c5','787be491-3d9a-4475-960c-9d3fbbc99a32','PRESENT','DONE','Hào hứng tham gia hoạt động','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 2','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('91bf63e7-17a5-4216-857b-ef702f54b054','b8568717-f78f-40d1-985c-dcd963a4b636','dbdd2d24-bc46-46a8-815d-f0dfae35c2a2','ABSENT','NOT_DONE','Hào hứng tham gia hoạt động','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 3','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('9284e63b-e719-4141-b558-a5f8695df13d','af6919e7-8b4c-479a-a44c-077b36d4e7bc','787be491-3d9a-4475-960c-9d3fbbc99a32','PRESENT','NOT_DONE','Cần khuyến khích tham gia hơn','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 6','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('93503bd0-092d-4c79-b66d-bf4817bab973','0a757c99-07b4-4e71-b44f-e6993fb69018','9926b55b-dc45-46b8-8061-c9795625a11c','PRESENT','NOT_DONE','Hào hứng tham gia hoạt động','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 6','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('935dba44-8b26-4e4a-bad6-a54e8df19e7a','96157490-73c5-49ed-b227-a2ef1fc33d2d','dbdd2d24-bc46-46a8-815d-f0dfae35c2a2','PRESENT','DONE','Hào hứng tham gia hoạt động','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 2','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('941ac6cc-6be4-48f6-bf5a-f42c483deb0f','17717904-06c0-4326-ae62-6cf74ef643cd','9926b55b-dc45-46b8-8061-c9795625a11c','LATE','NOT_DONE','Cần khuyến khích tham gia hơn','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 1','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('950ef465-b484-42ab-9bd0-96db9dfc9476','884504e2-2663-48a0-8d48-cd5c375c6b10','9926b55b-dc45-46b8-8061-c9795625a11c','PRESENT','NOT_DONE','Cần khuyến khích tham gia hơn','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 5','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('9a9026da-287c-4c29-8f0e-753813d5452a','36624795-e289-41e3-a1d7-558dcaeb3c10','8dd66899-c8dc-4e92-a8d3-86b0e3dc54b7','PRESENT','DONE','Cần khuyến khích tham gia hơn','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 1','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('9c3fb274-4886-4ebf-8798-f41d64dcbeb7','0a757c99-07b4-4e71-b44f-e6993fb69018','8dd66899-c8dc-4e92-a8d3-86b0e3dc54b7','PRESENT','NOT_DONE','Cần khuyến khích tham gia hơn','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 6','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('9c4c71e1-3c31-4b5b-a9f0-a1839c79fc17','b4d5b8d2-7b32-4546-8a67-71ca36b7841a','dbdd2d24-bc46-46a8-815d-f0dfae35c2a2','PRESENT','DONE','Hào hứng tham gia hoạt động','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 4','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('9cdaa815-f085-46a7-8f86-ebbbbc07233b','4f7544c6-e93a-4e26-8a83-723a6cdd6228','787be491-3d9a-4475-960c-9d3fbbc99a32','LATE','NOT_DONE','Hào hứng tham gia hoạt động','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 4','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('9d941db4-6afc-46d7-bf0b-ee9b4e170b9f','3c3e3ddf-9359-423f-91b3-2692dd37e6bb','f277ae6e-4895-43db-98bf-ae0c1493586c','PRESENT','DONE','Hào hứng tham gia hoạt động','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 2','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('9dbb9637-1095-43c4-80c4-785b9af51d54','af6919e7-8b4c-479a-a44c-077b36d4e7bc','dbdd2d24-bc46-46a8-815d-f0dfae35c2a2','PRESENT','NOT_DONE','Hào hứng tham gia hoạt động','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 6','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('a1a65d51-ea51-46c0-90f1-1f45f8570ba4','c6e2a726-d23f-422b-be29-a8f6e1abbe35','8dd66899-c8dc-4e92-a8d3-86b0e3dc54b7','PRESENT','DONE','Hào hứng tham gia hoạt động','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 1','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('a457b701-7787-4d04-b5b7-4d79c2b0e7c6','b4d5b8d2-7b32-4546-8a67-71ca36b7841a','8dd66899-c8dc-4e92-a8d3-86b0e3dc54b7','PRESENT','NOT_DONE','Cần khuyến khích tham gia hơn','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 4','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('a5f9494f-2a73-4d6d-9110-03a0c8e52353','a57b0138-243e-48cc-9e93-6bad1cda7a62','f277ae6e-4895-43db-98bf-ae0c1493586c','PRESENT','NOT_DONE','Hào hứng tham gia hoạt động','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 2','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('a6c892d3-3faa-482e-981e-370c9edafce3','b8568717-f78f-40d1-985c-dcd963a4b636','787be491-3d9a-4475-960c-9d3fbbc99a32','PRESENT','NOT_DONE','Cần khuyến khích tham gia hơn','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 3','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('a78368c5-ba06-4f24-87d7-12648cd9e68b','17717904-06c0-4326-ae62-6cf74ef643cd','787be491-3d9a-4475-960c-9d3fbbc99a32','PRESENT','DONE','Hào hứng tham gia hoạt động','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 1','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('a7bb5eb1-866f-41af-9f4f-16a912884af0','1154c952-b710-45d1-9149-c22526a4ff1a','f277ae6e-4895-43db-98bf-ae0c1493586c','PRESENT','NOT_DONE','Cần khuyến khích tham gia hơn','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 4','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('a9fb87e8-cc08-4bca-9f87-50ce85963d2f','ce2cdf85-482a-4885-80e2-64a7237b23f1','9926b55b-dc45-46b8-8061-c9795625a11c','PRESENT','DONE','Hào hứng tham gia hoạt động','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 4','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('aa9c22d8-a667-43d4-9aaf-54a3bc2998fc','5ea4474e-aed8-4034-ba08-4a0e4aab2cb2','8dd66899-c8dc-4e92-a8d3-86b0e3dc54b7','ABSENT','DONE','Cần khuyến khích tham gia hơn','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 5','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('ab1f511c-a30b-41c1-a24c-516ad9fb4478','3991101f-415f-4f95-bc52-0d0d344d5df4','9926b55b-dc45-46b8-8061-c9795625a11c','PRESENT','DONE','Hào hứng tham gia hoạt động','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 1','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('adfc510e-d219-4e18-ba71-25a23d6ab1fd','3991101f-415f-4f95-bc52-0d0d344d5df4','f277ae6e-4895-43db-98bf-ae0c1493586c','ABSENT','DONE','Hào hứng tham gia hoạt động','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 1','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('b848ea7d-b347-4867-80eb-6b1d385b134a','17717904-06c0-4326-ae62-6cf74ef643cd','8dd66899-c8dc-4e92-a8d3-86b0e3dc54b7','PRESENT','NOT_DONE','Hào hứng tham gia hoạt động','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 1','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('b8a74f76-99ab-443b-9ea1-0cd34ec6f2ef','5e69f33f-8ec9-4480-a792-b39e35c6b369','787be491-3d9a-4475-960c-9d3fbbc99a32','PRESENT','DONE','Hào hứng tham gia hoạt động','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 3','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('b8ffbd61-c14e-46b5-946a-1e0de4eecf70','b4d5b8d2-7b32-4546-8a67-71ca36b7841a','51bd7a04-f28f-4813-8312-88c20180a97f','PRESENT','DONE','Cần khuyến khích tham gia hơn','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 4','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('b9579092-219d-4d44-9114-e2193c95cfe6','51e7dccd-8ca2-495a-8845-3999e325a2ef','f277ae6e-4895-43db-98bf-ae0c1493586c','PRESENT','DONE','Cần khuyến khích tham gia hơn','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 3','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('b98f9532-66f8-42ae-aed4-8a38d7fffc54','1154c952-b710-45d1-9149-c22526a4ff1a','8dd66899-c8dc-4e92-a8d3-86b0e3dc54b7','PRESENT','DONE','Hào hứng tham gia hoạt động','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 4','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('bd624950-2832-4651-947f-46724c99769a','96157490-73c5-49ed-b227-a2ef1fc33d2d','787be491-3d9a-4475-960c-9d3fbbc99a32','PRESENT','DONE','Cần khuyến khích tham gia hơn','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 2','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('bd82dc3f-1523-4f3d-a408-fc95f515d6e7','24db0296-7762-42b6-99b1-7bb564b707c1','dbdd2d24-bc46-46a8-815d-f0dfae35c2a2','LATE','DONE','Cần khuyến khích tham gia hơn','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 4','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('c2380b7c-4222-403b-87e1-1345463303ab','5ea4474e-aed8-4034-ba08-4a0e4aab2cb2','9926b55b-dc45-46b8-8061-c9795625a11c','PRESENT','DONE','Hào hứng tham gia hoạt động','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 5','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('c2978e12-a3fd-47ef-8942-c9e22792fa84','51e7dccd-8ca2-495a-8845-3999e325a2ef','51bd7a04-f28f-4813-8312-88c20180a97f','PRESENT','DONE','Hào hứng tham gia hoạt động','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 3','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('c4c2988c-cef3-49aa-944b-6aa57470b4f9','4a46e5f3-25d2-4908-b02b-1bb06418a7c5','8dd66899-c8dc-4e92-a8d3-86b0e3dc54b7','PRESENT','NOT_DONE','Hào hứng tham gia hoạt động','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 2','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('c56fd187-f5f6-4839-ac6b-a2c65b823436','51e7dccd-8ca2-495a-8845-3999e325a2ef','8dd66899-c8dc-4e92-a8d3-86b0e3dc54b7','LATE','DONE','Hào hứng tham gia hoạt động','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 3','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('c5ac0789-ad50-4955-beba-b0ac3f2a270e','99c23cd1-9a07-4a83-a486-818c6bccd0bb','8dd66899-c8dc-4e92-a8d3-86b0e3dc54b7','PRESENT','DONE','Hào hứng tham gia hoạt động','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 6','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('c5e2d680-9e80-4c5c-8201-ed6e8e0a4c06','ba957152-32e2-469d-b071-3292207507ba','dbdd2d24-bc46-46a8-815d-f0dfae35c2a2','PRESENT','DONE','Hào hứng tham gia hoạt động','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 3','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('c88d96fb-31f7-40ae-895a-f3d9c64b7903','24db0296-7762-42b6-99b1-7bb564b707c1','787be491-3d9a-4475-960c-9d3fbbc99a32','PRESENT','DONE','Hào hứng tham gia hoạt động','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 4','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('caa1b247-7102-43e2-9c89-1d087a432c0c','884504e2-2663-48a0-8d48-cd5c375c6b10','f277ae6e-4895-43db-98bf-ae0c1493586c','PRESENT','DONE','Cần khuyến khích tham gia hơn','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 5','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('cc40712d-32c4-4c88-b968-7cddec03c55d','99c23cd1-9a07-4a83-a486-818c6bccd0bb','51bd7a04-f28f-4813-8312-88c20180a97f','PRESENT','NOT_DONE','Hào hứng tham gia hoạt động','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 6','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('cd14526a-f310-4763-9c4a-944bba85b9d5','96157490-73c5-49ed-b227-a2ef1fc33d2d','9926b55b-dc45-46b8-8061-c9795625a11c','PRESENT','NOT_DONE','Hào hứng tham gia hoạt động','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 2','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('cf396221-c88f-4197-99be-1cc72f2e1098','17717904-06c0-4326-ae62-6cf74ef643cd','51bd7a04-f28f-4813-8312-88c20180a97f','PRESENT','DONE','Hào hứng tham gia hoạt động','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 1','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('d1fd1e0b-1fb0-4805-9fcd-dd57fe63ae80','ce2cdf85-482a-4885-80e2-64a7237b23f1','dbdd2d24-bc46-46a8-815d-f0dfae35c2a2','PRESENT','DONE','Hào hứng tham gia hoạt động','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 4','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('d3fa54dc-0326-44dc-b64f-e2397b45712b','1154c952-b710-45d1-9149-c22526a4ff1a','9926b55b-dc45-46b8-8061-c9795625a11c','ABSENT','DONE','Cần khuyến khích tham gia hơn','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 4','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('d40b4933-6040-4258-a329-653422262c29','4f7544c6-e93a-4e26-8a83-723a6cdd6228','8dd66899-c8dc-4e92-a8d3-86b0e3dc54b7','PRESENT','DONE','Hào hứng tham gia hoạt động','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 4','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('d72e3eba-1216-4e91-adc3-4cce954dce7f','3f3be23c-a430-4fd2-9405-770cf5f4e4ab','dbdd2d24-bc46-46a8-815d-f0dfae35c2a2','ABSENT','DONE','Cần khuyến khích tham gia hơn','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 2','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('d78a1e6e-1ad3-4c30-aba5-85a166123cf6','99c23cd1-9a07-4a83-a486-818c6bccd0bb','9926b55b-dc45-46b8-8061-c9795625a11c','PRESENT','DONE','Cần khuyến khích tham gia hơn','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 6','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('d80818b2-4e1c-488b-900a-41a1d9990ad4','c6e2a726-d23f-422b-be29-a8f6e1abbe35','9926b55b-dc45-46b8-8061-c9795625a11c','ABSENT','DONE','Cần khuyến khích tham gia hơn','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 1','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('dab9c15c-29e5-437e-b75e-e4fce09162ea','e9bcc72a-e633-4419-9d61-bc0888d4feaa','dbdd2d24-bc46-46a8-815d-f0dfae35c2a2','PRESENT','NOT_DONE','Cần khuyến khích tham gia hơn','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 5','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('dadcc034-bdd8-400b-b412-181c97c250dc','e9bcc72a-e633-4419-9d61-bc0888d4feaa','787be491-3d9a-4475-960c-9d3fbbc99a32','PRESENT','NOT_DONE','Hào hứng tham gia hoạt động','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 5','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('dc0c3c74-136d-4680-b65b-1dad55cc2bd8','4a46e5f3-25d2-4908-b02b-1bb06418a7c5','51bd7a04-f28f-4813-8312-88c20180a97f','LATE','DONE','Hào hứng tham gia hoạt động','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 2','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('dd1e2b1f-b0bf-4122-a9bb-50a66ff70d51','5ea4474e-aed8-4034-ba08-4a0e4aab2cb2','dbdd2d24-bc46-46a8-815d-f0dfae35c2a2','PRESENT','DONE','Hào hứng tham gia hoạt động','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 5','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('ded3544c-b152-4693-acbf-8dc0b27a728f','99c23cd1-9a07-4a83-a486-818c6bccd0bb','dbdd2d24-bc46-46a8-815d-f0dfae35c2a2','LATE','DONE','Cần khuyến khích tham gia hơn','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 6','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('e63e4b79-1c19-4416-af23-91f1085363d8','99c23cd1-9a07-4a83-a486-818c6bccd0bb','787be491-3d9a-4475-960c-9d3fbbc99a32','ABSENT','DONE','Hào hứng tham gia hoạt động','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 6','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('ea616821-3aac-4264-b528-df397ab61c52','ce2cdf85-482a-4885-80e2-64a7237b23f1','51bd7a04-f28f-4813-8312-88c20180a97f','PRESENT','NOT_DONE','Cần khuyến khích tham gia hơn','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 4','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('ebc668f3-a493-4f59-b712-5311cf3abd63','af6919e7-8b4c-479a-a44c-077b36d4e7bc','9926b55b-dc45-46b8-8061-c9795625a11c','ABSENT','DONE','Hào hứng tham gia hoạt động','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 6','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('ef13e916-79a1-4567-9df9-39cad2eeccff','ce2cdf85-482a-4885-80e2-64a7237b23f1','8dd66899-c8dc-4e92-a8d3-86b0e3dc54b7','LATE','DONE','Cần khuyến khích tham gia hơn','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 4','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('ef371764-43f0-4228-884b-4527ee94ada7','5e69f33f-8ec9-4480-a792-b39e35c6b369','8dd66899-c8dc-4e92-a8d3-86b0e3dc54b7','PRESENT','DONE','Hào hứng tham gia hoạt động','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 3','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('f33f6ebc-d822-42eb-8644-2d926cd2fc1a','36624795-e289-41e3-a1d7-558dcaeb3c10','f277ae6e-4895-43db-98bf-ae0c1493586c','PRESENT','NOT_DONE','Hào hứng tham gia hoạt động','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 1','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('f700376f-707c-48ab-a076-f6155298e8e2','3c3e3ddf-9359-423f-91b3-2692dd37e6bb','51bd7a04-f28f-4813-8312-88c20180a97f','PRESENT','DONE','Cần khuyến khích tham gia hơn','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 2','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('f713ddfb-4061-4258-a79a-1eb973376ed5','ce2cdf85-482a-4885-80e2-64a7237b23f1','787be491-3d9a-4475-960c-9d3fbbc99a32','PRESENT','DONE','Cần khuyến khích tham gia hơn','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 4','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('f729d8f7-80a3-4634-bfe4-7fc4215e62cb','3c3e3ddf-9359-423f-91b3-2692dd37e6bb','8dd66899-c8dc-4e92-a8d3-86b0e3dc54b7','ABSENT','DONE','Cần khuyến khích tham gia hơn','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 2','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('f8368b36-b8eb-4003-a7c9-d3def3838f79','36624795-e289-41e3-a1d7-558dcaeb3c10','51bd7a04-f28f-4813-8312-88c20180a97f','LATE','NOT_DONE','Cần khuyến khích tham gia hơn','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 1','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('f8e83e9b-65a6-4fee-a74d-49743a7a02b0','52dbdab2-9652-4f7e-b985-0a8616caa2bd','dbdd2d24-bc46-46a8-815d-f0dfae35c2a2','LATE','NOT_DONE','Hào hứng tham gia hoạt động','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 5','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('fa91b4b5-ebe4-4b4c-bab4-22715815e474','0a757c99-07b4-4e71-b44f-e6993fb69018','787be491-3d9a-4475-960c-9d3fbbc99a32','PRESENT','DONE','Cần khuyến khích tham gia hơn','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 6','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('fabeacfa-b21b-41c8-bdbd-52ddff0db1ce','b8568717-f78f-40d1-985c-dcd963a4b636','9926b55b-dc45-46b8-8061-c9795625a11c','PRESENT','DONE','Hào hứng tham gia hoạt động','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 3','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('faeac3ac-5126-4f68-9b47-2ef601c29345','884504e2-2663-48a0-8d48-cd5c375c6b10','dbdd2d24-bc46-46a8-815d-f0dfae35c2a2','ABSENT','DONE','Cần khuyến khích tham gia hơn','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 5','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('fb0f8335-01da-49c3-82f7-4418d98ea65d','a57b0138-243e-48cc-9e93-6bad1cda7a62','8dd66899-c8dc-4e92-a8d3-86b0e3dc54b7','PRESENT','DONE','Cần khuyến khích tham gia hơn','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 2','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('fc3a5073-a5f6-4ff8-af1d-12cef4d9ec4f','ba957152-32e2-469d-b071-3292207507ba','f277ae6e-4895-43db-98bf-ae0c1493586c','LATE','DONE','Hào hứng tham gia hoạt động','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 3','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('fe79bad9-80cf-4c37-b0ff-f4b253260160','b8568717-f78f-40d1-985c-dcd963a4b636','f277ae6e-4895-43db-98bf-ae0c1493586c','PRESENT','DONE','Hào hứng tham gia hoạt động','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 3','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('ff6ad187-f0ee-4cae-82da-b470dcd36f00','36624795-e289-41e3-a1d7-558dcaeb3c10','9926b55b-dc45-46b8-8061-c9795625a11c','PRESENT','DONE','Hào hứng tham gia hoạt động','Ngoan, vui vẻ, hòa đồng','Bé hoàn thành tốt hoạt động của chủ đề 1','9c599cba-54c3-48f8-82f8-e0b43c9126b4','2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304');
/*!40000 ALTER TABLE `session_feedback` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `session_scores`
--

DROP TABLE IF EXISTS `session_scores`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `session_scores` (
  `id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `session_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `student_id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `score_type` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `language` int DEFAULT NULL,
  `cognitive` int DEFAULT NULL,
  `physical` int DEFAULT NULL,
  `social` int DEFAULT NULL,
  `total` int NOT NULL,
  `created_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  `updated_at` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  PRIMARY KEY (`id`),
  UNIQUE KEY `ix_session_scores_session_id_student_id` (`session_id`,`student_id`),
  KEY `ix_session_scores_student_id` (`student_id`),
  CONSTRAINT `fk_session_scores_sessions_session_id` FOREIGN KEY (`session_id`) REFERENCES `sessions` (`id`) ON DELETE RESTRICT,
  CONSTRAINT `fk_session_scores_students_student_id` FOREIGN KEY (`student_id`) REFERENCES `students` (`id`) ON DELETE RESTRICT,
  CONSTRAINT `ck_session_scores_cognitive` CHECK ((`cognitive` between 0 and 100)),
  CONSTRAINT `ck_session_scores_language` CHECK ((`language` between 0 and 100)),
  CONSTRAINT `ck_session_scores_physical` CHECK ((`physical` between 0 and 100)),
  CONSTRAINT `ck_session_scores_score_type` CHECK ((`score_type` in (_utf8mb4'TEST',_utf8mb4'MIDTERM',_utf8mb4'FINAL'))),
  CONSTRAINT `ck_session_scores_social` CHECK ((`social` between 0 and 100)),
  CONSTRAINT `ck_session_scores_total` CHECK ((`total` between 0 and 100))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `session_scores`
--

LOCK TABLES `session_scores` WRITE;
/*!40000 ALTER TABLE `session_scores` DISABLE KEYS */;
INSERT INTO `session_scores` VALUES ('059260de-f37a-447d-8219-560f8a511fb8','1154c952-b710-45d1-9149-c22526a4ff1a','787be491-3d9a-4475-960c-9d3fbbc99a32','TEST',92,87,82,77,90,'2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('09638656-8eda-4f23-847c-7c000f97580a','1154c952-b710-45d1-9149-c22526a4ff1a','f277ae6e-4895-43db-98bf-ae0c1493586c','TEST',87,82,77,92,85,'2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('1821d2ac-d00c-410e-8dcc-c5cadbd57e83','a57b0138-243e-48cc-9e93-6bad1cda7a62','787be491-3d9a-4475-960c-9d3fbbc99a32','TEST',83,78,73,88,81,'2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('4057c111-d07e-485d-b3af-5d2b91c84a37','1154c952-b710-45d1-9149-c22526a4ff1a','51bd7a04-f28f-4813-8312-88c20180a97f','TEST',90,85,80,75,88,'2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('79d8fcc5-afc9-4236-8624-21aac0bfdcfc','1154c952-b710-45d1-9149-c22526a4ff1a','8dd66899-c8dc-4e92-a8d3-86b0e3dc54b7','TEST',88,83,78,93,86,'2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('7f03fa3e-923c-4cd6-a762-6fe0e42b51dd','1154c952-b710-45d1-9149-c22526a4ff1a','dbdd2d24-bc46-46a8-815d-f0dfae35c2a2','TEST',89,84,79,94,87,'2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('b1d4f054-9cc0-4023-9d5b-3248fd0bb5a8','1154c952-b710-45d1-9149-c22526a4ff1a','9926b55b-dc45-46b8-8061-c9795625a11c','TEST',91,86,81,76,89,'2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('bc0709ee-6bad-41a3-bf2b-ec800a8dc576','a57b0138-243e-48cc-9e93-6bad1cda7a62','8dd66899-c8dc-4e92-a8d3-86b0e3dc54b7','TEST',79,74,69,84,77,'2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('c031b429-a540-49df-b550-0e1a19d3a3b4','a57b0138-243e-48cc-9e93-6bad1cda7a62','f277ae6e-4895-43db-98bf-ae0c1493586c','TEST',78,73,68,83,76,'2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('c0d26422-fc5c-4b43-9920-1f8e39e65bf7','a57b0138-243e-48cc-9e93-6bad1cda7a62','51bd7a04-f28f-4813-8312-88c20180a97f','TEST',81,76,71,86,79,'2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('db44e30a-dc74-4f92-92e7-b303dc8f12c6','a57b0138-243e-48cc-9e93-6bad1cda7a62','9926b55b-dc45-46b8-8061-c9795625a11c','TEST',82,77,72,87,80,'2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304'),('faa9fa23-443a-4d27-9392-dae742a3cdb8','a57b0138-243e-48cc-9e93-6bad1cda7a62','dbdd2d24-bc46-46a8-815d-f0dfae35c2a2','TEST',80,75,70,85,78,'2026-11-06 14:37:24.917304','2026-11-06 14:37:24.917304');
/*!40000 ALTER TABLE `session_scores` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `sessions`
--

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

--
-- Dumping data for table `sessions`
--

LOCK TABLES `sessions` WRITE;
/*!40000 ALTER TABLE `sessions` DISABLE KEYS */;
INSERT INTO `sessions` VALUES ('02c81761-1cfc-4e0c-8b11-9133ad7099a4','5c7b6dae-a17d-448a-a31e-a57039e1e5d5','2027-01-25',12,4,'NORMAL','6db6f765-0251-434c-b5a2-13f6b9af8dde',NULL,'2026-11-06 14:37:26.507879'),('068d9dbf-bfbc-4c3e-891b-401add344af1','c1ec3be8-d7b3-438b-9eb3-f982e5018891','2026-11-16',8,1,'NORMAL','9c599cba-54c3-48f8-82f8-e0b43c9126b4',NULL,'2026-11-06 14:37:26.363614'),('0707c9bd-096f-478b-b3be-e6e4495047ce','5c7b6dae-a17d-448a-a31e-a57039e1e5d5','2026-11-13',1,1,'NORMAL','6db6f765-0251-434c-b5a2-13f6b9af8dde',NULL,'2026-11-06 14:37:26.507674'),('0736c435-3537-4c49-8e54-d09a70d6e040','5c7b6dae-a17d-448a-a31e-a57039e1e5d5','2027-01-18',11,3,'NORMAL','6db6f765-0251-434c-b5a2-13f6b9af8dde',NULL,'2026-11-06 14:37:26.507853'),('0a757c99-07b4-4e71-b44f-e6993fb69018','c1ec3be8-d7b3-438b-9eb3-f982e5018891','2026-11-03',6,1,'NORMAL','9c599cba-54c3-48f8-82f8-e0b43c9126b4',NULL,'2026-11-06 14:37:26.363587'),('0b41cb1f-df17-469e-a0b1-370fbcb5ef7d','5c7b6dae-a17d-448a-a31e-a57039e1e5d5','2027-01-01',8,7,'TEST','6db6f765-0251-434c-b5a2-13f6b9af8dde',NULL,'2026-11-06 14:37:26.507819'),('0d8aac4d-fff8-46fe-b154-e3e4f47e9121','5c7b6dae-a17d-448a-a31e-a57039e1e5d5','2026-12-22',7,1,'NORMAL','6db6f765-0251-434c-b5a2-13f6b9af8dde',NULL,'2026-11-06 14:37:26.507789'),('0e7750d3-ed34-4c1e-b3d6-b6f48db8aff9','5c7b6dae-a17d-448a-a31e-a57039e1e5d5','2026-12-03',4,3,'NORMAL','6db6f765-0251-434c-b5a2-13f6b9af8dde',NULL,'2026-11-06 14:37:26.507729'),('0ec7e0cc-42d5-44dc-b6d9-aa5bc5c01792','c1ec3be8-d7b3-438b-9eb3-f982e5018891','2026-11-18',8,4,'NORMAL','9c599cba-54c3-48f8-82f8-e0b43c9126b4',NULL,'2026-11-06 14:37:26.363620'),('1150d3b2-41a3-4cd4-9395-837f328b744f','5c7b6dae-a17d-448a-a31e-a57039e1e5d5','2027-01-27',12,7,'FINAL','6db6f765-0251-434c-b5a2-13f6b9af8dde',NULL,'2026-11-06 14:37:26.507885'),('1154c952-b710-45d1-9149-c22526a4ff1a','c1ec3be8-d7b3-438b-9eb3-f982e5018891','2026-10-27',4,7,'TEST','9c599cba-54c3-48f8-82f8-e0b43c9126b4',NULL,'2026-11-06 14:37:26.363544'),('163068a6-52f8-4c8e-a97e-b7056f24164b','c1ec3be8-d7b3-438b-9eb3-f982e5018891','2026-12-07',11,3,'NORMAL','9c599cba-54c3-48f8-82f8-e0b43c9126b4',NULL,'2026-11-06 14:37:26.363660'),('17717904-06c0-4326-ae62-6cf74ef643cd','c1ec3be8-d7b3-438b-9eb3-f982e5018891','2026-10-05',1,3,'NORMAL','9c599cba-54c3-48f8-82f8-e0b43c9126b4',NULL,'2026-11-06 14:37:26.363491'),('1914e712-4e82-411d-998b-04467f7d5294','c1ec3be8-d7b3-438b-9eb3-f982e5018891','2026-11-30',10,3,'NORMAL','9c599cba-54c3-48f8-82f8-e0b43c9126b4',NULL,'2026-11-06 14:37:26.363645'),('205ea283-3ab2-477c-8958-e197a0365a55','c1ec3be8-d7b3-438b-9eb3-f982e5018891','2026-11-20',8,7,'TEST','9c599cba-54c3-48f8-82f8-e0b43c9126b4',NULL,'2026-11-06 14:37:26.363627'),('24db0296-7762-42b6-99b1-7bb564b707c1','c1ec3be8-d7b3-438b-9eb3-f982e5018891','2026-10-21',4,1,'NORMAL','9c599cba-54c3-48f8-82f8-e0b43c9126b4',NULL,'2026-11-06 14:37:26.363530'),('2aeb9b6a-39d8-48bc-a0dd-d1b30bd8f946','5c7b6dae-a17d-448a-a31e-a57039e1e5d5','2027-01-26',12,6,'NORMAL','6db6f765-0251-434c-b5a2-13f6b9af8dde',NULL,'2026-11-06 14:37:26.507882'),('2bdbe0ce-c845-414c-9aa9-ec5790f2148f','c1ec3be8-d7b3-438b-9eb3-f982e5018891','2026-12-02',10,6,'NORMAL','9c599cba-54c3-48f8-82f8-e0b43c9126b4',NULL,'2026-11-06 14:37:26.363651'),('2d9c1118-9b8c-43ae-9734-8b902905054d','5c7b6dae-a17d-448a-a31e-a57039e1e5d5','2027-01-13',10,6,'NORMAL','6db6f765-0251-434c-b5a2-13f6b9af8dde',NULL,'2026-11-06 14:37:26.507844'),('2eb8a252-6610-435b-914c-65a67b101394','5c7b6dae-a17d-448a-a31e-a57039e1e5d5','2027-01-06',9,4,'NORMAL','6db6f765-0251-434c-b5a2-13f6b9af8dde',NULL,'2026-11-06 14:37:26.507830'),('3062cfe5-7a1c-4d6d-914b-5fc24189f1f8','5c7b6dae-a17d-448a-a31e-a57039e1e5d5','2027-01-21',12,1,'NORMAL','6db6f765-0251-434c-b5a2-13f6b9af8dde',NULL,'2026-11-06 14:37:26.507861'),('36624795-e289-41e3-a1d7-558dcaeb3c10','c1ec3be8-d7b3-438b-9eb3-f982e5018891','2026-10-06',1,4,'NORMAL','9c599cba-54c3-48f8-82f8-e0b43c9126b4',NULL,'2026-11-06 14:37:26.363496'),('3991101f-415f-4f95-bc52-0d0d344d5df4','c1ec3be8-d7b3-438b-9eb3-f982e5018891','2026-10-02',1,1,'NORMAL','9c599cba-54c3-48f8-82f8-e0b43c9126b4',NULL,'2026-11-06 14:37:26.363075'),('3c3e3ddf-9359-423f-91b3-2692dd37e6bb','c1ec3be8-d7b3-438b-9eb3-f982e5018891','2026-10-12',2,4,'NORMAL','9c599cba-54c3-48f8-82f8-e0b43c9126b4',NULL,'2026-11-06 14:37:26.363510'),('3f3be23c-a430-4fd2-9405-770cf5f4e4ab','c1ec3be8-d7b3-438b-9eb3-f982e5018891','2026-10-09',2,3,'NORMAL','9c599cba-54c3-48f8-82f8-e0b43c9126b4',NULL,'2026-11-06 14:37:26.363507'),('42b286c4-a727-44ef-89da-b7a84c7e1beb','c1ec3be8-d7b3-438b-9eb3-f982e5018891','2026-11-25',9,4,'NORMAL','9c599cba-54c3-48f8-82f8-e0b43c9126b4',NULL,'2026-11-06 14:37:26.363636'),('45ec7d6c-6663-452e-acd4-b52eade8d040','5c7b6dae-a17d-448a-a31e-a57039e1e5d5','2027-01-15',11,1,'NORMAL','6db6f765-0251-434c-b5a2-13f6b9af8dde',NULL,'2026-11-06 14:37:26.507850'),('4962579f-d214-4fad-89a2-7e17a0e1d610','c1ec3be8-d7b3-438b-9eb3-f982e5018891','2026-11-27',10,1,'NORMAL','9c599cba-54c3-48f8-82f8-e0b43c9126b4',NULL,'2026-11-06 14:37:26.363642'),('49fb1e02-e8bb-45b9-a56d-eea25102b57b','5c7b6dae-a17d-448a-a31e-a57039e1e5d5','2026-12-08',4,7,'TEST','6db6f765-0251-434c-b5a2-13f6b9af8dde',NULL,'2026-11-06 14:37:26.507759'),('4a46e5f3-25d2-4908-b02b-1bb06418a7c5','c1ec3be8-d7b3-438b-9eb3-f982e5018891','2026-10-13',2,6,'NORMAL','9c599cba-54c3-48f8-82f8-e0b43c9126b4',NULL,'2026-11-06 14:37:26.363512'),('4f7544c6-e93a-4e26-8a83-723a6cdd6228','c1ec3be8-d7b3-438b-9eb3-f982e5018891','2026-10-23',4,4,'NORMAL','9c599cba-54c3-48f8-82f8-e0b43c9126b4',NULL,'2026-11-06 14:37:26.363536'),('4fae9887-14d0-47b9-b920-9b842065864b','c1ec3be8-d7b3-438b-9eb3-f982e5018891','2026-12-14',12,4,'NORMAL','9c599cba-54c3-48f8-82f8-e0b43c9126b4',NULL,'2026-11-06 14:37:26.363675'),('4fd70599-1dab-4298-a2bc-909cb22b8244','5c7b6dae-a17d-448a-a31e-a57039e1e5d5','2026-12-21',6,7,'MIDTERM','6db6f765-0251-434c-b5a2-13f6b9af8dde',NULL,'2026-11-06 14:37:26.507786'),('51e7dccd-8ca2-495a-8845-3999e325a2ef','c1ec3be8-d7b3-438b-9eb3-f982e5018891','2026-10-15',3,1,'NORMAL','9c599cba-54c3-48f8-82f8-e0b43c9126b4',NULL,'2026-11-06 14:37:26.363518'),('52dbdab2-9652-4f7e-b985-0a8616caa2bd','c1ec3be8-d7b3-438b-9eb3-f982e5018891','2026-10-28',5,1,'NORMAL','9c599cba-54c3-48f8-82f8-e0b43c9126b4',NULL,'2026-11-06 14:37:26.363547'),('54d5ab70-5bb7-4c1d-bf9d-2bfbff5d3b0b','5c7b6dae-a17d-448a-a31e-a57039e1e5d5','2026-11-16',1,3,'NORMAL','6db6f765-0251-434c-b5a2-13f6b9af8dde',NULL,'2026-11-06 14:37:26.507680'),('5dba7d95-a697-4160-859e-ab432faa7839','c1ec3be8-d7b3-438b-9eb3-f982e5018891','2026-11-12',7,4,'NORMAL','9c599cba-54c3-48f8-82f8-e0b43c9126b4',NULL,'2026-11-06 14:37:26.363608'),('5e69f33f-8ec9-4480-a792-b39e35c6b369','c1ec3be8-d7b3-438b-9eb3-f982e5018891','2026-10-19',3,4,'NORMAL','9c599cba-54c3-48f8-82f8-e0b43c9126b4',NULL,'2026-11-06 14:37:26.363524'),('5ea4474e-aed8-4034-ba08-4a0e4aab2cb2','c1ec3be8-d7b3-438b-9eb3-f982e5018891','2026-10-30',5,4,'NORMAL','9c599cba-54c3-48f8-82f8-e0b43c9126b4',NULL,'2026-11-06 14:37:26.363553'),('60228fc0-b52c-45df-a296-86d2d118a64a','5c7b6dae-a17d-448a-a31e-a57039e1e5d5','2026-12-28',8,1,'NORMAL','6db6f765-0251-434c-b5a2-13f6b9af8dde',NULL,'2026-11-06 14:37:26.507801'),('63263b3c-069a-4402-8133-bffe39c9098b','5c7b6dae-a17d-448a-a31e-a57039e1e5d5','2026-11-26',3,1,'NORMAL','6db6f765-0251-434c-b5a2-13f6b9af8dde',NULL,'2026-11-06 14:37:26.507711'),('68728183-a0b9-4ebd-9ee4-f4ada7004e24','5c7b6dae-a17d-448a-a31e-a57039e1e5d5','2026-12-16',6,3,'NORMAL','6db6f765-0251-434c-b5a2-13f6b9af8dde',NULL,'2026-11-06 14:37:26.507777'),('696863a7-be3d-4750-99dc-2d522cf54dbc','c1ec3be8-d7b3-438b-9eb3-f982e5018891','2026-12-04',11,1,'NORMAL','9c599cba-54c3-48f8-82f8-e0b43c9126b4',NULL,'2026-11-06 14:37:26.363657'),('71c49fbf-9c49-49f5-864c-19953110a1c8','c1ec3be8-d7b3-438b-9eb3-f982e5018891','2026-11-06',6,6,'NORMAL','9c599cba-54c3-48f8-82f8-e0b43c9126b4',NULL,'2026-11-06 14:37:26.363595'),('784e60b6-5907-4f14-830a-00adb991bb0e','5c7b6dae-a17d-448a-a31e-a57039e1e5d5','2026-11-17',1,4,'NORMAL','6db6f765-0251-434c-b5a2-13f6b9af8dde',NULL,'2026-11-06 14:37:26.507685'),('79e8883b-d10b-4f5c-9892-acf1b5190e25','c1ec3be8-d7b3-438b-9eb3-f982e5018891','2026-11-24',9,3,'NORMAL','9c599cba-54c3-48f8-82f8-e0b43c9126b4',NULL,'2026-11-06 14:37:26.363633'),('8326ba86-b814-4c61-9758-52f6f9c0c572','5c7b6dae-a17d-448a-a31e-a57039e1e5d5','2027-01-14',10,7,'TEST','6db6f765-0251-434c-b5a2-13f6b9af8dde',NULL,'2026-11-06 14:37:26.507847'),('84a7ef00-8e07-46f5-90fa-b04bbc08d5d0','5c7b6dae-a17d-448a-a31e-a57039e1e5d5','2026-12-17',6,4,'NORMAL','6db6f765-0251-434c-b5a2-13f6b9af8dde',NULL,'2026-11-06 14:37:26.507780'),('876187e5-527a-4738-89da-60403dcc08c4','5c7b6dae-a17d-448a-a31e-a57039e1e5d5','2026-11-24',2,6,'NORMAL','6db6f765-0251-434c-b5a2-13f6b9af8dde',NULL,'2026-11-06 14:37:26.507702'),('8809f4be-a9b1-4125-81a6-8e95ad2807c6','c1ec3be8-d7b3-438b-9eb3-f982e5018891','2026-11-10',7,1,'NORMAL','9c599cba-54c3-48f8-82f8-e0b43c9126b4',NULL,'2026-11-06 14:37:26.363601'),('884504e2-2663-48a0-8d48-cd5c375c6b10','c1ec3be8-d7b3-438b-9eb3-f982e5018891','2026-10-29',5,3,'NORMAL','9c599cba-54c3-48f8-82f8-e0b43c9126b4',NULL,'2026-11-06 14:37:26.363550'),('8a142a55-a63f-4c88-95dc-d580b78a809e','c1ec3be8-d7b3-438b-9eb3-f982e5018891','2026-11-13',7,6,'NORMAL','9c599cba-54c3-48f8-82f8-e0b43c9126b4',NULL,'2026-11-06 14:37:26.363611'),('8d0f0941-7517-4537-bd29-70f7e2a1e8d8','5c7b6dae-a17d-448a-a31e-a57039e1e5d5','2026-12-23',7,3,'NORMAL','6db6f765-0251-434c-b5a2-13f6b9af8dde',NULL,'2026-11-06 14:37:26.507792'),('8e970cae-da95-4a62-a755-0c5618b0f6c0','5c7b6dae-a17d-448a-a31e-a57039e1e5d5','2026-12-02',4,1,'NORMAL','6db6f765-0251-434c-b5a2-13f6b9af8dde',NULL,'2026-11-06 14:37:26.507725'),('8eaf7a88-7ca8-4b52-8a2f-33eaa5e3f2fc','5c7b6dae-a17d-448a-a31e-a57039e1e5d5','2026-12-30',8,4,'NORMAL','6db6f765-0251-434c-b5a2-13f6b9af8dde',NULL,'2026-11-06 14:37:26.507813'),('943eca08-0666-41bb-a860-f0f41e071c96','5c7b6dae-a17d-448a-a31e-a57039e1e5d5','2026-11-23',2,4,'NORMAL','6db6f765-0251-434c-b5a2-13f6b9af8dde',NULL,'2026-11-06 14:37:26.507699'),('95d77086-dcc5-4cca-a0f5-e24d9c5dcf10','c1ec3be8-d7b3-438b-9eb3-f982e5018891','2026-12-10',12,1,'NORMAL','9c599cba-54c3-48f8-82f8-e0b43c9126b4',NULL,'2026-11-06 14:37:26.363669'),('96157490-73c5-49ed-b227-a2ef1fc33d2d','c1ec3be8-d7b3-438b-9eb3-f982e5018891','2026-10-08',2,1,'NORMAL','9c599cba-54c3-48f8-82f8-e0b43c9126b4',NULL,'2026-11-06 14:37:26.363502'),('972d41a7-39bb-44fd-930d-0279a9024283','c1ec3be8-d7b3-438b-9eb3-f982e5018891','2026-12-15',12,6,'NORMAL','9c599cba-54c3-48f8-82f8-e0b43c9126b4',NULL,'2026-11-06 14:37:26.363678'),('98525cbc-ef06-46c1-a847-48e202b1d0fc','5c7b6dae-a17d-448a-a31e-a57039e1e5d5','2027-01-19',11,4,'NORMAL','6db6f765-0251-434c-b5a2-13f6b9af8dde',NULL,'2026-11-06 14:37:26.507855'),('99c23cd1-9a07-4a83-a486-818c6bccd0bb','c1ec3be8-d7b3-438b-9eb3-f982e5018891','2026-11-04',6,3,'NORMAL','9c599cba-54c3-48f8-82f8-e0b43c9126b4',NULL,'2026-11-06 14:37:26.363590'),('9be4fad1-afd4-4f93-940b-9a8c2b1d925e','5c7b6dae-a17d-448a-a31e-a57039e1e5d5','2026-12-29',8,3,'NORMAL','6db6f765-0251-434c-b5a2-13f6b9af8dde',NULL,'2026-11-06 14:37:26.507808'),('a184407a-b984-4b69-ac8b-f0b73ddf96eb','c1ec3be8-d7b3-438b-9eb3-f982e5018891','2026-11-23',9,1,'NORMAL','9c599cba-54c3-48f8-82f8-e0b43c9126b4',NULL,'2026-11-06 14:37:26.363630'),('a28bed0c-3e0d-4015-b26b-443cf2ca4784','5c7b6dae-a17d-448a-a31e-a57039e1e5d5','2026-12-14',5,6,'NORMAL','6db6f765-0251-434c-b5a2-13f6b9af8dde',NULL,'2026-11-06 14:37:26.507771'),('a32580ed-8104-41ec-b005-7bf817760710','5c7b6dae-a17d-448a-a31e-a57039e1e5d5','2027-01-11',10,3,'NORMAL','6db6f765-0251-434c-b5a2-13f6b9af8dde',NULL,'2026-11-06 14:37:26.507839'),('a42b6193-0c59-4551-ab87-c373044bb454','5c7b6dae-a17d-448a-a31e-a57039e1e5d5','2026-12-31',8,6,'NORMAL','6db6f765-0251-434c-b5a2-13f6b9af8dde',NULL,'2026-11-06 14:37:26.507816'),('a5226f94-ef43-478f-8c85-ea511e1863f5','5c7b6dae-a17d-448a-a31e-a57039e1e5d5','2027-01-22',12,3,'NORMAL','6db6f765-0251-434c-b5a2-13f6b9af8dde',NULL,'2026-11-06 14:37:26.507876'),('a57b0138-243e-48cc-9e93-6bad1cda7a62','c1ec3be8-d7b3-438b-9eb3-f982e5018891','2026-10-14',2,7,'TEST','9c599cba-54c3-48f8-82f8-e0b43c9126b4',NULL,'2026-11-06 14:37:26.363515'),('a639bed1-7e51-4470-83c5-b6ebae1c1e0f','5c7b6dae-a17d-448a-a31e-a57039e1e5d5','2026-12-10',5,3,'NORMAL','6db6f765-0251-434c-b5a2-13f6b9af8dde',NULL,'2026-11-06 14:37:26.507766'),('a8ca6f64-8590-4a1f-8c9a-ec233ee12f36','c1ec3be8-d7b3-438b-9eb3-f982e5018891','2026-11-17',8,3,'NORMAL','9c599cba-54c3-48f8-82f8-e0b43c9126b4',NULL,'2026-11-06 14:37:26.363617'),('ade6c889-ddd2-4fdf-a123-6952f1cf06e2','c1ec3be8-d7b3-438b-9eb3-f982e5018891','2026-12-09',11,6,'NORMAL','9c599cba-54c3-48f8-82f8-e0b43c9126b4',NULL,'2026-11-06 14:37:26.363666'),('aed8dfea-9e66-44c2-9285-4a6407af660f','c1ec3be8-d7b3-438b-9eb3-f982e5018891','2026-12-08',11,4,'NORMAL','9c599cba-54c3-48f8-82f8-e0b43c9126b4',NULL,'2026-11-06 14:37:26.363663'),('af6919e7-8b4c-479a-a44c-077b36d4e7bc','c1ec3be8-d7b3-438b-9eb3-f982e5018891','2026-11-05',6,4,'NORMAL','9c599cba-54c3-48f8-82f8-e0b43c9126b4',NULL,'2026-11-06 14:37:26.363593'),('af7331c7-4404-440f-b85d-2353ae58d547','c1ec3be8-d7b3-438b-9eb3-f982e5018891','2026-11-19',8,6,'NORMAL','9c599cba-54c3-48f8-82f8-e0b43c9126b4',NULL,'2026-11-06 14:37:26.363623'),('b4d5b8d2-7b32-4546-8a67-71ca36b7841a','c1ec3be8-d7b3-438b-9eb3-f982e5018891','2026-10-26',4,6,'NORMAL','9c599cba-54c3-48f8-82f8-e0b43c9126b4',NULL,'2026-11-06 14:37:26.363541'),('b50ca6e3-bce8-4a8d-a06c-3a51df6a73ad','5c7b6dae-a17d-448a-a31e-a57039e1e5d5','2026-11-18',1,6,'NORMAL','6db6f765-0251-434c-b5a2-13f6b9af8dde',NULL,'2026-11-06 14:37:26.507689'),('b6719cc6-b201-4cbf-8bdf-3233a9e1721a','5c7b6dae-a17d-448a-a31e-a57039e1e5d5','2026-11-19',2,1,'NORMAL','6db6f765-0251-434c-b5a2-13f6b9af8dde',NULL,'2026-11-06 14:37:26.507692'),('b8568717-f78f-40d1-985c-dcd963a4b636','c1ec3be8-d7b3-438b-9eb3-f982e5018891','2026-10-20',3,6,'NORMAL','9c599cba-54c3-48f8-82f8-e0b43c9126b4',NULL,'2026-11-06 14:37:26.363527'),('b9857401-9573-4153-a81c-cb6e04e2cd4e','5c7b6dae-a17d-448a-a31e-a57039e1e5d5','2026-12-24',7,4,'NORMAL','6db6f765-0251-434c-b5a2-13f6b9af8dde',NULL,'2026-11-06 14:37:26.507795'),('ba957152-32e2-469d-b071-3292207507ba','c1ec3be8-d7b3-438b-9eb3-f982e5018891','2026-10-16',3,3,'NORMAL','9c599cba-54c3-48f8-82f8-e0b43c9126b4',NULL,'2026-11-06 14:37:26.363521'),('bcbcb719-ad8e-4a64-a66e-2fbbe6e80f2a','c1ec3be8-d7b3-438b-9eb3-f982e5018891','2026-12-03',10,7,'TEST','9c599cba-54c3-48f8-82f8-e0b43c9126b4',NULL,'2026-11-06 14:37:26.363654'),('bd095ff6-1bca-4976-9084-3b1d88781306','c1ec3be8-d7b3-438b-9eb3-f982e5018891','2026-11-11',7,3,'NORMAL','9c599cba-54c3-48f8-82f8-e0b43c9126b4',NULL,'2026-11-06 14:37:26.363605'),('bd3d5c27-2524-4667-830d-55d61f53e500','5c7b6dae-a17d-448a-a31e-a57039e1e5d5','2026-12-01',3,6,'NORMAL','6db6f765-0251-434c-b5a2-13f6b9af8dde',NULL,'2026-11-06 14:37:26.507721'),('bfbd565b-3d19-4e3d-a933-553f366a5a19','5c7b6dae-a17d-448a-a31e-a57039e1e5d5','2026-11-25',2,7,'TEST','6db6f765-0251-434c-b5a2-13f6b9af8dde',NULL,'2026-11-06 14:37:26.507706'),('c1d44748-7ae1-4822-8934-89477fee6d24','5c7b6dae-a17d-448a-a31e-a57039e1e5d5','2026-12-11',5,4,'NORMAL','6db6f765-0251-434c-b5a2-13f6b9af8dde',NULL,'2026-11-06 14:37:26.507768'),('c241965f-dd0e-499f-9c86-20699b47baf6','5c7b6dae-a17d-448a-a31e-a57039e1e5d5','2026-12-18',6,6,'NORMAL','6db6f765-0251-434c-b5a2-13f6b9af8dde',NULL,'2026-11-06 14:37:26.507783'),('c41d66c6-1c52-487b-a66f-8095ea2acafc','c1ec3be8-d7b3-438b-9eb3-f982e5018891','2026-12-16',12,7,'FINAL','9c599cba-54c3-48f8-82f8-e0b43c9126b4',NULL,'2026-11-06 14:37:26.363681'),('c6e2a726-d23f-422b-be29-a8f6e1abbe35','c1ec3be8-d7b3-438b-9eb3-f982e5018891','2026-10-07',1,6,'NORMAL','9c599cba-54c3-48f8-82f8-e0b43c9126b4',NULL,'2026-11-06 14:37:26.363499'),('c8ff9c6f-2f85-4ea1-89ba-0e17f700e2ea','5c7b6dae-a17d-448a-a31e-a57039e1e5d5','2026-11-27',3,3,'NORMAL','6db6f765-0251-434c-b5a2-13f6b9af8dde',NULL,'2026-11-06 14:37:26.507714'),('cacbbe58-f8a5-423c-9e0f-8c5030ea3b0c','5c7b6dae-a17d-448a-a31e-a57039e1e5d5','2027-01-20',11,6,'NORMAL','6db6f765-0251-434c-b5a2-13f6b9af8dde',NULL,'2026-11-06 14:37:26.507858'),('ce2cdf85-482a-4885-80e2-64a7237b23f1','c1ec3be8-d7b3-438b-9eb3-f982e5018891','2026-10-22',4,3,'NORMAL','9c599cba-54c3-48f8-82f8-e0b43c9126b4',NULL,'2026-11-06 14:37:26.363533'),('d097efa5-67a7-40f7-8c8f-bca9b2946399','c1ec3be8-d7b3-438b-9eb3-f982e5018891','2026-12-01',10,4,'NORMAL','9c599cba-54c3-48f8-82f8-e0b43c9126b4',NULL,'2026-11-06 14:37:26.363648'),('d3faab33-860e-43d5-8704-e231444386b6','5c7b6dae-a17d-448a-a31e-a57039e1e5d5','2026-11-20',2,3,'NORMAL','6db6f765-0251-434c-b5a2-13f6b9af8dde',NULL,'2026-11-06 14:37:26.507696'),('d4067a7b-40e2-4e1d-945d-ea6314039c0f','5c7b6dae-a17d-448a-a31e-a57039e1e5d5','2027-01-05',9,3,'NORMAL','6db6f765-0251-434c-b5a2-13f6b9af8dde',NULL,'2026-11-06 14:37:26.507826'),('d43b8eec-37dd-40ca-8416-ba0c201e85bf','c1ec3be8-d7b3-438b-9eb3-f982e5018891','2026-11-26',9,6,'NORMAL','9c599cba-54c3-48f8-82f8-e0b43c9126b4',NULL,'2026-11-06 14:37:26.363639'),('d50302c2-48f9-4893-afe7-9814a07798e6','5c7b6dae-a17d-448a-a31e-a57039e1e5d5','2027-01-12',10,4,'NORMAL','6db6f765-0251-434c-b5a2-13f6b9af8dde',NULL,'2026-11-06 14:37:26.507841'),('d5deaa83-0adf-4324-b7a2-a8d40c6e129d','5c7b6dae-a17d-448a-a31e-a57039e1e5d5','2026-12-15',6,1,'NORMAL','6db6f765-0251-434c-b5a2-13f6b9af8dde',NULL,'2026-11-06 14:37:26.507774'),('d7751694-61d1-4078-b114-b9c4afc5f482','5c7b6dae-a17d-448a-a31e-a57039e1e5d5','2027-01-07',9,6,'NORMAL','6db6f765-0251-434c-b5a2-13f6b9af8dde',NULL,'2026-11-06 14:37:26.507833'),('df95271d-1356-44e1-8e78-38bfdb090714','5c7b6dae-a17d-448a-a31e-a57039e1e5d5','2027-01-08',10,1,'NORMAL','6db6f765-0251-434c-b5a2-13f6b9af8dde',NULL,'2026-11-06 14:37:26.507836'),('e0f41d24-5207-425a-a23f-6a60471d5cb6','5c7b6dae-a17d-448a-a31e-a57039e1e5d5','2026-12-25',7,6,'NORMAL','6db6f765-0251-434c-b5a2-13f6b9af8dde',NULL,'2026-11-06 14:37:26.507798'),('e43e72b7-b5d7-4a0e-a4b4-ca37765be088','c1ec3be8-d7b3-438b-9eb3-f982e5018891','2026-12-11',12,3,'NORMAL','9c599cba-54c3-48f8-82f8-e0b43c9126b4',NULL,'2026-11-06 14:37:26.363672'),('e4a3695f-f14f-46bb-9d7b-3ad918f923fb','5c7b6dae-a17d-448a-a31e-a57039e1e5d5','2027-01-04',9,1,'NORMAL','6db6f765-0251-434c-b5a2-13f6b9af8dde',NULL,'2026-11-06 14:37:26.507822'),('e5811b6f-b036-4584-a9d5-73724038a3a8','5c7b6dae-a17d-448a-a31e-a57039e1e5d5','2026-12-07',4,6,'NORMAL','6db6f765-0251-434c-b5a2-13f6b9af8dde',NULL,'2026-11-06 14:37:26.507737'),('e990a89d-d620-48d9-8d15-d216ddd88d9f','5c7b6dae-a17d-448a-a31e-a57039e1e5d5','2026-12-09',5,1,'NORMAL','6db6f765-0251-434c-b5a2-13f6b9af8dde',NULL,'2026-11-06 14:37:26.507763'),('e9bcc72a-e633-4419-9d61-bc0888d4feaa','c1ec3be8-d7b3-438b-9eb3-f982e5018891','2026-11-02',5,6,'NORMAL','9c599cba-54c3-48f8-82f8-e0b43c9126b4',NULL,'2026-11-06 14:37:26.363584'),('f048a51a-a16b-4705-bcc8-2e6e31ee1888','c1ec3be8-d7b3-438b-9eb3-f982e5018891','2026-11-09',6,7,'MIDTERM','9c599cba-54c3-48f8-82f8-e0b43c9126b4',NULL,'2026-11-06 14:37:26.363598'),('f68b57be-3e25-4ba3-bb6e-cdc7370f9294','5c7b6dae-a17d-448a-a31e-a57039e1e5d5','2026-12-04',4,4,'NORMAL','6db6f765-0251-434c-b5a2-13f6b9af8dde',NULL,'2026-11-06 14:37:26.507733'),('f6b0827c-88f3-4e37-ae2e-b54dbf78c3a6','5c7b6dae-a17d-448a-a31e-a57039e1e5d5','2026-11-30',3,4,'NORMAL','6db6f765-0251-434c-b5a2-13f6b9af8dde',NULL,'2026-11-06 14:37:26.507718');
/*!40000 ALTER TABLE `sessions` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `students`
--

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

--
-- Dumping data for table `students`
--

LOCK TABLES `students` WRITE;
/*!40000 ALTER TABLE `students` DISABLE KEYS */;
INSERT INTO `students` VALUES ('466477d9-2724-4a4e-909e-f312720ea4e7','Đặng Tuệ Minh','2022-07-16','MALE','0901000007',NULL,'Đặng Văn Long','0901000007','Hà Nội','2026-11-06 14:37:24.917304'),('51bd7a04-f28f-4813-8312-88c20180a97f','Phạm An Nhiên','2022-04-13','FEMALE','0901000004',NULL,'Phạm Thu Trang','0901000004','Hà Nội','2026-11-06 14:37:24.917304'),('787be491-3d9a-4475-960c-9d3fbbc99a32','Vũ Khánh Linh','2022-06-15','FEMALE','0901000006',NULL,'Vũ Thị Hạnh','0901000006','Hà Nội','2026-11-06 14:37:24.917304'),('8aa6c2cd-e347-4afb-8102-d90a4be147f7','Bùi Thảo Vy','2023-08-17','FEMALE','0901000008',NULL,'Bùi Thị Lan','0901000008','Hà Nội','2026-11-06 14:37:24.917304'),('8dd66899-c8dc-4e92-a8d3-86b0e3dc54b7','Trần Bảo Ngọc','2022-02-11','FEMALE','0901000002',NULL,'Trần Thị Mai','0901000002','Hà Nội','2026-11-06 14:37:24.917304'),('9926b55b-dc45-46b8-8061-c9795625a11c','Hoàng Đức Anh','2021-05-14','MALE','0901000005',NULL,'Hoàng Văn Nam','0901000005','Hà Nội','2026-11-06 14:37:24.917304'),('dbdd2d24-bc46-46a8-815d-f0dfae35c2a2','Lê Minh Khang','2021-03-12','MALE','0901000003',NULL,'Lê Văn Phúc','0901000003','Hà Nội','2026-11-06 14:37:24.917304'),('f277ae6e-4895-43db-98bf-ae0c1493586c','Nguyễn Gia Huy','2021-01-10','MALE','0901000001',NULL,'Nguyễn Văn Hùng','0901000001','Hà Nội','2026-11-06 14:37:24.917304');
/*!40000 ALTER TABLE `students` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `system_audit_logs`
--

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

--
-- Dumping data for table `system_audit_logs`
--

LOCK TABLES `system_audit_logs` WRITE;
/*!40000 ALTER TABLE `system_audit_logs` DISABLE KEYS */;
INSERT INTO `system_audit_logs` VALUES ('65a7366e-f4d4-4c0b-b93b-2b99a282ddaa',NULL,'ASSIGN_ROLE','auth_users','9c599cba-54c3-48f8-82f8-e0b43c9126b4','{\"role\": \"TEACHER\"}','2026-11-06 14:37:25.918647'),('679f6253-3b7a-473b-83b8-a4146e32f76b',NULL,'ASSIGN_ROLE','auth_users','bb4d9fa1-a193-4575-9710-c93e8a51b62f','{\"role\": \"ROOT\"}','2026-11-06 14:37:24.739876'),('73cbc1bc-4aaa-45d8-b10b-23c11e2601da',NULL,'ASSIGN_ROLE','auth_users','6a76758d-560f-4057-a231-3b9ed014a0dc','{\"role\": \"SALES\"}','2026-11-06 14:37:25.520417'),('808cfa35-ea10-4684-abdc-4e6ecc38eb96',NULL,'ASSIGN_ROLE','auth_users','b0d991f9-b4ce-4fb7-a857-4b42c327a47e','{\"role\": \"DIRECTOR\"}','2026-11-06 14:37:25.122127'),('af508fb5-b817-41c5-a91f-0d63770eaec1',NULL,'ASSIGN_ROLE','auth_users','92f96ce5-3644-42e0-94c0-699a5d6e4d00','{\"role\": \"ACCOUNTANT\"}','2026-11-06 14:37:25.724333'),('c56331d7-4dfb-409a-ae8d-3cd36b25c8a5',NULL,'TRIAL_SCHEDULE','trial_leads','dd09f34c-3f4e-4337-b9b8-cd935551629d','{\"note\": \"Phụ huynh đăng ký cho bé học thử\", \"classId\": \"5c7b6dae-a17d-448a-a31e-a57039e1e5d5\", \"sessionId\": \"0707c9bd-096f-478b-b3be-e6e4495047ce\", \"sessionDate\": \"2026-11-13\"}','2026-11-06 14:37:26.899187'),('ecefc766-b9b5-426b-bcfe-4facb8c027c4',NULL,'ASSIGN_ROLE','auth_users','63f7f44e-0cd0-4b4a-aed2-19c77eca8ad9','{\"role\": \"ACADEMIC\"}','2026-11-06 14:37:25.310809'),('fc993f89-d511-4bf0-be51-23bde1cda877',NULL,'ASSIGN_ROLE','auth_users','6db6f765-0251-434c-b5a2-13f6b9af8dde','{\"role\": \"TEACHER\"}','2026-11-06 14:37:26.140923');
/*!40000 ALTER TABLE `system_audit_logs` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `system_notifications`
--

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

--
-- Dumping data for table `system_notifications`
--

LOCK TABLES `system_notifications` WRITE;
/*!40000 ALTER TABLE `system_notifications` DISABLE KEYS */;
INSERT INTO `system_notifications` VALUES ('05147251-bbe2-4ace-907f-31ec5a49552f','92f96ce5-3644-42e0-94c0-699a5d6e4d00','Hóa đơn quá hạn','Có 1 hóa đơn học phí lớp TLH-CHOI1-2026A đã quá hạn thanh toán.',0,'2026-11-06 14:37:24.917304'),('5ada4381-f7ab-4fd6-bf22-e5a8597f5352','6db6f765-0251-434c-b5a2-13f6b9af8dde','Phân công chủ nhiệm lớp','Bạn được phân công làm giáo viên chủ nhiệm lớp TLH-MAM1-2026A.',0,'2026-11-06 14:37:24.917304'),('9ca458ff-4daf-4422-b312-5a78852dbcf5','63f7f44e-0cd0-4b4a-aed2-19c77eca8ad9','Lớp mới sắp khai giảng','Lớp TLH-MAM1-2026A khai giảng ngày 13/11/2026.',0,'2026-11-06 14:37:24.917304');
/*!40000 ALTER TABLE `system_notifications` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `trial_leads`
--

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

--
-- Dumping data for table `trial_leads`
--

LOCK TABLES `trial_leads` WRITE;
/*!40000 ALTER TABLE `trial_leads` DISABLE KEYS */;
INSERT INTO `trial_leads` VALUES ('1b9e5122-51aa-400a-b5b8-0ad99889e15f','Ngô Bảo Châu','0912000001',NULL,'Facebook','NEW',NULL,'6a76758d-560f-4057-a231-3b9ed014a0dc','2026-11-06 14:37:24.917304'),('3520ef45-895a-44a1-9895-ffa4cb7b4528','Trịnh Minh Thư','0912000002','parent.thu@example.com','Website','CONTACTED','Phụ huynh muốn cho bé học thử buổi sáng','6a76758d-560f-4057-a231-3b9ed014a0dc','2026-11-06 14:37:24.917304'),('dd09f34c-3f4e-4337-b9b8-cd935551629d','Mai Gia Bảo','0912000003',NULL,'Giới thiệu','SCHEDULED',NULL,'6a76758d-560f-4057-a231-3b9ed014a0dc','2026-11-06 14:37:24.917304');
/*!40000 ALTER TABLE `trial_leads` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-11-06 14:37:53
