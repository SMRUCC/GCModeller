CREATE DATABASE  IF NOT EXISTS `go` /*!40100 DEFAULT CHARACTER SET utf8 */;
USE `go`;
-- MySQL dump 10.13  Distrib 5.6.13, for Win32 (x86)
--
-- Host: localhost    Database: go
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
-- Table structure for table `assoc_rel`
--

DROP TABLE IF EXISTS `assoc_rel`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `assoc_rel` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `from_id` int(11) NOT NULL,
  `to_id` int(11) NOT NULL,
  `relationship_type_id` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `from_id` (`from_id`),
  KEY `to_id` (`to_id`),
  KEY `relationship_type_id` (`relationship_type_id`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `association`
--

DROP TABLE IF EXISTS `association`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `association` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `term_id` int(11) NOT NULL,
  `gene_product_id` int(11) NOT NULL,
  `is_not` int(11) DEFAULT NULL,
  `role_group` int(11) DEFAULT NULL,
  `assocdate` int(11) DEFAULT NULL,
  `source_db_id` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `a0` (`id`),
  KEY `source_db_id` (`source_db_id`),
  KEY `a1` (`term_id`),
  KEY `a2` (`gene_product_id`),
  KEY `a3` (`term_id`,`gene_product_id`),
  KEY `a4` (`id`,`term_id`,`gene_product_id`),
  KEY `a5` (`id`,`gene_product_id`),
  KEY `a6` (`is_not`,`term_id`,`gene_product_id`),
  KEY `a7` (`assocdate`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `association_isoform`
--

DROP TABLE IF EXISTS `association_isoform`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `association_isoform` (
  `association_id` int(11) NOT NULL,
  `gene_product_id` int(11) NOT NULL,
  KEY `association_id` (`association_id`),
  KEY `gene_product_id` (`gene_product_id`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `association_property`
--

DROP TABLE IF EXISTS `association_property`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `association_property` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `association_id` int(11) NOT NULL,
  `relationship_type_id` int(11) NOT NULL,
  `term_id` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `association_id` (`association_id`),
  KEY `relationship_type_id` (`relationship_type_id`),
  KEY `term_id` (`term_id`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `association_qualifier`
--

DROP TABLE IF EXISTS `association_qualifier`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `association_qualifier` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `association_id` int(11) NOT NULL,
  `term_id` int(11) NOT NULL,
  `value` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `term_id` (`term_id`),
  KEY `aq1` (`association_id`,`term_id`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `association_species_qualifier`
--

DROP TABLE IF EXISTS `association_species_qualifier`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `association_species_qualifier` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `association_id` int(11) NOT NULL,
  `species_id` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `association_id` (`association_id`),
  KEY `species_id` (`species_id`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `db`
--

DROP TABLE IF EXISTS `db`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `db` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(55) DEFAULT NULL,
  `fullname` varchar(255) DEFAULT NULL,
  `datatype` varchar(255) DEFAULT NULL,
  `generic_url` varchar(255) DEFAULT NULL,
  `url_syntax` varchar(255) DEFAULT NULL,
  `url_example` varchar(255) DEFAULT NULL,
  `uri_prefix` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `db0` (`id`),
  UNIQUE KEY `name` (`name`),
  KEY `db1` (`name`),
  KEY `db2` (`fullname`),
  KEY `db3` (`datatype`)
) ENGINE=MyISAM AUTO_INCREMENT=262 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `dbxref`
--

DROP TABLE IF EXISTS `dbxref`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `dbxref` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `xref_dbname` varchar(55) NOT NULL,
  `xref_key` varchar(255) NOT NULL,
  `xref_keytype` varchar(32) DEFAULT NULL,
  `xref_desc` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `xref_key` (`xref_key`,`xref_dbname`),
  UNIQUE KEY `dx0` (`id`),
  UNIQUE KEY `dx6` (`xref_key`,`xref_dbname`),
  KEY `dx1` (`xref_dbname`),
  KEY `dx2` (`xref_key`),
  KEY `dx3` (`id`,`xref_dbname`),
  KEY `dx4` (`id`,`xref_key`,`xref_dbname`),
  KEY `dx5` (`id`,`xref_key`)
) ENGINE=MyISAM AUTO_INCREMENT=85803 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `evidence`
--

DROP TABLE IF EXISTS `evidence`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `evidence` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `code` varchar(8) NOT NULL,
  `association_id` int(11) NOT NULL,
  `dbxref_id` int(11) NOT NULL,
  `seq_acc` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `association_id` (`association_id`,`dbxref_id`,`code`),
  UNIQUE KEY `ev0` (`id`),
  UNIQUE KEY `ev5` (`id`,`association_id`),
  UNIQUE KEY `ev6` (`id`,`code`,`association_id`),
  KEY `ev1` (`association_id`),
  KEY `ev2` (`code`),
  KEY `ev3` (`dbxref_id`),
  KEY `ev4` (`association_id`,`code`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `evidence_dbxref`
--

DROP TABLE IF EXISTS `evidence_dbxref`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `evidence_dbxref` (
  `evidence_id` int(11) NOT NULL,
  `dbxref_id` int(11) NOT NULL,
  KEY `evx1` (`evidence_id`),
  KEY `evx2` (`dbxref_id`),
  KEY `evx3` (`evidence_id`,`dbxref_id`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `gene_product`
--

DROP TABLE IF EXISTS `gene_product`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `gene_product` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `symbol` varchar(128) NOT NULL,
  `dbxref_id` int(11) NOT NULL,
  `species_id` int(11) DEFAULT NULL,
  `type_id` int(11) DEFAULT NULL,
  `full_name` text,
  PRIMARY KEY (`id`),
  UNIQUE KEY `dbxref_id` (`dbxref_id`),
  UNIQUE KEY `g0` (`id`),
  KEY `type_id` (`type_id`),
  KEY `g1` (`symbol`),
  KEY `g2` (`dbxref_id`),
  KEY `g3` (`species_id`),
  KEY `g4` (`id`,`species_id`),
  KEY `g5` (`dbxref_id`,`species_id`),
  KEY `g6` (`id`,`dbxref_id`),
  KEY `g7` (`id`,`species_id`),
  KEY `g8` (`id`,`dbxref_id`,`species_id`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `gene_product_ancestor`
--

DROP TABLE IF EXISTS `gene_product_ancestor`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `gene_product_ancestor` (
  `gene_product_id` int(11) NOT NULL,
  `ancestor_id` int(11) NOT NULL,
  `phylotree_id` int(11) NOT NULL,
  `branch_length` float DEFAULT NULL,
  `is_transitive` int(11) NOT NULL DEFAULT '0',
  UNIQUE KEY `gene_product_id` (`gene_product_id`,`ancestor_id`,`phylotree_id`),
  KEY `ancestor_id` (`ancestor_id`),
  KEY `phylotree_id` (`phylotree_id`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `gene_product_count`
--

DROP TABLE IF EXISTS `gene_product_count`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `gene_product_count` (
  `term_id` int(11) NOT NULL,
  `code` varchar(8) DEFAULT NULL,
  `speciesdbname` varchar(55) DEFAULT NULL,
  `species_id` int(11) DEFAULT NULL,
  `product_count` int(11) NOT NULL,
  KEY `species_id` (`species_id`),
  KEY `gpc1` (`term_id`),
  KEY `gpc2` (`code`),
  KEY `gpc3` (`speciesdbname`),
  KEY `gpc4` (`term_id`,`code`,`speciesdbname`),
  KEY `gpc5` (`term_id`,`species_id`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `gene_product_dbxref`
--

DROP TABLE IF EXISTS `gene_product_dbxref`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `gene_product_dbxref` (
  `gene_product_id` int(11) NOT NULL,
  `dbxref_id` int(11) NOT NULL,
  UNIQUE KEY `gpx3` (`gene_product_id`,`dbxref_id`),
  KEY `gpx1` (`gene_product_id`),
  KEY `gpx2` (`dbxref_id`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `gene_product_homology`
--

DROP TABLE IF EXISTS `gene_product_homology`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `gene_product_homology` (
  `gene_product1_id` int(11) NOT NULL,
  `gene_product2_id` int(11) NOT NULL,
  `relationship_type_id` int(11) NOT NULL,
  KEY `gene_product1_id` (`gene_product1_id`),
  KEY `gene_product2_id` (`gene_product2_id`),
  KEY `relationship_type_id` (`relationship_type_id`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `gene_product_homolset`
--

DROP TABLE IF EXISTS `gene_product_homolset`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `gene_product_homolset` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `gene_product_id` int(11) NOT NULL,
  `homolset_id` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `gene_product_id` (`gene_product_id`),
  KEY `homolset_id` (`homolset_id`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `gene_product_phylotree`
--

DROP TABLE IF EXISTS `gene_product_phylotree`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `gene_product_phylotree` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `gene_product_id` int(11) NOT NULL,
  `phylotree_id` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `gene_product_id` (`gene_product_id`),
  KEY `phylotree_id` (`phylotree_id`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `gene_product_property`
--

DROP TABLE IF EXISTS `gene_product_property`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `gene_product_property` (
  `gene_product_id` int(11) NOT NULL,
  `property_key` varchar(64) NOT NULL,
  `property_val` varchar(255) DEFAULT NULL,
  UNIQUE KEY `gppu4` (`gene_product_id`,`property_key`,`property_val`),
  KEY `gpp1` (`gene_product_id`),
  KEY `gpp2` (`property_key`),
  KEY `gpp3` (`property_val`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `gene_product_seq`
--

DROP TABLE IF EXISTS `gene_product_seq`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `gene_product_seq` (
  `gene_product_id` int(11) NOT NULL,
  `seq_id` int(11) NOT NULL,
  `is_primary_seq` int(11) DEFAULT NULL,
  KEY `gpseq1` (`gene_product_id`),
  KEY `gpseq2` (`seq_id`),
  KEY `gpseq3` (`seq_id`,`gene_product_id`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `gene_product_subset`
--

DROP TABLE IF EXISTS `gene_product_subset`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `gene_product_subset` (
  `gene_product_id` int(11) NOT NULL,
  `subset_id` int(11) NOT NULL,
  UNIQUE KEY `gps3` (`gene_product_id`,`subset_id`),
  KEY `gps1` (`gene_product_id`),
  KEY `gps2` (`subset_id`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `gene_product_synonym`
--

DROP TABLE IF EXISTS `gene_product_synonym`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `gene_product_synonym` (
  `gene_product_id` int(11) NOT NULL,
  `product_synonym` varchar(255) NOT NULL,
  UNIQUE KEY `gene_product_id` (`gene_product_id`,`product_synonym`),
  KEY `gs1` (`gene_product_id`),
  KEY `gs2` (`product_synonym`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `graph_path`
--

DROP TABLE IF EXISTS `graph_path`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `graph_path` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `term1_id` int(11) NOT NULL,
  `term2_id` int(11) NOT NULL,
  `relationship_type_id` int(11) DEFAULT NULL,
  `distance` int(11) DEFAULT NULL,
  `relation_distance` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `graph_path0` (`id`),
  KEY `relationship_type_id` (`relationship_type_id`),
  KEY `graph_path1` (`term1_id`),
  KEY `graph_path2` (`term2_id`),
  KEY `graph_path3` (`term1_id`,`term2_id`),
  KEY `graph_path4` (`term1_id`,`distance`),
  KEY `graph_path5` (`term1_id`,`term2_id`,`relationship_type_id`),
  KEY `graph_path6` (`term1_id`,`term2_id`,`relationship_type_id`,`distance`,`relation_distance`),
  KEY `graph_path7` (`term2_id`,`relationship_type_id`),
  KEY `graph_path8` (`term1_id`,`relationship_type_id`)
) ENGINE=MyISAM AUTO_INCREMENT=1226557 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `graph_path2term`
--

DROP TABLE IF EXISTS `graph_path2term`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `graph_path2term` (
  `graph_path_id` int(11) NOT NULL,
  `term_id` int(11) NOT NULL,
  `rank` int(11) NOT NULL,
  KEY `graph_path_id` (`graph_path_id`),
  KEY `term_id` (`term_id`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `homolset`
--

DROP TABLE IF EXISTS `homolset`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `homolset` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `symbol` varchar(128) DEFAULT NULL,
  `dbxref_id` int(11) DEFAULT NULL,
  `target_gene_product_id` int(11) DEFAULT NULL,
  `taxon_id` int(11) DEFAULT NULL,
  `type_id` int(11) DEFAULT NULL,
  `description` text,
  PRIMARY KEY (`id`),
  UNIQUE KEY `dbxref_id` (`dbxref_id`),
  KEY `target_gene_product_id` (`target_gene_product_id`),
  KEY `taxon_id` (`taxon_id`),
  KEY `type_id` (`type_id`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `instance_data`
--

DROP TABLE IF EXISTS `instance_data`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `instance_data` (
  `release_name` varchar(255) DEFAULT NULL,
  `release_type` varchar(255) DEFAULT NULL,
  `release_notes` text,
  `ontology_data_version` varchar(255) DEFAULT NULL,
  UNIQUE KEY `release_name` (`release_name`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `intersection_of`
--

DROP TABLE IF EXISTS `intersection_of`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `intersection_of` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `relationship_type_id` int(11) NOT NULL,
  `term1_id` int(11) NOT NULL,
  `term2_id` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `term1_id` (`term1_id`,`term2_id`,`relationship_type_id`),
  KEY `relationship_type_id` (`relationship_type_id`),
  KEY `term2_id` (`term2_id`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `phylotree`
--

DROP TABLE IF EXISTS `phylotree`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `phylotree` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) NOT NULL DEFAULT '',
  `dbxref_id` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `dbxref_id` (`dbxref_id`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `phylotree_property`
--

DROP TABLE IF EXISTS `phylotree_property`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `phylotree_property` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `phylotree_id` int(11) NOT NULL,
  `property_key` varchar(64) NOT NULL,
  `property_val` mediumtext,
  PRIMARY KEY (`id`),
  KEY `phylotree_id` (`phylotree_id`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `relation_composition`
--

DROP TABLE IF EXISTS `relation_composition`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `relation_composition` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `relation1_id` int(11) NOT NULL,
  `relation2_id` int(11) NOT NULL,
  `inferred_relation_id` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `relation1_id` (`relation1_id`,`relation2_id`,`inferred_relation_id`),
  KEY `rc1` (`relation1_id`),
  KEY `rc2` (`relation2_id`),
  KEY `rc3` (`inferred_relation_id`),
  KEY `rc4` (`relation1_id`,`relation2_id`,`inferred_relation_id`)
) ENGINE=MyISAM AUTO_INCREMENT=20 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `relation_properties`
--

DROP TABLE IF EXISTS `relation_properties`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `relation_properties` (
  `relationship_type_id` int(11) NOT NULL,
  `is_transitive` int(11) DEFAULT NULL,
  `is_symmetric` int(11) DEFAULT NULL,
  `is_anti_symmetric` int(11) DEFAULT NULL,
  `is_cyclic` int(11) DEFAULT NULL,
  `is_reflexive` int(11) DEFAULT NULL,
  `is_metadata_tag` int(11) DEFAULT NULL,
  UNIQUE KEY `relationship_type_id` (`relationship_type_id`),
  UNIQUE KEY `rp1` (`relationship_type_id`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `seq`
--

DROP TABLE IF EXISTS `seq`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `seq` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `display_id` varchar(64) DEFAULT NULL,
  `description` varchar(255) DEFAULT NULL,
  `seq` mediumtext,
  `seq_len` int(11) DEFAULT NULL,
  `md5checksum` varchar(32) DEFAULT NULL,
  `moltype` varchar(25) DEFAULT NULL,
  `timestamp` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `seq0` (`id`),
  UNIQUE KEY `display_id` (`display_id`,`md5checksum`),
  KEY `seq1` (`display_id`),
  KEY `seq2` (`md5checksum`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `seq_dbxref`
--

DROP TABLE IF EXISTS `seq_dbxref`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `seq_dbxref` (
  `seq_id` int(11) NOT NULL,
  `dbxref_id` int(11) NOT NULL,
  UNIQUE KEY `seq_id` (`seq_id`,`dbxref_id`),
  KEY `seqx0` (`seq_id`),
  KEY `seqx1` (`dbxref_id`),
  KEY `seqx2` (`seq_id`,`dbxref_id`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `seq_property`
--

DROP TABLE IF EXISTS `seq_property`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `seq_property` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `seq_id` int(11) NOT NULL,
  `property_key` varchar(64) NOT NULL,
  `property_val` varchar(255) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `seq_id` (`seq_id`,`property_key`,`property_val`),
  KEY `seqp0` (`seq_id`),
  KEY `seqp1` (`property_key`),
  KEY `seqp2` (`property_val`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `source_audit`
--

DROP TABLE IF EXISTS `source_audit`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `source_audit` (
  `source_id` varchar(255) DEFAULT NULL,
  `source_fullpath` varchar(255) DEFAULT NULL,
  `source_path` varchar(255) DEFAULT NULL,
  `source_type` varchar(255) DEFAULT NULL,
  `source_md5` char(32) DEFAULT NULL,
  `source_parsetime` int(11) DEFAULT NULL,
  `source_mtime` int(11) DEFAULT NULL,
  KEY `fa1` (`source_path`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `species`
--

DROP TABLE IF EXISTS `species`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `species` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `ncbi_taxa_id` int(11) DEFAULT NULL,
  `common_name` varchar(255) DEFAULT NULL,
  `lineage_string` text,
  `genus` varchar(55) DEFAULT NULL,
  `species` varchar(255) DEFAULT NULL,
  `parent_id` int(11) DEFAULT NULL,
  `left_value` int(11) DEFAULT NULL,
  `right_value` int(11) DEFAULT NULL,
  `taxonomic_rank` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `sp0` (`id`),
  UNIQUE KEY `ncbi_taxa_id` (`ncbi_taxa_id`),
  KEY `sp1` (`ncbi_taxa_id`),
  KEY `sp2` (`common_name`),
  KEY `sp3` (`genus`),
  KEY `sp4` (`species`),
  KEY `sp5` (`genus`,`species`),
  KEY `sp6` (`id`,`ncbi_taxa_id`),
  KEY `sp7` (`id`,`ncbi_taxa_id`,`genus`,`species`),
  KEY `sp8` (`parent_id`),
  KEY `sp9` (`left_value`),
  KEY `sp10` (`right_value`),
  KEY `sp11` (`left_value`,`right_value`),
  KEY `sp12` (`id`,`left_value`),
  KEY `sp13` (`genus`,`left_value`,`right_value`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `term`
--

DROP TABLE IF EXISTS `term`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `term` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) NOT NULL DEFAULT '',
  `term_type` varchar(55) NOT NULL,
  `acc` varchar(255) NOT NULL,
  `is_obsolete` int(11) NOT NULL DEFAULT '0',
  `is_root` int(11) NOT NULL DEFAULT '0',
  `is_relation` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  UNIQUE KEY `acc` (`acc`),
  UNIQUE KEY `t0` (`id`),
  KEY `t1` (`name`),
  KEY `t2` (`term_type`),
  KEY `t3` (`acc`),
  KEY `t4` (`id`,`acc`),
  KEY `t5` (`id`,`name`),
  KEY `t6` (`id`,`term_type`),
  KEY `t7` (`id`,`acc`,`name`,`term_type`)
) ENGINE=MyISAM AUTO_INCREMENT=43827 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `term2term`
--

DROP TABLE IF EXISTS `term2term`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `term2term` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `relationship_type_id` int(11) NOT NULL,
  `term1_id` int(11) NOT NULL,
  `term2_id` int(11) NOT NULL,
  `complete` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  UNIQUE KEY `term1_id` (`term1_id`,`term2_id`,`relationship_type_id`),
  KEY `tt1` (`term1_id`),
  KEY `tt2` (`term2_id`),
  KEY `tt3` (`term1_id`,`term2_id`),
  KEY `tt4` (`relationship_type_id`)
) ENGINE=MyISAM AUTO_INCREMENT=89342 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `term2term_metadata`
--

DROP TABLE IF EXISTS `term2term_metadata`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `term2term_metadata` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `relationship_type_id` int(11) NOT NULL,
  `term1_id` int(11) NOT NULL,
  `term2_id` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `term1_id` (`term1_id`,`term2_id`),
  KEY `relationship_type_id` (`relationship_type_id`),
  KEY `term2_id` (`term2_id`)
) ENGINE=MyISAM AUTO_INCREMENT=2317 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `term_audit`
--

DROP TABLE IF EXISTS `term_audit`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `term_audit` (
  `term_id` int(11) NOT NULL,
  `term_loadtime` int(11) DEFAULT NULL,
  UNIQUE KEY `term_id` (`term_id`),
  KEY `ta1` (`term_id`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `term_dbxref`
--

DROP TABLE IF EXISTS `term_dbxref`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `term_dbxref` (
  `term_id` int(11) NOT NULL,
  `dbxref_id` int(11) NOT NULL,
  `is_for_definition` int(11) NOT NULL DEFAULT '0',
  UNIQUE KEY `term_id` (`term_id`,`dbxref_id`,`is_for_definition`),
  KEY `tx0` (`term_id`),
  KEY `tx1` (`dbxref_id`),
  KEY `tx2` (`term_id`,`dbxref_id`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `term_definition`
--

DROP TABLE IF EXISTS `term_definition`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `term_definition` (
  `term_id` int(11) NOT NULL,
  `term_definition` text NOT NULL,
  `dbxref_id` int(11) DEFAULT NULL,
  `term_comment` mediumtext,
  `reference` varchar(255) DEFAULT NULL,
  UNIQUE KEY `term_id` (`term_id`),
  KEY `dbxref_id` (`dbxref_id`),
  KEY `td1` (`term_id`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `term_property`
--

DROP TABLE IF EXISTS `term_property`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `term_property` (
  `term_id` int(11) NOT NULL,
  `property_key` varchar(64) NOT NULL,
  `property_val` varchar(255) DEFAULT NULL,
  KEY `term_id` (`term_id`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `term_subset`
--

DROP TABLE IF EXISTS `term_subset`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `term_subset` (
  `term_id` int(11) NOT NULL,
  `subset_id` int(11) NOT NULL,
  KEY `tss1` (`term_id`),
  KEY `tss2` (`subset_id`),
  KEY `tss3` (`term_id`,`subset_id`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `term_synonym`
--

DROP TABLE IF EXISTS `term_synonym`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `term_synonym` (
  `term_id` int(11) NOT NULL,
  `term_synonym` varchar(996) DEFAULT NULL,
  `acc_synonym` varchar(255) DEFAULT NULL,
  `synonym_type_id` int(11) NOT NULL,
  `synonym_category_id` int(11) DEFAULT NULL,
  UNIQUE KEY `term_id` (`term_id`,`term_synonym`),
  KEY `synonym_type_id` (`synonym_type_id`),
  KEY `synonym_category_id` (`synonym_category_id`),
  KEY `ts1` (`term_id`),
  KEY `ts2` (`term_synonym`),
  KEY `ts3` (`term_id`,`term_synonym`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2015-12-03 20:47:31
