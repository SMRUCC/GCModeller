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
-- Table structure for table `feature_site_variation`
--

DROP TABLE IF EXISTS `feature_site_variation`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `feature_site_variation` (
  `uid` int(10) unsigned NOT NULL,
  `hash_code` int(10) unsigned NOT NULL,
  `uniprot_id` varchar(45) DEFAULT NULL,
  `original` varchar(45) DEFAULT NULL,
  `variation` varchar(45) DEFAULT NULL,
  `position` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`uid`,`hash_code`),
  UNIQUE KEY `uid_UNIQUE` (`uid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='序列的突变位点';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `feature_types`
--

DROP TABLE IF EXISTS `feature_types`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `feature_types` (
  `uid` int(10) unsigned NOT NULL,
  `type_name` varchar(45) NOT NULL,
  PRIMARY KEY (`uid`),
  UNIQUE KEY `uid_UNIQUE` (`uid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `gene_info`
--

DROP TABLE IF EXISTS `gene_info`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `gene_info` (
  `hash_code` int(10) unsigned NOT NULL,
  `uniprot_id` varchar(45) DEFAULT NULL,
  `gene_name` varchar(45) DEFAULT NULL,
  `ORF` varchar(45) DEFAULT NULL,
  `synonym1` varchar(45) DEFAULT NULL,
  `synonym2` varchar(45) DEFAULT NULL,
  `synonym3` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`hash_code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
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
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='这个表主要是为了加快整个数据库的查询效率而建立的冗余表，在这里为每一个uniport accession编号都赋值了一个唯一编号，然后利用这个唯一编号就可以实现对其他数据表之中的数据的快速查询了';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `keywords`
--

DROP TABLE IF EXISTS `keywords`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `keywords` (
  `uid` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `keyword` varchar(45) NOT NULL,
  PRIMARY KEY (`uid`),
  UNIQUE KEY `uid_UNIQUE` (`uid`)
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
  `uid` int(10) unsigned NOT NULL COMMENT '在这里使用的是NCBI Taxonomy编号',
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
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='物种信息简表';
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
-- Table structure for table `peoples`
--

DROP TABLE IF EXISTS `peoples`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `peoples` (
  `uid` int(10) unsigned NOT NULL,
  `name` varchar(45) NOT NULL,
  PRIMARY KEY (`uid`),
  UNIQUE KEY `uid_UNIQUE` (`uid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `protein_alternative_name`
--

DROP TABLE IF EXISTS `protein_alternative_name`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `protein_alternative_name` (
  `hash_code` int(10) unsigned NOT NULL,
  `uniprot_id` varchar(45) NOT NULL,
  `name` varchar(45) NOT NULL,
  `fullName` varchar(45) DEFAULT NULL,
  `shortName1` varchar(45) DEFAULT NULL,
  `shortName2` varchar(45) DEFAULT NULL,
  `shortName3` varchar(45) DEFAULT NULL,
  `shortName4` varchar(45) DEFAULT NULL,
  `shortName5` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`hash_code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `protein_feature_regions`
--

DROP TABLE IF EXISTS `protein_feature_regions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `protein_feature_regions` (
  `uid` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `hash_code` int(10) unsigned NOT NULL,
  `uniprot_id` varchar(45) DEFAULT NULL,
  `type_id` int(10) unsigned NOT NULL,
  `type` varchar(45) DEFAULT NULL,
  `description` varchar(45) DEFAULT NULL,
  `begin` varchar(45) DEFAULT NULL,
  `end` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`uid`),
  UNIQUE KEY `uid_UNIQUE` (`uid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `protein_feature_site`
--

DROP TABLE IF EXISTS `protein_feature_site`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `protein_feature_site` (
  `uid` int(10) unsigned NOT NULL,
  `hash_code` int(10) unsigned NOT NULL,
  `uniprot_id` varchar(45) DEFAULT NULL,
  `type_id` int(10) unsigned NOT NULL,
  `type` varchar(45) DEFAULT NULL,
  `description` varchar(45) DEFAULT NULL,
  `position` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`uid`),
  UNIQUE KEY `uid_UNIQUE` (`uid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
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
  `full_name` varchar(45) DEFAULT NULL COMMENT 'recommendedName',
  `short_name1` varchar(45) DEFAULT NULL COMMENT 'recommendedName',
  `short_name2` varchar(45) DEFAULT NULL COMMENT 'recommendedName',
  `short_name3` varchar(45) DEFAULT NULL COMMENT 'recommendedName',
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
  `namespace_id` int(10) unsigned NOT NULL,
  `namespace` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`hash_code`,`go_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='对蛋白质的GO功能注释的信息关联表';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `protein_keywords`
--

DROP TABLE IF EXISTS `protein_keywords`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `protein_keywords` (
  `hash_code` int(10) unsigned NOT NULL,
  `uniprot_id` varchar(45) NOT NULL,
  `keyword_id` int(10) unsigned NOT NULL,
  `keyword` varchar(45) NOT NULL,
  PRIMARY KEY (`hash_code`,`keyword_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
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
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='蛋白质的KEGG直系同源的注释信息表，uniprotKB库通过这个表连接kegg知识库';
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
-- Table structure for table `protein_structures`
--

DROP TABLE IF EXISTS `protein_structures`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `protein_structures` (
  `uid` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `hash_code` int(10) unsigned NOT NULL,
  `uniprot_id` varchar(45) NOT NULL,
  `pdb_id` varchar(45) DEFAULT NULL,
  `method` varchar(45) DEFAULT NULL,
  `resolution` varchar(45) DEFAULT NULL,
  `chains` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`uid`),
  UNIQUE KEY `uid_UNIQUE` (`uid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='主要是pdb结构记录数据';
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
-- Table structure for table `research_jobs`
--

DROP TABLE IF EXISTS `research_jobs`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `research_jobs` (
  `person` int(10) unsigned NOT NULL,
  `people_name` varchar(45) DEFAULT NULL,
  `literature_id` int(10) unsigned NOT NULL,
  `literature_title` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`person`,`literature_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
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
-- Table structure for table `tissue_code`
--

DROP TABLE IF EXISTS `tissue_code`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tissue_code` (
  `uid` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `tissue_name` varchar(45) NOT NULL,
  `org_id` int(10) unsigned DEFAULT NULL,
  `organism` varchar(45) DEFAULT NULL COMMENT '物种名称',
  PRIMARY KEY (`uid`),
  UNIQUE KEY `uid_UNIQUE` (`uid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='对某一个物种的组织进行编号';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tissue_locations`
--

DROP TABLE IF EXISTS `tissue_locations`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tissue_locations` (
  `hash_code` int(10) unsigned NOT NULL,
  `uniprot_id` varchar(45) DEFAULT NULL,
  `name` varchar(45) DEFAULT NULL,
  `tissue_id` int(10) unsigned NOT NULL,
  `tissue_name` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`hash_code`,`tissue_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
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
-- Table structure for table `xref`
--

DROP TABLE IF EXISTS `xref`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `xref` (
  `hash_code` int(10) unsigned NOT NULL,
  `uniprot_id` varchar(45) DEFAULT NULL,
  `xref` varchar(45) NOT NULL,
  `external_id` varchar(45) NOT NULL,
  `molecule_type` varchar(45) DEFAULT NULL,
  `protein_ID` varchar(45) DEFAULT NULL,
  `nucleotide_ID` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`hash_code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='某一个uniprot蛋白质记录对外部的链接信息';
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

-- Dump completed on 2017-09-03  5:49:19
