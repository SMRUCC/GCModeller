CREATE DATABASE  IF NOT EXISTS `kb_go` /*!40100 DEFAULT CHARACTER SET utf8 */;
USE `kb_go`;
-- MySQL dump 10.13  Distrib 5.7.17, for Win64 (x86_64)
--
-- Host: localhost    Database: kb_go
-- ------------------------------------------------------
-- Server version	5.7.18-log

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `alt_id`
--

DROP TABLE IF EXISTS `alt_id`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `alt_id` (
  `id` int(10) unsigned NOT NULL,
  `alt_id` int(10) unsigned NOT NULL,
  `name` mediumtext,
  PRIMARY KEY (`id`,`alt_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `dag_relationship`
--

DROP TABLE IF EXISTS `dag_relationship`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `dag_relationship` (
  `id` int(10) unsigned NOT NULL,
  `relationship` varchar(45) DEFAULT NULL,
  `relationship_id` int(10) unsigned NOT NULL,
  `term_id` int(10) unsigned NOT NULL,
  `name` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`id`,`term_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `go_terms`
--

DROP TABLE IF EXISTS `go_terms`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `go_terms` (
  `id` int(10) unsigned NOT NULL COMMENT '其实就是将term编号之中的``GO:``前缀给删除了而得到的一个数字',
  `term` char(16) NOT NULL COMMENT 'GO id',
  `name` varchar(45) DEFAULT NULL,
  `namespace_id` int(10) unsigned NOT NULL,
  `namespace` varchar(45) NOT NULL,
  `def` longtext NOT NULL,
  `is_obsolete` tinyint(4) NOT NULL DEFAULT '0' COMMENT '0 为 False, 1 为 True',
  `comment` longtext,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id_UNIQUE` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `relation_names`
--

DROP TABLE IF EXISTS `relation_names`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `relation_names` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `name` mediumtext NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id_UNIQUE` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `term_namespace`
--

DROP TABLE IF EXISTS `term_namespace`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `term_namespace` (
  `id` int(10) unsigned zerofill NOT NULL,
  `namespace` tinytext NOT NULL COMMENT '这个表里面只有三个值',
  PRIMARY KEY (`id`),
  UNIQUE KEY `id_UNIQUE` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `term_synonym`
--

DROP TABLE IF EXISTS `term_synonym`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `term_synonym` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `term_id` int(10) unsigned NOT NULL,
  `synonym` mediumtext NOT NULL,
  `type` varchar(45) DEFAULT NULL COMMENT 'EXACT []  表示完全一样\nRELATED [EC:3.1.27.3] 表示和xxxx有关联，其中EC编号为本表之中的object字段 ',
  `object` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id_UNIQUE` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `xref`
--

DROP TABLE IF EXISTS `xref`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `xref` (
  `go_id` int(10) unsigned NOT NULL,
  `xref` varchar(45) NOT NULL COMMENT '外部数据库名称',
  `external_id` varchar(45) NOT NULL COMMENT '外部数据库编号',
  `comment` longtext,
  PRIMARY KEY (`go_id`,`external_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping events for database 'kb_go'
--

--
-- Dumping routines for database 'kb_go'
--
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2017-08-31 23:10:11
