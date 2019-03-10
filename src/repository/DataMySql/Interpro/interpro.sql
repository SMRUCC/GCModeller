CREATE DATABASE  IF NOT EXISTS `interpro` /*!40100 DEFAULT CHARACTER SET latin1 */;
USE `interpro`;
-- MySQL dump 10.13  Distrib 5.6.13, for Win32 (x86)
--
-- Host: localhost    Database: interpro
-- ------------------------------------------------------
-- Server version	5.6.17

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
-- Table structure for table `abstract`
--

DROP TABLE IF EXISTS `abstract`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `abstract` (
  `entry_ac` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `abstract` mediumtext CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `comments` varchar(100) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  PRIMARY KEY (`entry_ac`),
  CONSTRAINT `fk_abstract$entry_ac` FOREIGN KEY (`entry_ac`) REFERENCES `entry` (`entry_ac`) ON DELETE CASCADE ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `author`
--

DROP TABLE IF EXISTS `author`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `author` (
  `author_id` int(9) NOT NULL,
  `name` varchar(80) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `uppercase` varchar(80) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  PRIMARY KEY (`author_id`),
  UNIQUE KEY `ui_author$id$name` (`author_id`,`name`),
  UNIQUE KEY `uq_author$name` (`name`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `book`
--

DROP TABLE IF EXISTS `book`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `book` (
  `isbn` varchar(10) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `title` mediumtext CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `edition` int(3) DEFAULT NULL,
  `publisher` varchar(200) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  `pubplace` varchar(50) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  PRIMARY KEY (`isbn`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `book2author`
--

DROP TABLE IF EXISTS `book2author`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `book2author` (
  `isbn` varchar(10) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `author_id` int(9) NOT NULL,
  `order_in` int(3) NOT NULL,
  PRIMARY KEY (`isbn`,`order_in`,`author_id`),
  UNIQUE KEY `uq_book2author$1` (`isbn`,`order_in`),
  KEY `i_book2author$fk_author_id` (`author_id`),
  CONSTRAINT `fk_book2author$author_id` FOREIGN KEY (`author_id`) REFERENCES `author` (`author_id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_book2author$isbn` FOREIGN KEY (`isbn`) REFERENCES `book` (`isbn`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `common_annotation`
--

DROP TABLE IF EXISTS `common_annotation`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `common_annotation` (
  `ann_id` varchar(7) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `name` varchar(50) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  `text` mediumtext CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `comments` varchar(100) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  PRIMARY KEY (`ann_id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `cv_database`
--

DROP TABLE IF EXISTS `cv_database`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cv_database` (
  `dbcode` char(1) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `dbname` varchar(20) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `dborder` int(5) NOT NULL,
  `dbshort` varchar(10) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  PRIMARY KEY (`dbcode`),
  UNIQUE KEY `uq_cv_database$database` (`dbname`),
  UNIQUE KEY `uq_cv_database$dborder` (`dborder`),
  UNIQUE KEY `uq_cv_database$dbshort` (`dbshort`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `cv_entry_type`
--

DROP TABLE IF EXISTS `cv_entry_type`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cv_entry_type` (
  `code` char(1) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `abbrev` varchar(20) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `description` mediumtext CHARACTER SET latin1 COLLATE latin1_bin,
  PRIMARY KEY (`code`),
  UNIQUE KEY `uq_entry_type$abbrev` (`abbrev`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `cv_evidence`
--

DROP TABLE IF EXISTS `cv_evidence`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cv_evidence` (
  `code` char(3) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `abbrev` varchar(10) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `description` mediumtext CHARACTER SET latin1 COLLATE latin1_bin,
  PRIMARY KEY (`code`),
  UNIQUE KEY `uq_evidence$abbrev` (`abbrev`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `cv_rank`
--

DROP TABLE IF EXISTS `cv_rank`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cv_rank` (
  `rank` varchar(20) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  `seq` bigint(15) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `cv_relation`
--

DROP TABLE IF EXISTS `cv_relation`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cv_relation` (
  `code` char(2) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `abbrev` varchar(10) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `description` mediumtext CHARACTER SET latin1 COLLATE latin1_bin,
  `forward` varchar(30) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `reverse` varchar(30) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  PRIMARY KEY (`code`),
  UNIQUE KEY `uq_relation$abbrev` (`abbrev`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `cv_synonym`
--

DROP TABLE IF EXISTS `cv_synonym`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cv_synonym` (
  `code` char(4) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `description` varchar(80) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  PRIMARY KEY (`code`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `db_version`
--

DROP TABLE IF EXISTS `db_version`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `db_version` (
  `dbcode` char(1) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `version` varchar(20) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `entry_count` bigint(10) NOT NULL,
  `file_date` datetime NOT NULL,
  `load_date` datetime NOT NULL,
  PRIMARY KEY (`dbcode`),
  CONSTRAINT `fk_db_version$dbcode` FOREIGN KEY (`dbcode`) REFERENCES `cv_database` (`dbcode`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `entry`
--

DROP TABLE IF EXISTS `entry`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `entry` (
  `entry_ac` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `entry_type` char(1) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `name` varchar(100) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  `created` datetime NOT NULL,
  `short_name` varchar(30) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  PRIMARY KEY (`entry_ac`),
  KEY `i_fk_entry$entry_type` (`entry_type`),
  KEY `fk_entry$entry_type` (`entry_type`),
  CONSTRAINT `fk_entry$entry_type` FOREIGN KEY (`entry_type`) REFERENCES `cv_entry_type` (`code`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `entry2common`
--

DROP TABLE IF EXISTS `entry2common`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `entry2common` (
  `entry_ac` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `ann_id` varchar(7) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `order_in` int(3) NOT NULL,
  PRIMARY KEY (`entry_ac`,`ann_id`,`order_in`),
  KEY `fk_entry2common$ann_id` (`ann_id`),
  CONSTRAINT `fk_entry2common$ann_id` FOREIGN KEY (`ann_id`) REFERENCES `common_annotation` (`ann_id`) ON DELETE CASCADE ON UPDATE NO ACTION,
  CONSTRAINT `fk_entry2common$entry_ac` FOREIGN KEY (`entry_ac`) REFERENCES `entry` (`entry_ac`) ON DELETE CASCADE ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `entry2comp`
--

DROP TABLE IF EXISTS `entry2comp`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `entry2comp` (
  `entry1_ac` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `entry2_ac` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `relation` char(2) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  PRIMARY KEY (`entry1_ac`,`entry2_ac`),
  KEY `fk_entry2comp$relation` (`relation`),
  KEY `fk_entry2comp$2` (`entry2_ac`),
  CONSTRAINT `fk_entry2comp$relation` FOREIGN KEY (`relation`) REFERENCES `cv_relation` (`code`) ON DELETE CASCADE ON UPDATE NO ACTION,
  CONSTRAINT `fk_entry2comp$1` FOREIGN KEY (`entry1_ac`) REFERENCES `entry` (`entry_ac`) ON DELETE CASCADE ON UPDATE NO ACTION,
  CONSTRAINT `fk_entry2comp$2` FOREIGN KEY (`entry2_ac`) REFERENCES `entry` (`entry_ac`) ON DELETE CASCADE ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `entry2entry`
--

DROP TABLE IF EXISTS `entry2entry`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `entry2entry` (
  `entry_ac` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `parent_ac` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `relation` char(2) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  PRIMARY KEY (`entry_ac`,`parent_ac`),
  KEY `fk_entry2entry$parent` (`parent_ac`),
  KEY `fk_entry2entry$relation` (`relation`),
  CONSTRAINT `fk_entry2entry$child` FOREIGN KEY (`entry_ac`) REFERENCES `entry` (`entry_ac`) ON DELETE CASCADE ON UPDATE NO ACTION,
  CONSTRAINT `fk_entry2entry$parent` FOREIGN KEY (`parent_ac`) REFERENCES `entry` (`entry_ac`) ON DELETE CASCADE ON UPDATE NO ACTION,
  CONSTRAINT `fk_entry2entry$relation` FOREIGN KEY (`relation`) REFERENCES `cv_relation` (`code`) ON DELETE CASCADE ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `entry2method`
--

DROP TABLE IF EXISTS `entry2method`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `entry2method` (
  `entry_ac` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `method_ac` varchar(25) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `evidence` char(3) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `ida` char(1) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  PRIMARY KEY (`entry_ac`,`method_ac`),
  KEY `fk_entry2method$evidence` (`evidence`),
  KEY `fk_entry2method$method` (`method_ac`),
  CONSTRAINT `fk_entry2method$entry` FOREIGN KEY (`entry_ac`) REFERENCES `entry` (`entry_ac`) ON DELETE CASCADE ON UPDATE NO ACTION,
  CONSTRAINT `fk_entry2method$evidence` FOREIGN KEY (`evidence`) REFERENCES `cv_evidence` (`code`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_entry2method$method` FOREIGN KEY (`method_ac`) REFERENCES `method` (`method_ac`) ON DELETE CASCADE ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `entry2pub`
--

DROP TABLE IF EXISTS `entry2pub`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `entry2pub` (
  `entry_ac` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `order_in` int(3) NOT NULL,
  `pub_id` varchar(11) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  PRIMARY KEY (`entry_ac`,`pub_id`),
  KEY `fk_entry2pub$pub` (`pub_id`),
  CONSTRAINT `fk_entry2pub$entry` FOREIGN KEY (`entry_ac`) REFERENCES `entry` (`entry_ac`) ON DELETE CASCADE ON UPDATE NO ACTION,
  CONSTRAINT `fk_entry2pub$pub` FOREIGN KEY (`pub_id`) REFERENCES `pub` (`pub_id`) ON DELETE CASCADE ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `entry_accpair`
--

DROP TABLE IF EXISTS `entry_accpair`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `entry_accpair` (
  `entry_ac` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `secondary_ac` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  PRIMARY KEY (`entry_ac`,`secondary_ac`),
  CONSTRAINT `fk_entry_accpair$ac1` FOREIGN KEY (`entry_ac`) REFERENCES `entry` (`entry_ac`) ON DELETE CASCADE ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `entry_deleted`
--

DROP TABLE IF EXISTS `entry_deleted`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `entry_deleted` (
  `entry_ac` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `remark` varchar(50) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  PRIMARY KEY (`entry_ac`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `entry_friends`
--

DROP TABLE IF EXISTS `entry_friends`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `entry_friends` (
  `entry1_ac` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `entry2_ac` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `s` int(3) NOT NULL,
  `p1` int(7) NOT NULL,
  `p2` int(7) NOT NULL,
  `pb` int(7) NOT NULL,
  `a1` int(5) NOT NULL,
  `a2` int(5) NOT NULL,
  `ab` int(5) NOT NULL,
  PRIMARY KEY (`entry1_ac`,`entry2_ac`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `entry_xref`
--

DROP TABLE IF EXISTS `entry_xref`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `entry_xref` (
  `entry_ac` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `dbcode` char(1) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `ac` varchar(30) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `name` varchar(70) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  PRIMARY KEY (`entry_ac`,`dbcode`,`ac`),
  KEY `fk_entry_xref$dbcode` (`dbcode`),
  CONSTRAINT `fk_entry_xref$dbcode` FOREIGN KEY (`dbcode`) REFERENCES `cv_database` (`dbcode`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_entry_xref$entry_ac` FOREIGN KEY (`entry_ac`) REFERENCES `entry` (`entry_ac`) ON DELETE CASCADE ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `etaxi`
--

DROP TABLE IF EXISTS `etaxi`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `etaxi` (
  `tax_id` bigint(15) NOT NULL,
  `parent_id` bigint(15) DEFAULT NULL,
  `scientific_name` varchar(100) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `complete_genome_flag` char(1) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `rank` varchar(20) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  `hidden` int(3) NOT NULL,
  `left_number` bigint(15) DEFAULT NULL,
  `right_number` bigint(15) DEFAULT NULL,
  `annotation_source` varchar(30) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `full_name` mediumtext CHARACTER SET latin1 COLLATE latin1_bin
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `example`
--

DROP TABLE IF EXISTS `example`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `example` (
  `entry_ac` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `protein_ac` varchar(6) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  PRIMARY KEY (`entry_ac`,`protein_ac`),
  KEY `fk_example$protein_ac` (`protein_ac`),
  CONSTRAINT `fk_example$entry_ac` FOREIGN KEY (`entry_ac`) REFERENCES `entry` (`entry_ac`) ON DELETE CASCADE ON UPDATE NO ACTION,
  CONSTRAINT `fk_example$protein_ac` FOREIGN KEY (`protein_ac`) REFERENCES `protein` (`protein_ac`) ON DELETE CASCADE ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `example_auto`
--

DROP TABLE IF EXISTS `example_auto`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `example_auto` (
  `entry_ac` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `protein_ac` varchar(6) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `intact_data`
--

DROP TABLE IF EXISTS `intact_data`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `intact_data` (
  `uniprot_id` varchar(20) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `protein_ac` varchar(30) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `undetermined` char(1) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  `intact_id` varchar(30) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `interacts_with` varchar(30) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `type` varchar(20) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  `entry_ac` varchar(30) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `pubmed_id` varchar(30) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  PRIMARY KEY (`entry_ac`,`intact_id`,`interacts_with`,`protein_ac`),
  CONSTRAINT `fk_intact_data$entry_ac` FOREIGN KEY (`entry_ac`) REFERENCES `entry` (`entry_ac`) ON DELETE CASCADE ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `interpro2go`
--

DROP TABLE IF EXISTS `interpro2go`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `interpro2go` (
  `entry_ac` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `go_id` varchar(10) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `journal`
--

DROP TABLE IF EXISTS `journal`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `journal` (
  `issn` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `abbrev` varchar(60) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `uppercase` varchar(60) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  `start_date` datetime DEFAULT NULL,
  `end_date` datetime DEFAULT NULL,
  PRIMARY KEY (`issn`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `journal_syn`
--

DROP TABLE IF EXISTS `journal_syn`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `journal_syn` (
  `issn` varchar(10) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `code` char(4) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `text` varchar(200) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `uppercase` varchar(200) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  PRIMARY KEY (`issn`,`text`,`code`),
  KEY `fk_journal_syn$code` (`code`),
  CONSTRAINT `fk_journal_syn$code` FOREIGN KEY (`code`) REFERENCES `cv_synonym` (`code`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_journal_syn$issn` FOREIGN KEY (`issn`) REFERENCES `journal` (`issn`) ON DELETE CASCADE ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `match_struct`
--

DROP TABLE IF EXISTS `match_struct`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `match_struct` (
  `protein_ac` varchar(10) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `domain_id` varchar(14) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `pos_from` int(5) NOT NULL,
  `pos_to` int(5) DEFAULT NULL,
  `dbcode` varchar(1) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  PRIMARY KEY (`protein_ac`,`domain_id`,`pos_from`),
  KEY `fk_match_struct` (`domain_id`),
  CONSTRAINT `fk_match_struct` FOREIGN KEY (`domain_id`) REFERENCES `struct_class` (`domain_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `matches`
--

DROP TABLE IF EXISTS `matches`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `matches` (
  `protein_ac` varchar(12) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `method_ac` varchar(25) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `pos_from` int(5) DEFAULT NULL,
  `pos_to` int(5) DEFAULT NULL,
  `status` char(1) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `evidence` char(3) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  `match_date` datetime NOT NULL,
  `seq_date` datetime NOT NULL,
  `score` double DEFAULT NULL,
  KEY `fk_matches$evidence` (`evidence`),
  KEY `fk_matches$method` (`method_ac`),
  CONSTRAINT `fk_matches$evidence` FOREIGN KEY (`evidence`) REFERENCES `cv_evidence` (`code`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_matches$method` FOREIGN KEY (`method_ac`) REFERENCES `method` (`method_ac`) ON DELETE CASCADE ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `method`
--

DROP TABLE IF EXISTS `method`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `method` (
  `method_ac` varchar(25) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `name` varchar(30) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `dbcode` char(1) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `method_date` datetime NOT NULL,
  `skip_flag` char(1) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL DEFAULT 'N',
  `candidate` char(1) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  PRIMARY KEY (`method_ac`),
  KEY `fk_method$dbcode` (`dbcode`),
  CONSTRAINT `fk_method$dbcode` FOREIGN KEY (`dbcode`) REFERENCES `cv_database` (`dbcode`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `method2pub`
--

DROP TABLE IF EXISTS `method2pub`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `method2pub` (
  `pub_id` varchar(11) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `method_ac` varchar(25) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  PRIMARY KEY (`method_ac`,`pub_id`),
  KEY `fk_method2pub$pub_id` (`pub_id`),
  CONSTRAINT `fk_method2pub$method` FOREIGN KEY (`method_ac`) REFERENCES `method` (`method_ac`) ON DELETE CASCADE ON UPDATE NO ACTION,
  CONSTRAINT `fk_method2pub$pub_id` FOREIGN KEY (`pub_id`) REFERENCES `pub` (`pub_id`) ON DELETE CASCADE ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `mv_entry2protein`
--

DROP TABLE IF EXISTS `mv_entry2protein`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `mv_entry2protein` (
  `entry_ac` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `protein_ac` varchar(6) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `match_count` int(7) NOT NULL,
  PRIMARY KEY (`entry_ac`,`protein_ac`),
  KEY `fk_mv_entry2protein$protein` (`protein_ac`),
  CONSTRAINT `fk_mv_entry2protein$entry` FOREIGN KEY (`entry_ac`) REFERENCES `entry` (`entry_ac`) ON DELETE CASCADE ON UPDATE NO ACTION,
  CONSTRAINT `fk_mv_entry2protein$protein` FOREIGN KEY (`protein_ac`) REFERENCES `protein` (`protein_ac`) ON DELETE CASCADE ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `mv_entry2protein_true`
--

DROP TABLE IF EXISTS `mv_entry2protein_true`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `mv_entry2protein_true` (
  `entry_ac` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `protein_ac` varchar(6) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `match_count` int(7) NOT NULL,
  PRIMARY KEY (`entry_ac`,`protein_ac`),
  KEY `fk_mv_entry2protein_true$p` (`protein_ac`),
  CONSTRAINT `fk_mv_entry2protein_true$e` FOREIGN KEY (`entry_ac`) REFERENCES `entry` (`entry_ac`) ON DELETE CASCADE ON UPDATE NO ACTION,
  CONSTRAINT `fk_mv_entry2protein_true$p` FOREIGN KEY (`protein_ac`) REFERENCES `protein` (`protein_ac`) ON DELETE CASCADE ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `mv_entry_match`
--

DROP TABLE IF EXISTS `mv_entry_match`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `mv_entry_match` (
  `entry_ac` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `protein_count` int(7) NOT NULL,
  `match_count` int(7) NOT NULL,
  PRIMARY KEY (`entry_ac`),
  CONSTRAINT `fk_mv_entry_match$entry` FOREIGN KEY (`entry_ac`) REFERENCES `entry` (`entry_ac`) ON DELETE CASCADE ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `mv_method2protein`
--

DROP TABLE IF EXISTS `mv_method2protein`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `mv_method2protein` (
  `method_ac` varchar(25) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `protein_ac` varchar(6) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `match_count` int(7) NOT NULL,
  PRIMARY KEY (`method_ac`,`protein_ac`),
  KEY `fk_mv_method2protein$protein` (`protein_ac`),
  CONSTRAINT `fk_mv_method2protein$protein` FOREIGN KEY (`protein_ac`) REFERENCES `protein` (`protein_ac`) ON DELETE CASCADE ON UPDATE NO ACTION,
  CONSTRAINT `fk_mv_method2prot$method` FOREIGN KEY (`method_ac`) REFERENCES `method` (`method_ac`) ON DELETE CASCADE ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `mv_method_match`
--

DROP TABLE IF EXISTS `mv_method_match`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `mv_method_match` (
  `method_ac` varchar(25) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `protein_count` int(7) NOT NULL,
  `match_count` int(7) NOT NULL,
  PRIMARY KEY (`method_ac`),
  CONSTRAINT `fk_mv_method_match$method` FOREIGN KEY (`method_ac`) REFERENCES `method` (`method_ac`) ON DELETE CASCADE ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `mv_proteome_count`
--

DROP TABLE IF EXISTS `mv_proteome_count`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `mv_proteome_count` (
  `entry_ac` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `name` varchar(100) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `oscode` varchar(5) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `protein_count` int(7) NOT NULL,
  `method_count` int(7) NOT NULL,
  PRIMARY KEY (`entry_ac`,`oscode`),
  KEY `fk_mv_proteome_count$oscode` (`oscode`),
  CONSTRAINT `fk_mv_proteome_count$entry` FOREIGN KEY (`entry_ac`) REFERENCES `entry` (`entry_ac`) ON DELETE CASCADE ON UPDATE NO ACTION,
  CONSTRAINT `fk_mv_proteome_count$oscode` FOREIGN KEY (`oscode`) REFERENCES `organism` (`oscode`) ON DELETE CASCADE ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `mv_secondary`
--

DROP TABLE IF EXISTS `mv_secondary`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `mv_secondary` (
  `entry_ac` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `secondary_ac` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `method_ac` varchar(25) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `mv_tax_entry_count`
--

DROP TABLE IF EXISTS `mv_tax_entry_count`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `mv_tax_entry_count` (
  `entry_ac` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `tax_id` decimal(22,0) NOT NULL,
  `count` decimal(22,0) NOT NULL,
  PRIMARY KEY (`entry_ac`,`tax_id`,`count`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `organism`
--

DROP TABLE IF EXISTS `organism`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `organism` (
  `oscode` varchar(5) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `name` varchar(100) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `italics_name` varchar(100) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `full_name` varchar(100) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  `tax_code` decimal(38,0) DEFAULT NULL,
  PRIMARY KEY (`oscode`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `pdb_pub_additional`
--

DROP TABLE IF EXISTS `pdb_pub_additional`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `pdb_pub_additional` (
  `entry_ac` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `pub_id` varchar(11) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  KEY `fk_pdbpubadd$entry_ac` (`entry_ac`),
  KEY `fk_pdbpubadd$pub_id` (`pub_id`),
  CONSTRAINT `fk_pdbpubadd$entry_ac` FOREIGN KEY (`entry_ac`) REFERENCES `entry` (`entry_ac`) ON DELETE CASCADE ON UPDATE NO ACTION,
  CONSTRAINT `fk_pdbpubadd$pub_id` FOREIGN KEY (`pub_id`) REFERENCES `pub` (`pub_id`) ON DELETE CASCADE ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `pfam_clan`
--

DROP TABLE IF EXISTS `pfam_clan`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `pfam_clan` (
  `clan_id` varchar(15) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  `method_ac` varchar(25) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `pfam_clan_data`
--

DROP TABLE IF EXISTS `pfam_clan_data`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `pfam_clan_data` (
  `clan_id` varchar(15) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `name` varchar(50) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `description` varchar(75) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  PRIMARY KEY (`clan_id`,`name`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `protein`
--

DROP TABLE IF EXISTS `protein`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `protein` (
  `protein_ac` varchar(6) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `name` varchar(12) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `dbcode` char(1) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `crc64` char(16) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `len` int(5) NOT NULL,
  `fragment` char(1) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `struct_flag` char(1) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `tax_id` bigint(15) DEFAULT NULL,
  PRIMARY KEY (`protein_ac`),
  KEY `fk_protein$dbcode` (`dbcode`),
  CONSTRAINT `fk_protein$dbcode` FOREIGN KEY (`dbcode`) REFERENCES `cv_database` (`dbcode`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `protein2genome`
--

DROP TABLE IF EXISTS `protein2genome`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `protein2genome` (
  `protein_ac` varchar(6) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `oscode` varchar(5) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  PRIMARY KEY (`oscode`,`protein_ac`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `protein_accpair`
--

DROP TABLE IF EXISTS `protein_accpair`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `protein_accpair` (
  `protein_ac` varchar(6) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `secondary_ac` varchar(6) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  PRIMARY KEY (`protein_ac`,`secondary_ac`),
  CONSTRAINT `fk_accpair$primary` FOREIGN KEY (`protein_ac`) REFERENCES `protein` (`protein_ac`) ON DELETE CASCADE ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `protein_ida`
--

DROP TABLE IF EXISTS `protein_ida`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `protein_ida` (
  `protein_ac` varchar(6) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `ida` mediumtext CHARACTER SET latin1 COLLATE latin1_bin,
  PRIMARY KEY (`protein_ac`),
  CONSTRAINT `fk_protein_ida_p` FOREIGN KEY (`protein_ac`) REFERENCES `protein` (`protein_ac`) ON DELETE CASCADE ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `proteome_rank`
--

DROP TABLE IF EXISTS `proteome_rank`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `proteome_rank` (
  `entry_ac` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `oscode` varchar(5) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `rank` int(7) NOT NULL,
  PRIMARY KEY (`entry_ac`,`oscode`),
  KEY `fk_proteome_rank$oscode` (`oscode`),
  CONSTRAINT `fk_proteome_rank$entry` FOREIGN KEY (`entry_ac`) REFERENCES `entry` (`entry_ac`) ON DELETE CASCADE ON UPDATE NO ACTION,
  CONSTRAINT `fk_proteome_rank$oscode` FOREIGN KEY (`oscode`) REFERENCES `organism` (`oscode`) ON DELETE CASCADE ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `pub`
--

DROP TABLE IF EXISTS `pub`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `pub` (
  `pub_id` varchar(11) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `pub_type` char(1) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `medline_id` int(9) DEFAULT NULL,
  `issn` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  `isbn` varchar(10) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  `volume` varchar(5) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  `issue` varchar(5) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  `firstpage` int(6) DEFAULT NULL,
  `lastpage` int(6) DEFAULT NULL,
  `year` int(4) NOT NULL,
  `title` mediumtext CHARACTER SET latin1 COLLATE latin1_bin,
  `url` mediumtext CHARACTER SET latin1 COLLATE latin1_bin,
  `rawpages` varchar(30) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  `pubmed_id` bigint(10) DEFAULT NULL,
  PRIMARY KEY (`pub_id`),
  KEY `fk_pub$issn` (`issn`),
  CONSTRAINT `fk_pub$issn` FOREIGN KEY (`issn`) REFERENCES `journal` (`issn`) ON DELETE CASCADE ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `pub2author`
--

DROP TABLE IF EXISTS `pub2author`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `pub2author` (
  `pub_id` varchar(11) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `author_id` int(9) NOT NULL,
  `order_in` int(3) NOT NULL,
  PRIMARY KEY (`pub_id`,`author_id`,`order_in`),
  KEY `fk_pub2author$author_id` (`author_id`),
  CONSTRAINT `fk_pub2author$author_id` FOREIGN KEY (`author_id`) REFERENCES `author` (`author_id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_pub2author$pub_id` FOREIGN KEY (`pub_id`) REFERENCES `pub` (`pub_id`) ON DELETE CASCADE ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `struct_class`
--

DROP TABLE IF EXISTS `struct_class`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `struct_class` (
  `domain_id` varchar(14) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `fam_id` varchar(20) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  `dbcode` varchar(1) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  PRIMARY KEY (`domain_id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `supermatch`
--

DROP TABLE IF EXISTS `supermatch`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `supermatch` (
  `protein_ac` varchar(6) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `entry_ac` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `pos_from` int(5) NOT NULL,
  `pos_to` int(5) NOT NULL,
  PRIMARY KEY (`protein_ac`,`entry_ac`,`pos_to`,`pos_from`),
  KEY `fkv_supermatch$entry_ac` (`entry_ac`),
  CONSTRAINT `fkv_supermatch$entry_ac` FOREIGN KEY (`entry_ac`) REFERENCES `entry` (`entry_ac`) ON DELETE CASCADE ON UPDATE NO ACTION,
  CONSTRAINT `fkv_supermatch$protein_ac` FOREIGN KEY (`protein_ac`) REFERENCES `protein` (`protein_ac`) ON DELETE CASCADE ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tax_entry_count`
--

DROP TABLE IF EXISTS `tax_entry_count`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tax_entry_count` (
  `entry_ac` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  `tax_name` varchar(30) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  `count` int(5) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tax_name_to_id`
--

DROP TABLE IF EXISTS `tax_name_to_id`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tax_name_to_id` (
  `tax_name` varchar(30) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  `tax_id` bigint(15) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `taxonomy2protein`
--

DROP TABLE IF EXISTS `taxonomy2protein`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `taxonomy2protein` (
  `protein_ac` varchar(6) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `tax_id` bigint(15) NOT NULL,
  `hierarchy` varchar(200) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  `tax_name_concat` varchar(80) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  PRIMARY KEY (`protein_ac`,`tax_id`),
  CONSTRAINT `fk_taxonomy2protein$p` FOREIGN KEY (`protein_ac`) REFERENCES `protein` (`protein_ac`) ON DELETE CASCADE ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `text_index_entry`
--

DROP TABLE IF EXISTS `text_index_entry`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `text_index_entry` (
  `id` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  `field` varchar(20) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  `text` varchar(100) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `uniprot_taxonomy`
--

DROP TABLE IF EXISTS `uniprot_taxonomy`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `uniprot_taxonomy` (
  `protein_ac` varchar(10) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `tax_id` bigint(15) NOT NULL,
  `left_number` bigint(15) NOT NULL,
  `right_number` bigint(15) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `varsplic_master`
--

DROP TABLE IF EXISTS `varsplic_master`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `varsplic_master` (
  `protein_ac` varchar(6) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  `variant` int(3) DEFAULT NULL,
  `crc64` varchar(16) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  `length` int(5) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `varsplic_match`
--

DROP TABLE IF EXISTS `varsplic_match`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `varsplic_match` (
  `protein_ac` varchar(12) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `method_ac` varchar(25) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `pos_from` int(5) DEFAULT NULL,
  `pos_to` int(5) DEFAULT NULL,
  `status` char(1) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `dbcode` char(1) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `evidence` char(3) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  `seq_date` datetime NOT NULL,
  `match_date` datetime NOT NULL,
  `score` double DEFAULT NULL,
  KEY `fk_varsplic_match$dbcode` (`dbcode`),
  KEY `fk_varsplic_match$evidence` (`evidence`),
  KEY `fk_varsplic_match$method` (`method_ac`),
  CONSTRAINT `fk_varsplic_match$dbcode` FOREIGN KEY (`dbcode`) REFERENCES `cv_database` (`dbcode`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_varsplic_match$evidence` FOREIGN KEY (`evidence`) REFERENCES `cv_evidence` (`code`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_varsplic_match$method` FOREIGN KEY (`method_ac`) REFERENCES `method` (`method_ac`) ON DELETE CASCADE ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `varsplic_supermatch`
--

DROP TABLE IF EXISTS `varsplic_supermatch`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `varsplic_supermatch` (
  `protein_ac` varchar(12) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `entry_ac` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `pos_from` int(5) NOT NULL,
  `pos_to` int(5) NOT NULL,
  PRIMARY KEY (`protein_ac`,`entry_ac`,`pos_from`,`pos_to`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2015-11-20 18:54:43
