CREATE DATABASE  IF NOT EXISTS `rfam_12_2` /*!40100 DEFAULT CHARACTER SET utf8 */;
USE `rfam_12_2`;
-- MySQL dump 10.13  Distrib 5.7.9, for Win64 (x86_64)
--
-- Host: localhost    Database: rfam_12_2
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
-- Table structure for table `alignment_and_tree`
--

DROP TABLE IF EXISTS `alignment_and_tree`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `alignment_and_tree` (
  `rfam_acc` varchar(7) NOT NULL,
  `type` enum('seed','seedTax','genome','genomeTax') NOT NULL,
  `alignment` longblob,
  `tree` longblob,
  `treemethod` tinytext,
  `average_length` double(7,2) DEFAULT NULL,
  `percent_id` double(5,2) DEFAULT NULL,
  `number_of_sequences` bigint(20) DEFAULT NULL,
  KEY `fk_alignments_and_trees_family1_idx` (`rfam_acc`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `clan`
--

DROP TABLE IF EXISTS `clan`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `clan` (
  `clan_acc` varchar(7) NOT NULL,
  `id` varchar(40) DEFAULT NULL,
  `previous_id` tinytext,
  `description` varchar(100) DEFAULT NULL,
  `author` tinytext,
  `comment` longtext,
  `created` datetime NOT NULL,
  `updated` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`clan_acc`),
  UNIQUE KEY `clan_acc` (`clan_acc`),
  UNIQUE KEY `clan_acc_2` (`clan_acc`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `clan_database_link`
--

DROP TABLE IF EXISTS `clan_database_link`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `clan_database_link` (
  `clan_acc` varchar(7) NOT NULL,
  `db_id` tinytext NOT NULL,
  `comment` tinytext,
  `db_link` tinytext NOT NULL,
  `other_params` tinytext,
  KEY `fk_clan_database_links_clan1_idx` (`clan_acc`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `clan_literature_reference`
--

DROP TABLE IF EXISTS `clan_literature_reference`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `clan_literature_reference` (
  `clan_acc` varchar(7) NOT NULL,
  `pmid` int(10) NOT NULL,
  `comment` tinytext,
  `order_added` tinyint(3) DEFAULT NULL,
  KEY `fk_clan_literature_references_clan1_idx` (`clan_acc`),
  KEY `fk_clan_literature_references_literature_reference1_idx` (`pmid`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `clan_membership`
--

DROP TABLE IF EXISTS `clan_membership`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `clan_membership` (
  `clan_acc` varchar(7) NOT NULL,
  `rfam_acc` varchar(7) NOT NULL,
  UNIQUE KEY `UniqueFamilyIdx` (`rfam_acc`),
  KEY `fk_clan_membership_family1_idx` (`rfam_acc`),
  KEY `fk_clan_membership_clan1_idx` (`clan_acc`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `database_link`
--

DROP TABLE IF EXISTS `database_link`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `database_link` (
  `rfam_acc` varchar(7) NOT NULL,
  `db_id` tinytext NOT NULL,
  `comment` tinytext,
  `db_link` tinytext NOT NULL,
  `other_params` tinytext,
  KEY `fk_rfam_database_link_family1_idx` (`rfam_acc`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `db_version`
--

DROP TABLE IF EXISTS `db_version`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `db_version` (
  `rfam_release` double(4,1) NOT NULL,
  `rfam_release_date` datetime NOT NULL,
  `number_families` int(10) NOT NULL,
  `embl_release` tinytext NOT NULL,
  `genome_collection_date` datetime DEFAULT NULL,
  `refseq_version` int(11) DEFAULT NULL,
  `pdb_date` datetime DEFAULT NULL,
  `infernal_version` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`rfam_release`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `dead_clan`
--

DROP TABLE IF EXISTS `dead_clan`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `dead_clan` (
  `clan_acc` varchar(7) NOT NULL DEFAULT '',
  `clan_id` varchar(40) NOT NULL COMMENT 'Added. Add author?',
  `comment` mediumtext,
  `forward_to` varchar(7) DEFAULT NULL,
  `user` tinytext NOT NULL,
  UNIQUE KEY `rfam_acc` (`clan_acc`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `dead_family`
--

DROP TABLE IF EXISTS `dead_family`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `dead_family` (
  `rfam_acc` varchar(7) NOT NULL DEFAULT '' COMMENT 'record the author???',
  `rfam_id` varchar(40) NOT NULL,
  `comment` mediumtext,
  `forward_to` varchar(7) DEFAULT NULL,
  `title` varchar(150) DEFAULT NULL COMMENT 'wikipedia page title\n',
  `user` tinytext NOT NULL,
  UNIQUE KEY `rfam_acc` (`rfam_acc`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `family`
--

DROP TABLE IF EXISTS `family`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `family` (
  `rfam_acc` varchar(7) NOT NULL,
  `rfam_id` varchar(40) NOT NULL,
  `auto_wiki` int(10) unsigned NOT NULL,
  `description` varchar(75) DEFAULT NULL,
  `author` tinytext,
  `seed_source` tinytext,
  `gathering_cutoff` double(5,2) DEFAULT NULL,
  `trusted_cutoff` double(5,2) DEFAULT NULL,
  `noise_cutoff` double(5,2) DEFAULT NULL,
  `comment` longtext,
  `previous_id` tinytext,
  `cmbuild` tinytext,
  `cmcalibrate` tinytext,
  `cmsearch` tinytext,
  `num_seed` bigint(20) DEFAULT NULL,
  `num_full` bigint(20) DEFAULT NULL,
  `num_genome_seq` bigint(20) DEFAULT NULL,
  `num_refseq` bigint(20) DEFAULT NULL,
  `type` varchar(50) DEFAULT NULL,
  `structure_source` tinytext,
  `number_of_species` bigint(20) DEFAULT NULL,
  `number_3d_structures` int(11) DEFAULT NULL,
  `tax_seed` mediumtext,
  `ecmli_lambda` double(10,5) DEFAULT NULL,
  `ecmli_mu` double(10,5) DEFAULT NULL,
  `ecmli_cal_db` mediumint(9) DEFAULT '0',
  `ecmli_cal_hits` mediumint(9) DEFAULT '0',
  `maxl` mediumint(9) DEFAULT '0',
  `clen` mediumint(9) DEFAULT '0',
  `match_pair_node` tinyint(1) DEFAULT '0',
  `hmm_tau` double(10,5) DEFAULT NULL,
  `hmm_lambda` double(10,5) DEFAULT NULL,
  `created` datetime NOT NULL,
  `updated` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`rfam_acc`),
  UNIQUE KEY `rfam_acc` (`rfam_acc`),
  KEY `rfam_id` (`rfam_id`),
  KEY `fk_family_wikitext1_idx` (`auto_wiki`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `family_literature_reference`
--

DROP TABLE IF EXISTS `family_literature_reference`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `family_literature_reference` (
  `rfam_acc` varchar(7) NOT NULL,
  `pmid` int(10) NOT NULL,
  `comment` tinytext,
  `order_added` tinyint(3) DEFAULT NULL,
  KEY `fk_family_literature_reference_family1_idx` (`rfam_acc`),
  KEY `fk_family_literature_reference_literature_reference1_idx` (`pmid`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `family_ncbi`
--

DROP TABLE IF EXISTS `family_ncbi`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `family_ncbi` (
  `ncbi_id` int(10) unsigned NOT NULL,
  `rfam_id` varchar(40) DEFAULT NULL COMMENT 'Is this really needed?',
  `rfam_acc` varchar(7) NOT NULL,
  KEY `fk_rfam_ncbi_family1_idx` (`rfam_acc`),
  KEY `fk_family_ncbi_taxonomy1_idx` (`ncbi_id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `features`
--

DROP TABLE IF EXISTS `features`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `features` (
  `rfamseq_acc` varchar(20) NOT NULL,
  `database_id` varchar(50) NOT NULL,
  `primary_id` varchar(100) NOT NULL,
  `secondary_id` varchar(255) DEFAULT NULL,
  `feat_orient` tinyint(3) NOT NULL DEFAULT '0',
  `feat_start` bigint(19) unsigned NOT NULL DEFAULT '0',
  `feat_end` bigint(19) unsigned NOT NULL DEFAULT '0',
  `quaternary_id` varchar(150) DEFAULT NULL,
  KEY `fk_features_rfamseq1_idx` (`rfamseq_acc`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `full_region`
--

DROP TABLE IF EXISTS `full_region`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `full_region` (
  `rfam_acc` varchar(7) NOT NULL,
  `rfamseq_acc` varchar(20) NOT NULL,
  `seq_start` bigint(19) unsigned NOT NULL DEFAULT '0',
  `seq_end` bigint(19) unsigned NOT NULL,
  `bit_score` double(7,2) NOT NULL DEFAULT '0.00' COMMENT '99999.99 is the approx limit from Infernal.',
  `evalue_score` varchar(15) NOT NULL DEFAULT '0',
  `cm_start` mediumint(8) unsigned NOT NULL,
  `cm_end` mediumint(8) unsigned NOT NULL,
  `truncated` enum('0','5','3','53') NOT NULL,
  `type` enum('seed','full') NOT NULL DEFAULT 'full',
  `is_significant` tinyint(1) unsigned NOT NULL,
  KEY `full_region_sign` (`is_significant`),
  KEY `full_region_acc_sign` (`rfam_acc`,`is_significant`) USING BTREE,
  KEY `fk_full_region_rfamseq1_cascase` (`rfamseq_acc`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `html_alignment`
--

DROP TABLE IF EXISTS `html_alignment`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `html_alignment` (
  `rfam_acc` varchar(7) NOT NULL,
  `type` enum('seed','genome','seedColorstock','genomeColorstock') NOT NULL,
  `html` longblob,
  `block` int(6) NOT NULL,
  `html_alignmentscol` varchar(45) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
  KEY `fk_html_alignments_family1_idx` (`rfam_acc`),
  KEY `htmlTypeIdx` (`type`),
  KEY `htmlBlockIdx` (`block`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `keywords`
--

DROP TABLE IF EXISTS `keywords`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `keywords` (
  `rfam_acc` varchar(7) NOT NULL DEFAULT '',
  `rfam_id` varchar(40) DEFAULT NULL,
  `description` varchar(100) DEFAULT 'NULL',
  `rfam_general` longtext,
  `literature` longtext,
  `wiki` longtext,
  `pdb_mappings` longtext,
  `clan_info` longtext,
  PRIMARY KEY (`rfam_acc`),
  FULLTEXT KEY `rfam_kw_idx` (`description`,`rfam_general`,`literature`,`wiki`,`pdb_mappings`,`clan_info`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `literature_reference`
--

DROP TABLE IF EXISTS `literature_reference`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `literature_reference` (
  `pmid` int(10) NOT NULL AUTO_INCREMENT,
  `title` tinytext,
  `author` text,
  `journal` tinytext,
  PRIMARY KEY (`pmid`)
) ENGINE=InnoDB AUTO_INCREMENT=97362240 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `matches_and_fasta`
--

DROP TABLE IF EXISTS `matches_and_fasta`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `matches_and_fasta` (
  `rfam_acc` varchar(7) NOT NULL,
  `match_list` longblob,
  `fasta` longblob,
  `type` enum('rfamseq','genome','refseq') NOT NULL,
  KEY `fk_matches_and_fasta_family1_idx` (`rfam_acc`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `motif`
--

DROP TABLE IF EXISTS `motif`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `motif` (
  `motif_acc` varchar(7) NOT NULL,
  `motif_id` varchar(40) DEFAULT NULL,
  `description` varchar(75) DEFAULT NULL,
  `author` tinytext,
  `seed_source` tinytext,
  `gathering_cutoff` double(5,2) DEFAULT NULL,
  `trusted_cutoff` double(5,2) DEFAULT NULL,
  `noise_cutoff` double(5,2) DEFAULT NULL,
  `cmbuild` tinytext,
  `cmcalibrate` tinytext,
  `type` varchar(50) DEFAULT NULL,
  `num_seed` bigint(20) DEFAULT NULL,
  `average_id` double(5,2) DEFAULT NULL,
  `average_sqlen` double(7,2) DEFAULT NULL,
  `ecmli_lambda` double(10,5) DEFAULT NULL,
  `ecmli_mu` double(10,5) DEFAULT NULL,
  `ecmli_cal_db` mediumint(9) DEFAULT '0',
  `ecmli_cal_hits` mediumint(9) DEFAULT '0',
  `maxl` mediumint(9) DEFAULT '0',
  `clen` mediumint(9) DEFAULT '0',
  `match_pair_node` tinyint(1) DEFAULT '0',
  `hmm_tau` double(10,5) DEFAULT NULL,
  `hmm_lambda` double(10,5) DEFAULT NULL,
  `wiki` varchar(80) DEFAULT NULL,
  `created` datetime NOT NULL,
  `updated` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`motif_acc`),
  KEY `motif_id` (`motif_id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `motif_database_link`
--

DROP TABLE IF EXISTS `motif_database_link`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `motif_database_link` (
  `motif_acc` varchar(7) NOT NULL,
  `db_id` tinytext NOT NULL,
  `comment` tinytext,
  `db_link` tinytext NOT NULL,
  `other_params` tinytext,
  KEY `motif_acc` (`motif_acc`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `motif_family_stats`
--

DROP TABLE IF EXISTS `motif_family_stats`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `motif_family_stats` (
  `rfam_acc` varchar(7) NOT NULL,
  `motif_acc` varchar(7) NOT NULL,
  `num_hits` int(11) DEFAULT NULL,
  `frac_hits` decimal(4,3) DEFAULT NULL,
  `sum_bits` decimal(12,3) DEFAULT NULL,
  `avg_weight_bits` decimal(12,3) DEFAULT NULL,
  KEY `motif_family_stats_rfam_acc_idx` (`rfam_acc`),
  KEY `motif_family_stats_motif_acc_idx` (`motif_acc`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `motif_file`
--

DROP TABLE IF EXISTS `motif_file`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `motif_file` (
  `motif_acc` varchar(7) NOT NULL,
  `seed` longblob NOT NULL,
  `cm` longblob NOT NULL,
  KEY `fk_motif_file_motif_idx` (`motif_acc`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `motif_literature`
--

DROP TABLE IF EXISTS `motif_literature`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `motif_literature` (
  `motif_acc` varchar(7) NOT NULL,
  `pmid` int(10) NOT NULL,
  `comment` tinytext,
  `order_added` tinyint(3) DEFAULT NULL,
  KEY `motif_literature_pmid_idx` (`pmid`),
  KEY `motif_literature_motif_acc` (`motif_acc`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `motif_matches`
--

DROP TABLE IF EXISTS `motif_matches`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `motif_matches` (
  `motif_acc` varchar(7) NOT NULL,
  `rfam_acc` varchar(7) NOT NULL,
  `rfamseq_acc` varchar(20) NOT NULL,
  `rfamseq_start` bigint(19) DEFAULT NULL,
  `rfamseq_stop` bigint(19) DEFAULT NULL,
  `query_start` int(11) DEFAULT NULL,
  `query_stop` int(11) DEFAULT NULL,
  `motif_start` int(11) DEFAULT NULL,
  `motif_stop` int(11) DEFAULT NULL,
  `e_value` varchar(15) DEFAULT NULL,
  `bit_score` double(7,2) DEFAULT NULL,
  KEY `motif_match_motif_acc_idx` (`motif_acc`),
  KEY `motif_match_rfam_acc_idx` (`rfam_acc`),
  KEY `motif_match_rfamseq_acc_idx` (`rfamseq_acc`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `motif_pdb`
--

DROP TABLE IF EXISTS `motif_pdb`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `motif_pdb` (
  `motif_acc` varchar(7) NOT NULL,
  `pdb_id` varchar(4) NOT NULL,
  `chain` varchar(4) DEFAULT NULL,
  `pdb_start` mediumint(9) DEFAULT NULL,
  `pdb_end` mediumint(9) DEFAULT NULL,
  KEY `motif_pdb_pdb_idx` (`pdb_id`),
  KEY `motif_pdb_motif_acc_idx` (`motif_acc`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `motif_ss_image`
--

DROP TABLE IF EXISTS `motif_ss_image`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `motif_ss_image` (
  `rfam_acc` varchar(7) NOT NULL,
  `motif_acc` varchar(7) NOT NULL,
  `image` longblob,
  KEY `fk_motif_ss_images_family1_idx` (`rfam_acc`),
  KEY `fk_motif_ss_images_motif1_idx` (`motif_acc`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `pdb_full_region`
--

DROP TABLE IF EXISTS `pdb_full_region`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `pdb_full_region` (
  `rfam_acc` varchar(7) NOT NULL,
  `pdb_id` varchar(4) NOT NULL,
  `chain` varchar(4) DEFAULT 'NULL',
  `pdb_start` mediumint(8) NOT NULL,
  `pdb_end` mediumint(8) NOT NULL,
  `bit_score` double(7,2) NOT NULL DEFAULT '0.00',
  `evalue_score` varchar(15) NOT NULL DEFAULT '0',
  `cm_start` mediumint(8) NOT NULL,
  `cm_end` mediumint(8) NOT NULL,
  `hex_colour` varchar(6) DEFAULT 'NULL',
  KEY `fk_pdb_rfam_reg_family1_idx` (`rfam_acc`),
  KEY `fk_pdb_rfam_reg_pdb1_idx` (`pdb_id`),
  KEY `rfam_acc` (`rfam_acc`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `rfamseq`
--

DROP TABLE IF EXISTS `rfamseq`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `rfamseq` (
  `rfamseq_acc` varchar(20) NOT NULL DEFAULT '' COMMENT 'This should be ',
  `accession` varchar(15) NOT NULL,
  `version` int(6) NOT NULL,
  `ncbi_id` int(10) unsigned NOT NULL,
  `mol_type` enum('protein','genomic DNA','DNA','ss-DNA','RNA','genomic RNA','ds-RNA','ss-cRNA','ss-RNA','mRNA','tRNA','rRNA','snoRNA','snRNA','scRNA','pre-RNA','other RNA','other DNA','unassigned DNA','unassigned RNA','viral cRNA','cRNA','transcribed RNA') NOT NULL,
  `length` int(10) unsigned DEFAULT '0',
  `description` varchar(250) NOT NULL DEFAULT '',
  `previous_acc` mediumtext,
  `source` char(20) NOT NULL,
  PRIMARY KEY (`rfamseq_acc`),
  UNIQUE KEY `rfamseq_acc` (`rfamseq_acc`),
  KEY `version` (`version`),
  KEY `fk_rfamseq_taxonomy1_idx` (`ncbi_id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `secondary_structure_image`
--

DROP TABLE IF EXISTS `secondary_structure_image`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `secondary_structure_image` (
  `rfam_acc` varchar(7) NOT NULL,
  `type` enum('cons','dist','ent','fcbp','cov','disttruc','maxcm','norm','rchie','species','ss','rscape','rscape-cyk') DEFAULT NULL,
  `image` longblob,
  KEY `fk_secondary_structure_images_family1_idx` (`rfam_acc`),
  KEY `secondatStructureTypeIdx` (`type`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `seed_region`
--

DROP TABLE IF EXISTS `seed_region`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `seed_region` (
  `rfam_acc` varchar(7) NOT NULL,
  `rfamseq_acc` varchar(20) NOT NULL,
  `seq_start` bigint(19) unsigned NOT NULL DEFAULT '0',
  `seq_end` bigint(19) unsigned NOT NULL,
  KEY `fk_rfam_reg_seed_family1_idx` (`rfam_acc`),
  KEY `fk_rfam_reg_seed_rfamseq1_idx` (`rfamseq_acc`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `sunburst`
--

DROP TABLE IF EXISTS `sunburst`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `sunburst` (
  `rfam_acc` varchar(7) NOT NULL,
  `data` longblob NOT NULL,
  `type` enum('rfamseq','genome','refseq') NOT NULL,
  KEY `fk_table1_family3_idx` (`rfam_acc`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `taxonomy`
--

DROP TABLE IF EXISTS `taxonomy`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `taxonomy` (
  `ncbi_id` int(10) unsigned NOT NULL DEFAULT '0',
  `species` varchar(100) NOT NULL DEFAULT '',
  `tax_string` mediumtext,
  `tree_display_name` varchar(100) DEFAULT NULL,
  `align_display_name` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`ncbi_id`),
  KEY `species` (`species`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `taxonomy_websearch`
--

DROP TABLE IF EXISTS `taxonomy_websearch`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `taxonomy_websearch` (
  `ncbi_id` int(10) unsigned DEFAULT '0',
  `species` varchar(100) DEFAULT NULL,
  `taxonomy` mediumtext,
  `lft` int(10) DEFAULT NULL,
  `rgt` int(10) DEFAULT NULL,
  `parent` int(10) unsigned DEFAULT NULL,
  `level` varchar(200) DEFAULT NULL,
  `minimal` tinyint(1) NOT NULL DEFAULT '0',
  `rank` varchar(100) DEFAULT NULL,
  KEY `taxonomy_lft_idx` (`lft`),
  KEY `taxonomy_rgt_idx` (`rgt`),
  KEY `taxonomy_level_text_idx` (`level`),
  KEY `taxonomy_species_text_idx` (`species`),
  KEY `minimal_idx` (`minimal`),
  KEY `parent` (`parent`),
  KEY `ncbi_id_idx` (`ncbi_id`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `version`
--

DROP TABLE IF EXISTS `version`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `version` (
  `rfam_release` double(4,1) NOT NULL,
  `rfam_release_date` date NOT NULL,
  `number_families` int(10) NOT NULL,
  `embl_release` tinytext NOT NULL,
  PRIMARY KEY (`rfam_release`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `wikitext`
--

DROP TABLE IF EXISTS `wikitext`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `wikitext` (
  `auto_wiki` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `title` varchar(150) CHARACTER SET utf8 COLLATE utf8_bin NOT NULL,
  PRIMARY KEY (`auto_wiki`),
  UNIQUE KEY `title_UNIQUE` (`title`)
) ENGINE=InnoDB AUTO_INCREMENT=2450 DEFAULT CHARSET=latin1 COLLATE=latin1_general_cs;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping events for database 'rfam_12_2'
--

--
-- Dumping routines for database 'rfam_12_2'
--
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2017-03-29 23:52:18
