CREATE DATABASE  IF NOT EXISTS `kb_uniprotkb` /*!40100 DEFAULT CHARACTER SET utf8 */;
USE `kb_uniprotkb`;
-- MySQL dump 10.13  Distrib 5.7.17, for Win64 (x86_64)
--
-- Host: localhost    Database: kb_uniprotkb
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
  `primary_hashcode` int(10) unsigned NOT NULL,
  `uniprot_id` varchar(45) DEFAULT NULL,
  `alt_id` int(10) unsigned NOT NULL,
  `name` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`primary_hashcode`,`alt_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='当uniprot的XML数据库之中的某一条蛋白质的entry由多个uniprot编号的时候，在这个表之中就会记录下其他的编号信息，默认取entry记录的第一个accession编号为主编号';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `hash_table`
--

DROP TABLE IF EXISTS `hash_table`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `hash_table` (
  `uniprot_id` char(32) NOT NULL COMMENT 'uniprot数据库编号首先会在这个表之中进行查找，得到自己唯一的哈希值结果，然后再根据这个哈希值去快速的查找其他的表之中的结果',
  `hash_code` int(10) unsigned NOT NULL AUTO_INCREMENT COMMENT '每一个字符串形式的uniprot数据库编号都有一个唯一的哈希值编号',
  `name` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`uniprot_id`),
  UNIQUE KEY `uniprot_id_UNIQUE` (`uniprot_id`),
  UNIQUE KEY `hash_code_UNIQUE` (`hash_code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `literature`
--

