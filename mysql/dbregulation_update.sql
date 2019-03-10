CREATE DATABASE  IF NOT EXISTS `dbregulation_update` /*!40100 DEFAULT CHARACTER SET utf8 */;
USE `dbregulation_update`;
-- MySQL dump 10.13  Distrib 5.7.9, for Win64 (x86_64)
--
-- Host: localhost    Database: dbregulation_update
-- ------------------------------------------------------
-- Server version	5.7.12-log

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
-- Table structure for table `articles`
--

DROP TABLE IF EXISTS `articles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `articles` (
  `art_guid` int(11) NOT NULL DEFAULT '0',
  `pkg_guid` int(11) NOT NULL DEFAULT '0',
  `title` varchar(255) DEFAULT NULL,
  `author` varchar(255) DEFAULT NULL,
  `pmid` varchar(20) DEFAULT NULL,
  `art_journal` varchar(50) DEFAULT NULL,
  `art_year` varchar(10) DEFAULT NULL,
  `art_month` varchar(10) DEFAULT NULL,
  `art_volume` varchar(10) DEFAULT NULL,
  `art_issue` varchar(10) DEFAULT NULL,
  `art_pages` varchar(20) DEFAULT NULL,
  `art_abstruct` blob,
  `exp_num` int(11) DEFAULT NULL,
  `art_state` int(11) DEFAULT '0',
  `last_update` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`art_guid`),
  UNIQUE KEY `pmid_unique` (`pmid`),
  KEY `FK_articles-pkg_guid` (`pkg_guid`),
  CONSTRAINT `FK_articles-pkg_guid` FOREIGN KEY (`pkg_guid`) REFERENCES `packages` (`pkg_guid`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `db_user_roles`
--

DROP TABLE IF EXISTS `db_user_roles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `db_user_roles` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `role_name` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `db_users`
--