DROP TABLE IF EXISTS `literature`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `literature` (
  `uid` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `type` varchar(45) DEFAULT NULL,
  `date` varchar(45) DEFAULT NULL,
  `db` varchar(45) DEFAULT NULL,
  `title` varchar(45) DEFAULT NULL,
  `author_list` varchar(45) DEFAULT NULL,
  `pubmed` varchar(45) DEFAULT NULL,
  `doi` varchar(45) DEFAULT NULL,
  `volume` varchar(45) DEFAULT NULL,
  `pages` varchar(45) DEFAULT NULL,
  `journal` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`uid`),
  UNIQUE KEY `uid_UNIQUE` (`uid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='文献报道数据';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `location_id`
--

DROP TABLE IF EXISTS `location_id`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `location_id` (
  `uid` int(10) unsigned NOT NULL,
  `name` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`uid`),
  UNIQUE KEY `uid_UNIQUE` (`uid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `organism_code`
--

DROP TABLE IF EXISTS `organism_code`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `organism_code` (
  `uid` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `organism_name` varchar(100) NOT NULL,
  `domain` varchar(45) DEFAULT NULL,
  `kingdom` varchar(45) DEFAULT NULL,
  `phylum` varchar(45) DEFAULT NULL,
  `class` varchar(45) DEFAULT NULL,
  `order` varchar(45) DEFAULT NULL,
  `family` varchar(45) DEFAULT NULL,
  `genus` varchar(45) DEFAULT NULL,
  `species` varchar(45) DEFAULT NULL,
  `full` mediumtext NOT NULL COMMENT '除了前面的标准的分类层次之外，在这里还有包含有非标准的分类层次的信息，使用json字符串存放这些物种分类信息',
  PRIMARY KEY (`uid`),
  UNIQUE KEY `organism_name_UNIQUE` (`organism_name`),
  UNIQUE KEY `uid_UNIQUE` (`uid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `organism_proteome`
--

DROP TABLE IF EXISTS `organism_proteome`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `organism_proteome` (
  `org_id` int(10) unsigned NOT NULL,
  `uniprot_id` varchar(45) DEFAULT NULL,
  `id_hashcode` int(10) unsigned NOT NULL,
  `gene_name` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`org_id`,`id_hashcode`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='这个表之中列举出了某一个物种其基因组之中所拥有的蛋白质的集合';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `protein_functions`
--

DROP TABLE IF EXISTS `protein_functions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `protein_functions` (
  `hash_code` int(10) unsigned NOT NULL,
  `uniprot_id` varchar(45) DEFAULT NULL,
  `function` varchar(45) DEFAULT NULL COMMENT 'comment -> type = function',
  `name` varchar(45) DEFAULT NULL,
  `full_name` varchar(45) DEFAULT NULL,
  `short_name1` varchar(45) DEFAULT NULL,
  `short_name2` varchar(45) DEFAULT NULL,
  `short_name3` varchar(45) DEFAULT NULL,
  `protein_functionscol` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`hash_code`),
  UNIQUE KEY `hash_code_UNIQUE` (`hash_code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='对蛋白质的名称以及功能方面的字符串描述';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `protein_go`
--

DROP TABLE IF EXISTS `protein_go`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `protein_go` (
  `hash_code` int(10) unsigned NOT NULL,
  `uniprot_id` varchar(45) NOT NULL,
  `go_id` int(10) unsigned NOT NULL,
  PRIMARY KEY (`hash_code`,`go_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='对蛋白质的GO功能注释的信息关联表';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `protein_ko`
--

DROP TABLE IF EXISTS `protein_ko`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `protein_ko` (
  `hash_code` int(10) unsigned NOT NULL,
  `uniprot_id` varchar(45) DEFAULT NULL,
  `KO` int(10) unsigned NOT NULL,
  PRIMARY KEY (`hash_code`,`KO`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='蛋白质的KEGG直系同源的注释信息表';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `protein_reference`
--

DROP TABLE IF EXISTS `protein_reference`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `protein_reference` (
  `hash_code` int(10) unsigned NOT NULL,
  `uniprot_id` varchar(45) DEFAULT NULL,
  `reference_id` int(10) unsigned NOT NULL,
  `scope` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`hash_code`,`reference_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='对这个蛋白质的文献报道数据';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `protein_subcellular_location`
--

DROP TABLE IF EXISTS `protein_subcellular_location`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `protein_subcellular_location` (
  `hash_code` int(10) unsigned NOT NULL,
  `uniprot_id` varchar(45) DEFAULT NULL,
  `location` varchar(45) DEFAULT NULL,
  `location_id` int(10) unsigned DEFAULT NULL,
  `topology` varchar(45) DEFAULT NULL,
  `topology_id` int(10) unsigned DEFAULT NULL,
  PRIMARY KEY (`hash_code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='目标蛋白质在细胞质中的亚细胞定位结果';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `seq_archive`
--

DROP TABLE IF EXISTS `seq_archive`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `seq_archive` (
  `hash_code` int(10) unsigned NOT NULL,
  `uniprot_id` varchar(45) NOT NULL COMMENT 'UniqueIdentifier Is the primary accession number of the UniProtKB entry.(对hash_code起校验用)',
  `entry_name` varchar(45) DEFAULT NULL COMMENT 'EntryName Is the entry name of the UniProtKB entry.',
  `organism_id` int(10) unsigned NOT NULL COMMENT 'OrganismName Is the scientific name of the organism of the UniProtKB entry, this is the id reference to the organism_code table.',
  `organism_name` longtext NOT NULL COMMENT '对organism_id校验所使用的',
  `gn` varchar(45) DEFAULT NULL COMMENT 'GeneName Is the first gene name of the UniProtKB entry. If there Is no gene name, OrderedLocusName Or ORFname, the GN field Is Not listed.',
  `pe` varchar(45) DEFAULT NULL COMMENT 'ProteinExistence Is the numerical value describing the evidence for the existence of the protein.',
  `sv` varchar(45) DEFAULT NULL COMMENT 'SequenceVersion Is the version number of the sequence.',
  `prot_name` tinytext COMMENT 'ProteinName Is the recommended name of the UniProtKB entry as annotated in the RecName field. For UniProtKB/TrEMBL entries without a RecName field, the SubName field Is used. In case of multiple SubNames, the first one Is used. The ''precursor'' attribute is excluded, ''Fragment'' is included with the name if applicable.',
  `length` int(11) DEFAULT NULL COMMENT 'length of the protein sequence',
  `sequence` text COMMENT 'protein sequence',
  PRIMARY KEY (`hash_code`,`uniprot_id`),
  UNIQUE KEY `uniprot_id_UNIQUE` (`uniprot_id`),
  UNIQUE KEY `hash_code_UNIQUE` (`hash_code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='蛋白质序列存储表';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `topology_id`
--

DROP TABLE IF EXISTS `topology_id`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `topology_id` (
  `uid` int(10) unsigned NOT NULL,
  `name` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`uid`),
  UNIQUE KEY `uid_UNIQUE` (`uid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping events for database 'kb_uniprotkb'
--

--
-- Dumping routines for database 'kb_uniprotkb'
--
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2017-09-02 23:46:51