DROP TABLE IF EXISTS `db_users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `db_users` (
  `id` int(11) NOT NULL DEFAULT '0',
  `user_role_id` int(11) DEFAULT NULL,
  `name` varchar(20) DEFAULT NULL,
  `full_name` varchar(100) DEFAULT NULL,
  `phone` varchar(100) DEFAULT '',
  `email` varchar(100) DEFAULT '',
  `fl_active` int(1) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `dict_effectors`
--

DROP TABLE IF EXISTS `dict_effectors`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `dict_effectors` (
  `effector_guid` int(11) NOT NULL DEFAULT '0',
  `name` text,
  `description` mediumtext NOT NULL,
  `effector_superclass_guid` int(11) DEFAULT NULL,
  PRIMARY KEY (`effector_guid`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1 ROW_FORMAT=DYNAMIC;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `dict_effectors_superclasses`
--

DROP TABLE IF EXISTS `dict_effectors_superclasses`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `dict_effectors_superclasses` (
  `effector_superclass_guid` int(11) NOT NULL DEFAULT '0',
  `name` varchar(100) NOT NULL DEFAULT '',
  `parent_guid` int(11) DEFAULT NULL,
  `idx` int(11) NOT NULL DEFAULT '0',
  `left_idx` int(11) NOT NULL DEFAULT '0',
  `right_idx` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`effector_superclass_guid`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `dict_exp_result_types`
--

DROP TABLE IF EXISTS `dict_exp_result_types`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `dict_exp_result_types` (
  `exp_result_type_guid` int(11) NOT NULL DEFAULT '0',
  `name` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`exp_result_type_guid`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `dict_exp_technique_types`
--

DROP TABLE IF EXISTS `dict_exp_technique_types`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `dict_exp_technique_types` (
  `exp_technique_type_guid` int(11) NOT NULL DEFAULT '0',
  `name` varchar(100) DEFAULT NULL,
  `exp_technique_type_superclass_guid` int(10) unsigned DEFAULT NULL,
  PRIMARY KEY (`exp_technique_type_guid`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `dict_exp_technique_types_superclasses`
--

DROP TABLE IF EXISTS `dict_exp_technique_types_superclasses`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `dict_exp_technique_types_superclasses` (
  `exp_technique_type_superclass_guid` int(10) unsigned NOT NULL DEFAULT '0',
  `name` varchar(255) NOT NULL DEFAULT '',
  PRIMARY KEY (`exp_technique_type_superclass_guid`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `dict_func_site_types`
--

DROP TABLE IF EXISTS `dict_func_site_types`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `dict_func_site_types` (
  `func_site_type_guid` int(11) NOT NULL DEFAULT '0',
  `name` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`func_site_type_guid`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `dict_genomes`
--

DROP TABLE IF EXISTS `dict_genomes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `dict_genomes` (
  `genome_guid` int(11) NOT NULL DEFAULT '0',
  `name` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`genome_guid`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `dict_obj_side_types`
--

DROP TABLE IF EXISTS `dict_obj_side_types`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `dict_obj_side_types` (
  `obj_side_type_guid` int(11) NOT NULL DEFAULT '0',
  `name` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`obj_side_type_guid`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `dict_regulator_types`
--

DROP TABLE IF EXISTS `dict_regulator_types`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `dict_regulator_types` (
  `regulator_type_guid` int(11) NOT NULL DEFAULT '0',
  `name` varchar(100) NOT NULL DEFAULT '',
  PRIMARY KEY (`regulator_type_guid`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `dict_struct_site_types`
--

DROP TABLE IF EXISTS `dict_struct_site_types`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `dict_struct_site_types` (
  `struct_site_type_guid` int(11) NOT NULL DEFAULT '0',
  `name` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`struct_site_type_guid`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `exp2effectors`
--

DROP TABLE IF EXISTS `exp2effectors`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `exp2effectors` (
  `pkg_guid` int(11) NOT NULL DEFAULT '0',
  `art_guid` int(11) NOT NULL DEFAULT '0',
  `exp_guid` int(11) NOT NULL DEFAULT '0',
  `effector_guid` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`exp_guid`,`effector_guid`),
  KEY `FK_exp2effectors-pkg_guid` (`pkg_guid`),
  KEY `FK_exp2effectors-art_guid` (`art_guid`),
  KEY `FK_exp2effectors-effector_guid` (`effector_guid`),
  CONSTRAINT `FK_exp2effectors-art_guid` FOREIGN KEY (`art_guid`) REFERENCES `articles` (`art_guid`),
  CONSTRAINT `FK_exp2effectors-effector_guid` FOREIGN KEY (`effector_guid`) REFERENCES `dict_effectors` (`effector_guid`),
  CONSTRAINT `FK_exp2effectors-exp_guid` FOREIGN KEY (`exp_guid`) REFERENCES `experiments` (`exp_guid`),
  CONSTRAINT `FK_exp2effectors-pkg_guid` FOREIGN KEY (`pkg_guid`) REFERENCES `packages` (`pkg_guid`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `exp2result_types`
--

DROP TABLE IF EXISTS `exp2result_types`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `exp2result_types` (
  `pkg_guid` int(11) NOT NULL DEFAULT '0',
  `art_guid` int(11) NOT NULL DEFAULT '0',
  `exp_guid` int(11) NOT NULL DEFAULT '0',
  `exp_result_type_guid` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`exp_guid`,`exp_result_type_guid`),
  KEY `FK_exp2result_types-pkg_guid` (`pkg_guid`),
  KEY `FK_exp2result_types-art_guid` (`art_guid`),
  KEY `FK_exp2result_types-exp_result_type_guid` (`exp_result_type_guid`),
  CONSTRAINT `FK_exp2result_types-art_guid` FOREIGN KEY (`art_guid`) REFERENCES `articles` (`art_guid`),
  CONSTRAINT `FK_exp2result_types-exp_guid` FOREIGN KEY (`exp_guid`) REFERENCES `experiments` (`exp_guid`),
  CONSTRAINT `FK_exp2result_types-exp_result_type_guid` FOREIGN KEY (`exp_result_type_guid`) REFERENCES `dict_exp_result_types` (`exp_result_type_guid`),
  CONSTRAINT `FK_exp2result_types-pkg_guid` FOREIGN KEY (`pkg_guid`) REFERENCES `packages` (`pkg_guid`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `exp2technique_types`
--

DROP TABLE IF EXISTS `exp2technique_types`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `exp2technique_types` (
  `pkg_guid` int(11) NOT NULL DEFAULT '0',
  `art_guid` int(11) NOT NULL DEFAULT '0',
  `exp_guid` int(11) NOT NULL DEFAULT '0',
  `exp_technique_type_guid` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`exp_guid`,`exp_technique_type_guid`),
  KEY `FK_exp2technique_types-pkg_guid` (`pkg_guid`),
  KEY `FK_exp2technique_types-art_guid` (`art_guid`),
  KEY `FK_exp2technique_types-exp_technique_type_guid` (`exp_technique_type_guid`),
  CONSTRAINT `FK_exp2technique_types-art_guid` FOREIGN KEY (`art_guid`) REFERENCES `articles` (`art_guid`),
  CONSTRAINT `FK_exp2technique_types-exp_guid` FOREIGN KEY (`exp_guid`) REFERENCES `experiments` (`exp_guid`),
  CONSTRAINT `FK_exp2technique_types-exp_technique_type_guid` FOREIGN KEY (`exp_technique_type_guid`) REFERENCES `dict_exp_technique_types` (`exp_technique_type_guid`),
  CONSTRAINT `FK_exp2technique_types-pkg_guid` FOREIGN KEY (`pkg_guid`) REFERENCES `packages` (`pkg_guid`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `exp_sub_objects`
--

DROP TABLE IF EXISTS `exp_sub_objects`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `exp_sub_objects` (
  `pkg_guid` int(11) NOT NULL DEFAULT '0',
  `art_guid` int(11) NOT NULL DEFAULT '0',
  `exp_guid` int(11) NOT NULL DEFAULT '0',
  `obj_guid` int(11) NOT NULL DEFAULT '0',
  `obj_type_id` int(11) DEFAULT NULL,
  `order_num` int(11) DEFAULT NULL,
  `strand` int(1) DEFAULT NULL,
  PRIMARY KEY (`exp_guid`,`obj_guid`),
  KEY `FK_exp_sub_objects-pkg_guid` (`pkg_guid`),
  KEY `FK_exp_sub_objects-art_guid` (`art_guid`),
  KEY `obj_guid` (`obj_guid`),
  CONSTRAINT `FK_exp_sub_objects-art_guid` FOREIGN KEY (`art_guid`) REFERENCES `articles` (`art_guid`),
  CONSTRAINT `FK_exp_sub_objects-exp_guid` FOREIGN KEY (`exp_guid`) REFERENCES `experiments` (`exp_guid`),
  CONSTRAINT `FK_exp_sub_objects-pkg_guid` FOREIGN KEY (`pkg_guid`) REFERENCES `packages` (`pkg_guid`),
  CONSTRAINT `exp_sub_objects_ibfk_1` FOREIGN KEY (`obj_guid`) REFERENCES `obj_name_genomes` (`obj_guid`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `experiments`
--

DROP TABLE IF EXISTS `experiments`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `experiments` (
  `exp_guid` int(11) NOT NULL DEFAULT '0',
  `pkg_guid` int(11) NOT NULL DEFAULT '0',
  `art_guid` int(11) NOT NULL DEFAULT '0',
  `descript` blob,
  `last_update` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`exp_guid`),
  KEY `FK_experiments-pkg_guid` (`pkg_guid`),
  KEY `FK_experiments-art_guid` (`art_guid`),
  CONSTRAINT `FK_experiments-art_guid` FOREIGN KEY (`art_guid`) REFERENCES `articles` (`art_guid`),
  CONSTRAINT `FK_experiments-pkg_guid` FOREIGN KEY (`pkg_guid`) REFERENCES `packages` (`pkg_guid`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `genes`
--

DROP TABLE IF EXISTS `genes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `genes` (
  `gene_guid` int(11) NOT NULL DEFAULT '0',
  `pkg_guid` int(11) NOT NULL DEFAULT '0',
  `art_guid` int(11) NOT NULL DEFAULT '0',
  `name` varchar(50) DEFAULT NULL,
  `fl_real_name` int(1) DEFAULT NULL,
  `genome_guid` int(11) DEFAULT NULL,
  `location` varchar(50) DEFAULT NULL,
  `ref_bank1` varchar(255) DEFAULT NULL,
  `ref_bank2` varchar(255) DEFAULT NULL,
  `ref_bank3` varchar(255) DEFAULT NULL,
  `ref_bank4` varchar(255) DEFAULT NULL,
  `signature` text,
  `metabol_path` varchar(100) DEFAULT NULL,
  `ferment_num` varchar(20) DEFAULT NULL,
  `gene_function` varchar(100) DEFAULT NULL,
  `descript` blob,
  `last_update` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`gene_guid`),
  KEY `FK_genes-pkg_guid` (`pkg_guid`),
  KEY `FK_genes-art_guid` (`art_guid`),
  KEY `FK_genes-genome_guid` (`genome_guid`),
  CONSTRAINT `FK_genes-art_guid` FOREIGN KEY (`art_guid`) REFERENCES `articles` (`art_guid`),
  CONSTRAINT `FK_genes-genome_guid` FOREIGN KEY (`genome_guid`) REFERENCES `dict_genomes` (`genome_guid`),
  CONSTRAINT `FK_genes-pkg_guid` FOREIGN KEY (`pkg_guid`) REFERENCES `packages` (`pkg_guid`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `genome2ncbitaxon`
--

DROP TABLE IF EXISTS `genome2ncbitaxon`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `genome2ncbitaxon` (
  `genome_guid` int(10) unsigned NOT NULL DEFAULT '0',
  `ncbi_tax_id` int(10) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`genome_guid`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `guids`
--

DROP TABLE IF EXISTS `guids`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `guids` (
  `obj_type` varchar(100) NOT NULL DEFAULT '',
  `max_guid` int(11) NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `h_dict_exp_result_types`
--

DROP TABLE IF EXISTS `h_dict_exp_result_types`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `h_dict_exp_result_types` (
  `pkg_name` varchar(100) NOT NULL DEFAULT '',
  `db_name` varchar(100) DEFAULT '0',
  PRIMARY KEY (`pkg_name`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `h_dict_exp_technique_types`
--

DROP TABLE IF EXISTS `h_dict_exp_technique_types`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `h_dict_exp_technique_types` (
  `pkg_name` varchar(100) NOT NULL DEFAULT '',
  `db_name` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`pkg_name`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `h_dict_func_site_types`
--

DROP TABLE IF EXISTS `h_dict_func_site_types`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `h_dict_func_site_types` (
  `pkg_name` varchar(100) NOT NULL DEFAULT '',
  `db_name` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`pkg_name`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `h_dict_genomes`
--

DROP TABLE IF EXISTS `h_dict_genomes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `h_dict_genomes` (
  `pkg_name` varchar(100) NOT NULL DEFAULT '',
  `db_name` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`pkg_name`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `h_dict_obj_side_types`
--

DROP TABLE IF EXISTS `h_dict_obj_side_types`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `h_dict_obj_side_types` (
  `pkg_name` varchar(100) NOT NULL DEFAULT '',
  `db_name` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`pkg_name`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `h_dict_regulator_types`
--

DROP TABLE IF EXISTS `h_dict_regulator_types`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `h_dict_regulator_types` (
  `pkg_name` varchar(100) NOT NULL DEFAULT '',
  `db_name` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`pkg_name`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `h_dict_struct_site_types`
--

DROP TABLE IF EXISTS `h_dict_struct_site_types`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `h_dict_struct_site_types` (
  `pkg_name` varchar(100) NOT NULL DEFAULT '',
  `db_name` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`pkg_name`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `helices`
--

DROP TABLE IF EXISTS `helices`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `helices` (
  `helix_guid` int(11) NOT NULL DEFAULT '0',
  `pkg_guid` int(11) NOT NULL DEFAULT '0',
  `art_guid` int(11) NOT NULL DEFAULT '0',
  `name` varchar(50) DEFAULT NULL,
  `fl_real_name` int(1) DEFAULT NULL,
  `genome_guid` int(11) DEFAULT NULL,
  `sec_struct_guid` int(11) DEFAULT NULL,
  `pos_from1` int(11) DEFAULT NULL,
  `pos_to1` int(11) DEFAULT NULL,
  `pos_from2` int(11) DEFAULT NULL,
  `pos_to2` int(11) DEFAULT NULL,
  `descript` blob,
  PRIMARY KEY (`helix_guid`),
  KEY `FK_helices-pkg_guid` (`pkg_guid`),
  KEY `FK_helices-art_guid` (`art_guid`),
  KEY `FK_helices-genome_guid` (`genome_guid`),
  KEY `FK_helices-sec_struct_guid` (`sec_struct_guid`),
  CONSTRAINT `FK_helices-art_guid` FOREIGN KEY (`art_guid`) REFERENCES `articles` (`art_guid`),
  CONSTRAINT `FK_helices-genome_guid` FOREIGN KEY (`genome_guid`) REFERENCES `dict_genomes` (`genome_guid`),
  CONSTRAINT `FK_helices-pkg_guid` FOREIGN KEY (`pkg_guid`) REFERENCES `packages` (`pkg_guid`),
  CONSTRAINT `FK_helices-sec_struct_guid` FOREIGN KEY (`sec_struct_guid`) REFERENCES `sec_structures` (`sec_struct_guid`),
  CONSTRAINT `helices_ibfk_1` FOREIGN KEY (`sec_struct_guid`) REFERENCES `obj_name_genomes` (`obj_guid`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `locuses`
--

DROP TABLE IF EXISTS `locuses`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `locuses` (
  `locus_guid` int(11) NOT NULL DEFAULT '0',
  `pkg_guid` int(11) NOT NULL DEFAULT '0',
  `art_guid` int(11) NOT NULL DEFAULT '0',
  `name` varchar(50) DEFAULT NULL,
  `fl_real_name` int(1) DEFAULT NULL,
  `genome_guid` int(11) DEFAULT NULL,
  `location` varchar(50) DEFAULT NULL,
  `descript` blob,
  `last_update` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`locus_guid`),
  KEY `FK_locuses-pkg_guid` (`pkg_guid`),
  KEY `FK_locuses-art_guid` (`art_guid`),
  KEY `FK_locuses-genome_guid` (`genome_guid`),
  CONSTRAINT `FK_locuses-art_guid` FOREIGN KEY (`art_guid`) REFERENCES `articles` (`art_guid`),
  CONSTRAINT `FK_locuses-genome_guid` FOREIGN KEY (`genome_guid`) REFERENCES `dict_genomes` (`genome_guid`),
  CONSTRAINT `FK_locuses-pkg_guid` FOREIGN KEY (`pkg_guid`) REFERENCES `packages` (`pkg_guid`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `obj_name_genomes`
--

DROP TABLE IF EXISTS `obj_name_genomes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `obj_name_genomes` (
  `obj_guid` int(11) NOT NULL DEFAULT '0',
  `pkg_guid` int(11) NOT NULL DEFAULT '0',
  `art_guid` int(11) NOT NULL DEFAULT '0',
  `name` varchar(50) DEFAULT NULL,
  `genome_guid` int(11) DEFAULT NULL,
  `obj_type_id` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`obj_guid`),
  KEY `FK_obj_name_genomes-pkg_guid` (`pkg_guid`),
  KEY `FK_obj_name_genomes-art_guid` (`art_guid`),
  KEY `FK_obj_name_genomes-genome_guid` (`genome_guid`),
  CONSTRAINT `FK_obj_name_genomes-art_guid` FOREIGN KEY (`art_guid`) REFERENCES `articles` (`art_guid`),
  CONSTRAINT `FK_obj_name_genomes-genome_guid` FOREIGN KEY (`genome_guid`) REFERENCES `dict_genomes` (`genome_guid`),
  CONSTRAINT `FK_obj_name_genomes-pkg_guid` FOREIGN KEY (`pkg_guid`) REFERENCES `packages` (`pkg_guid`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `obj_sub_types`
--

DROP TABLE IF EXISTS `obj_sub_types`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `obj_sub_types` (
  `parent_obj_type_id` int(11) NOT NULL DEFAULT '0',
  `child_obj_type_id` int(11) NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `obj_synonyms`
--

DROP TABLE IF EXISTS `obj_synonyms`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `obj_synonyms` (
  `pkg_guid` int(11) NOT NULL DEFAULT '0',
  `art_guid` int(11) NOT NULL DEFAULT '0',
  `obj_guid` int(11) NOT NULL DEFAULT '0',
  `syn_name` varchar(50) NOT NULL DEFAULT '',
  `fl_real_name` int(1) DEFAULT NULL,
  PRIMARY KEY (`obj_guid`,`syn_name`),
  KEY `pkg_guid` (`pkg_guid`),
  KEY `art_guid` (`art_guid`),
  CONSTRAINT `obj_synonyms_ibfk_1` FOREIGN KEY (`pkg_guid`) REFERENCES `packages` (`pkg_guid`),
  CONSTRAINT `obj_synonyms_ibfk_2` FOREIGN KEY (`art_guid`) REFERENCES `articles` (`art_guid`),
  CONSTRAINT `obj_synonyms_ibfk_3` FOREIGN KEY (`obj_guid`) REFERENCES `obj_name_genomes` (`obj_guid`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `obj_types`
--

DROP TABLE IF EXISTS `obj_types`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `obj_types` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `obj_type_name` varchar(50) DEFAULT NULL,
  `obj_tbname` varchar(50) DEFAULT NULL,
  `cp_order` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `operons`
--

DROP TABLE IF EXISTS `operons`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `operons` (
  `operon_guid` int(11) NOT NULL DEFAULT '0',
  `pkg_guid` int(11) NOT NULL DEFAULT '0',
  `art_guid` int(11) NOT NULL DEFAULT '0',
  `name` varchar(50) DEFAULT NULL,
  `fl_real_name` int(1) DEFAULT NULL,
  `genome_guid` int(11) DEFAULT NULL,
  `descript` blob,
  `last_update` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`operon_guid`),
  KEY `FK_operons-pkg_guid` (`pkg_guid`),
  KEY `FK_operons-art_guid` (`art_guid`),
  KEY `FK_operons-genome_guid` (`genome_guid`),
  CONSTRAINT `FK_operons-art_guid` FOREIGN KEY (`art_guid`) REFERENCES `articles` (`art_guid`),
  CONSTRAINT `FK_operons-genome_guid` FOREIGN KEY (`genome_guid`) REFERENCES `dict_genomes` (`genome_guid`),
  CONSTRAINT `FK_operons-pkg_guid` FOREIGN KEY (`pkg_guid`) REFERENCES `packages` (`pkg_guid`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `packages`
--

DROP TABLE IF EXISTS `packages`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `packages` (
  `pkg_guid` int(11) NOT NULL DEFAULT '0',
  `title` char(50) DEFAULT NULL,
  `annotator_id` int(11) DEFAULT NULL,
  `article_num` int(11) DEFAULT NULL,
  `pkg_state` int(11) NOT NULL DEFAULT '0',
  `pkg_state_date` char(10) DEFAULT NULL,
  `last_update` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`pkg_guid`),
  KEY `annotator_id` (`annotator_id`),
  CONSTRAINT `packages_ibfk_1` FOREIGN KEY (`annotator_id`) REFERENCES `db_users` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `pkg_history`
--

DROP TABLE IF EXISTS `pkg_history`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `pkg_history` (
  `pkg_guid` int(11) NOT NULL DEFAULT '0',
  `event_date` varchar(100) NOT NULL DEFAULT '',
  `event_operation` varchar(100) NOT NULL DEFAULT '',
  `user_by_id` int(11) DEFAULT '0',
  `user_by_name` varchar(100) DEFAULT '',
  `user_by_role` varchar(100) DEFAULT '',
  `user_by_email` varchar(100) DEFAULT '',
  `user_by_phone` varchar(100) DEFAULT '',
  `user_to_id` int(11) DEFAULT '0',
  `user_to_name` varchar(100) DEFAULT '',
  `user_to_role` varchar(100) DEFAULT '',
  `user_to_email` varchar(100) DEFAULT '',
  `user_to_phone` varchar(100) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `reg_elem_sub_objects`
--

DROP TABLE IF EXISTS `reg_elem_sub_objects`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `reg_elem_sub_objects` (
  `pkg_guid` int(11) NOT NULL DEFAULT '0',
  `art_guid` int(11) NOT NULL DEFAULT '0',
  `parent_guid` int(11) NOT NULL DEFAULT '0',
  `parent_type_id` int(11) DEFAULT NULL,
  `child_guid` int(11) NOT NULL DEFAULT '0',
  `child_type_id` int(11) DEFAULT NULL,
  `child_n` int(11) DEFAULT NULL,
  `strand` int(1) DEFAULT NULL,
  PRIMARY KEY (`parent_guid`,`child_guid`),
  KEY `FK_reg_elem_sub_objects-pkg_guid` (`pkg_guid`),
  KEY `FK_reg_elem_sub_objects-art_guid` (`art_guid`),
  KEY `child_guid` (`child_guid`),
  CONSTRAINT `FK_reg_elem_sub_objects-art_guid` FOREIGN KEY (`art_guid`) REFERENCES `articles` (`art_guid`),
  CONSTRAINT `FK_reg_elem_sub_objects-pkg_guid` FOREIGN KEY (`pkg_guid`) REFERENCES `packages` (`pkg_guid`),
  CONSTRAINT `reg_elem_sub_objects_ibfk_1` FOREIGN KEY (`child_guid`) REFERENCES `obj_name_genomes` (`obj_guid`),
  CONSTRAINT `reg_elem_sub_objects_ibfk_2` FOREIGN KEY (`parent_guid`) REFERENCES `obj_name_genomes` (`obj_guid`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `regulator2effectors`
--

DROP TABLE IF EXISTS `regulator2effectors`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `regulator2effectors` (
  `regulator_guid` int(10) unsigned NOT NULL DEFAULT '0',
  `effector_guid` int(10) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`regulator_guid`,`effector_guid`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `regulators`
--

DROP TABLE IF EXISTS `regulators`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `regulators` (
  `regulator_guid` int(11) NOT NULL DEFAULT '0',
  `pkg_guid` int(11) NOT NULL DEFAULT '0',
  `art_guid` int(11) NOT NULL DEFAULT '0',
  `name` varchar(50) DEFAULT NULL,
  `fl_real_name` int(1) DEFAULT NULL,
  `genome_guid` int(11) DEFAULT NULL,
  `fl_prot_rna` int(1) DEFAULT NULL,
  `regulator_type_guid` int(11) DEFAULT '0',
  `gene_guid` int(11) DEFAULT NULL,
  `ref_bank1` varchar(255) DEFAULT NULL,
  `ref_bank2` varchar(255) DEFAULT NULL,
  `ref_bank3` varchar(255) DEFAULT NULL,
  `ref_bank4` varchar(255) DEFAULT NULL,
  `consensus` varchar(50) DEFAULT NULL,
  `family` varchar(20) DEFAULT NULL,
  `descript` blob,
  `last_update` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`regulator_guid`),
  KEY `FK_regulators-pkg_guid` (`pkg_guid`),
  KEY `FK_regulators-art_guid` (`art_guid`),
  KEY `FK_regulators-genome_guid` (`genome_guid`),
  KEY `FK_regulators-regulator_type_guid` (`regulator_type_guid`),
  KEY `FK_regulators-gene_guid` (`gene_guid`),
  CONSTRAINT `FK_regulators-art_guid` FOREIGN KEY (`art_guid`) REFERENCES `articles` (`art_guid`),
  CONSTRAINT `FK_regulators-genome_guid` FOREIGN KEY (`genome_guid`) REFERENCES `dict_genomes` (`genome_guid`),
  CONSTRAINT `FK_regulators-pkg_guid` FOREIGN KEY (`pkg_guid`) REFERENCES `packages` (`pkg_guid`),
  CONSTRAINT `FK_regulators-regulator_type_guid` FOREIGN KEY (`regulator_type_guid`) REFERENCES `dict_regulator_types` (`regulator_type_guid`),
  CONSTRAINT `regulators_ibfk_1` FOREIGN KEY (`gene_guid`) REFERENCES `obj_name_genomes` (`obj_guid`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `regulons`
--

DROP TABLE IF EXISTS `regulons`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `regulons` (
  `regulon_guid` int(11) NOT NULL DEFAULT '0',
  `pkg_guid` int(11) NOT NULL DEFAULT '0',
  `art_guid` int(11) NOT NULL DEFAULT '0',
  `name` varchar(50) DEFAULT NULL,
  `fl_real_name` int(1) DEFAULT NULL,
  `genome_guid` int(11) DEFAULT NULL,
  `regulator_guid` int(11) DEFAULT NULL,
  `descript` blob,
  `last_update` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`regulon_guid`),
  KEY `FK_regulons-pkg_guid` (`pkg_guid`),
  KEY `FK_regulons-art_guid` (`art_guid`),
  KEY `FK_regulons-genome_guid` (`genome_guid`),
  KEY `FK_regulons-regulator_guid` (`regulator_guid`),
  CONSTRAINT `FK_regulons-art_guid` FOREIGN KEY (`art_guid`) REFERENCES `articles` (`art_guid`),
  CONSTRAINT `FK_regulons-genome_guid` FOREIGN KEY (`genome_guid`) REFERENCES `dict_genomes` (`genome_guid`),
  CONSTRAINT `FK_regulons-pkg_guid` FOREIGN KEY (`pkg_guid`) REFERENCES `packages` (`pkg_guid`),
  CONSTRAINT `regulons_ibfk_1` FOREIGN KEY (`regulator_guid`) REFERENCES `obj_name_genomes` (`obj_guid`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `sec_structures`
--

DROP TABLE IF EXISTS `sec_structures`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `sec_structures` (
  `sec_struct_guid` int(11) NOT NULL DEFAULT '0',
  `pkg_guid` int(11) NOT NULL DEFAULT '0',
  `art_guid` int(11) NOT NULL DEFAULT '0',
  `name` varchar(50) DEFAULT NULL,
  `fl_real_name` int(1) DEFAULT NULL,
  `genome_guid` int(11) DEFAULT NULL,
  `pos_from` int(11) DEFAULT NULL,
  `pos_from_guid` int(11) DEFAULT NULL,
  `pfo_type_id` int(11) DEFAULT NULL,
  `pfo_side_guid` int(11) DEFAULT NULL,
  `pos_to` int(11) DEFAULT NULL,
  `pos_to_guid` int(11) DEFAULT NULL,
  `pto_type_id` int(11) DEFAULT NULL,
  `pto_side_guid` int(11) DEFAULT NULL,
  `sequence` varchar(255) DEFAULT NULL,
  `descript` blob,
  PRIMARY KEY (`sec_struct_guid`),
  KEY `FK_sec_structures-pkg_guid` (`pkg_guid`),
  KEY `FK_sec_structures-art_guid` (`art_guid`),
  KEY `FK_sec_structures-genome_guid` (`genome_guid`),
  CONSTRAINT `FK_sec_structures-art_guid` FOREIGN KEY (`art_guid`) REFERENCES `articles` (`art_guid`),
  CONSTRAINT `FK_sec_structures-genome_guid` FOREIGN KEY (`genome_guid`) REFERENCES `dict_genomes` (`genome_guid`),
  CONSTRAINT `FK_sec_structures-pkg_guid` FOREIGN KEY (`pkg_guid`) REFERENCES `packages` (`pkg_guid`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `sites`
--

DROP TABLE IF EXISTS `sites`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `sites` (
  `site_guid` int(11) NOT NULL DEFAULT '0',
  `pkg_guid` int(11) NOT NULL DEFAULT '0',
  `art_guid` int(11) NOT NULL DEFAULT '0',
  `name` varchar(50) DEFAULT NULL,
  `fl_real_name` int(1) DEFAULT NULL,
  `genome_guid` int(11) DEFAULT NULL,
  `func_site_type_guid` int(11) DEFAULT NULL,
  `struct_site_type_guid` int(11) DEFAULT NULL,
  `regulator_guid` int(11) DEFAULT '0',
  `fl_dna_rna` int(1) DEFAULT NULL,
  `pos_from` int(11) DEFAULT NULL,
  `pos_from_guid` int(11) DEFAULT NULL,
  `pfo_type_id` int(11) DEFAULT NULL,
  `pfo_side_guid` int(11) DEFAULT NULL,
  `pos_to` int(11) DEFAULT NULL,
  `pos_to_guid` int(11) DEFAULT NULL,
  `pto_type_id` int(11) DEFAULT NULL,
  `pto_side_guid` int(11) DEFAULT NULL,
  `site_len` int(11) DEFAULT NULL,
  `sequence` text,
  `signature` varchar(255) DEFAULT NULL,
  `descript` blob,
  `last_update` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`site_guid`),
  KEY `FK_sites-pkg_guid` (`pkg_guid`),
  KEY `FK_sites-art_guid` (`art_guid`),
  KEY `FK_sites-genome_guid` (`genome_guid`),
  KEY `FK_sites-func_site_type_guid` (`func_site_type_guid`),
  KEY `FK_sites-struct_site_type_guid` (`struct_site_type_guid`),
  KEY `FK_sites-regulator_guid` (`regulator_guid`),
  CONSTRAINT `FK_sites-art_guid` FOREIGN KEY (`art_guid`) REFERENCES `articles` (`art_guid`),
  CONSTRAINT `FK_sites-func_site_type_guid` FOREIGN KEY (`func_site_type_guid`) REFERENCES `dict_func_site_types` (`func_site_type_guid`),
  CONSTRAINT `FK_sites-genome_guid` FOREIGN KEY (`genome_guid`) REFERENCES `dict_genomes` (`genome_guid`),
  CONSTRAINT `FK_sites-pkg_guid` FOREIGN KEY (`pkg_guid`) REFERENCES `packages` (`pkg_guid`),
  CONSTRAINT `FK_sites-struct_site_type_guid` FOREIGN KEY (`struct_site_type_guid`) REFERENCES `dict_struct_site_types` (`struct_site_type_guid`),
  CONSTRAINT `sites_ibfk_1` FOREIGN KEY (`regulator_guid`) REFERENCES `obj_name_genomes` (`obj_guid`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `transcripts`
--

DROP TABLE IF EXISTS `transcripts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `transcripts` (
  `transcript_guid` int(11) NOT NULL DEFAULT '0',
  `pkg_guid` int(11) NOT NULL DEFAULT '0',
  `art_guid` int(11) NOT NULL DEFAULT '0',
  `name` varchar(50) DEFAULT NULL,
  `fl_real_name` int(1) DEFAULT NULL,
  `genome_guid` int(11) DEFAULT NULL,
  `pos_from` int(11) DEFAULT NULL,
  `pos_from_guid` int(11) DEFAULT NULL,
  `pfo_type_id` int(11) DEFAULT NULL,
  `pfo_side_guid` int(11) DEFAULT NULL,
  `pos_to` int(11) DEFAULT NULL,
  `pos_to_guid` int(11) DEFAULT NULL,
  `pto_type_id` int(11) DEFAULT NULL,
  `pto_side_guid` int(11) DEFAULT NULL,
  `tr_len` int(11) DEFAULT NULL,
  `descript` blob,
  `last_update` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`transcript_guid`),
  KEY `FK_transcripts-pkg_guid` (`pkg_guid`),
  KEY `FK_transcripts-art_guid` (`art_guid`),
  KEY `FK_transcripts-genome_guid` (`genome_guid`),
  CONSTRAINT `FK_transcripts-art_guid` FOREIGN KEY (`art_guid`) REFERENCES `articles` (`art_guid`),
  CONSTRAINT `FK_transcripts-genome_guid` FOREIGN KEY (`genome_guid`) REFERENCES `dict_genomes` (`genome_guid`),
  CONSTRAINT `FK_transcripts-pkg_guid` FOREIGN KEY (`pkg_guid`) REFERENCES `packages` (`pkg_guid`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping events for database 'dbregulation_update'
--

--
-- Dumping routines for database 'dbregulation_update'
--
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2017-03-29 22:39:44
