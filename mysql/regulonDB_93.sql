CREATE DATABASE  IF NOT EXISTS `regulondb_93` /*!40100 DEFAULT CHARACTER SET utf8 */;
USE `regulondb_93`;
-- MySQL dump 10.13  Distrib 5.7.9, for Win64 (x86_64)
--
-- Host: localhost    Database: regulondb_93
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
-- Table structure for table `alignment`
--

DROP TABLE IF EXISTS `alignment`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `alignment` (
  `tf_alignment_id` char(12) NOT NULL,
  `site_id` char(12) NOT NULL,
  `alignment_sequence` varchar(255) NOT NULL,
  `alignment_score_sequence` decimal(10,0) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `attenuator`
--

DROP TABLE IF EXISTS `attenuator`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `attenuator` (
  `attenuator_id` varchar(12) NOT NULL,
  `gene_id` char(12) DEFAULT NULL,
  `attenuator_type` varchar(16) DEFAULT NULL,
  `attenuator_strand` varchar(12) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `attenuator_terminator`
--

DROP TABLE IF EXISTS `attenuator_terminator`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `attenuator_terminator` (
  `a_terminator_id` varchar(12) NOT NULL,
  `a_terminator_type` varchar(25) DEFAULT NULL,
  `a_terminator_posleft` decimal(10,0) DEFAULT NULL,
  `a_terminator_posright` decimal(10,0) DEFAULT NULL,
  `a_terminator_energy` decimal(7,2) DEFAULT NULL,
  `a_terminator_sequence` varchar(200) DEFAULT NULL,
  `a_terminator_attenuator_id` varchar(12) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `charts_tmp`
--

DROP TABLE IF EXISTS `charts_tmp`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `charts_tmp` (
  `chart_name` varchar(150) NOT NULL,
  `chart_type` varchar(150) NOT NULL,
  `chart_title` varchar(150) DEFAULT NULL,
  `title_x` varchar(150) DEFAULT NULL,
  `title_y` varchar(150) DEFAULT NULL,
  `object_name` varchar(150) DEFAULT NULL,
  `number_option` decimal(10,0) DEFAULT NULL,
  `query_number` decimal(10,0) DEFAULT NULL,
  `chart_id` decimal(15,5) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `component`
--

DROP TABLE IF EXISTS `component`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `component` (
  `component_id` char(12) NOT NULL,
  `component_name` varchar(255) NOT NULL,
  `component_type` varchar(250) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `cond_effect_link`
--

DROP TABLE IF EXISTS `cond_effect_link`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cond_effect_link` (
  `cond_effect_link_id` char(12) NOT NULL,
  `condition_id` char(12) NOT NULL,
  `medium_id` char(12) NOT NULL,
  `effect` varchar(250) NOT NULL,
  `cond_effect_link_notes` varchar(2000) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `condition`
--

DROP TABLE IF EXISTS `condition`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `condition` (
  `condition_id` char(12) NOT NULL,
  `control_condition` varchar(2000) NOT NULL,
  `control_details` varchar(2000) DEFAULT NULL,
  `exp_condition` varchar(2000) NOT NULL,
  `exp_details` varchar(2000) DEFAULT NULL,
  `condition_global` varchar(2000) DEFAULT NULL,
  `condition_notes` varchar(2000) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `condition_object`
--

DROP TABLE IF EXISTS `condition_object`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `condition_object` (
  `cond_effect_link_id` char(12) NOT NULL,
  `object_id` char(12) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `condition_tmp`
--

DROP TABLE IF EXISTS `condition_tmp`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `condition_tmp` (
  `condition_id` char(12) DEFAULT NULL,
  `cond_effect_link_id` char(12) DEFAULT NULL,
  `condition_gene_name` varchar(200) DEFAULT NULL,
  `condition_gene_id` varchar(12) DEFAULT NULL,
  `condition_effect` varchar(10) DEFAULT NULL,
  `condition_promoter_name` varchar(200) DEFAULT NULL,
  `condition_promoter_id` varchar(12) DEFAULT NULL,
  `condition_final_state` varchar(200) DEFAULT NULL,
  `condition_conformation_id` varchar(12) DEFAULT NULL,
  `condition_site` varchar(200) DEFAULT NULL,
  `condition_evidence` varchar(200) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `conformation`
--

DROP TABLE IF EXISTS `conformation`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `conformation` (
  `conformation_id` char(12) NOT NULL,
  `transcription_factor_id` char(12) NOT NULL,
  `final_state` varchar(2000) DEFAULT NULL,
  `conformation_note` varchar(2000) DEFAULT NULL,
  `interaction_type` varchar(250) DEFAULT NULL,
  `conformation_internal_comment` longtext,
  `key_id_org` varchar(5) NOT NULL,
  `conformation_type` varchar(20) DEFAULT NULL,
  `apo_holo_conformation` varchar(10) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `conformation_effector_link`
--

DROP TABLE IF EXISTS `conformation_effector_link`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `conformation_effector_link` (
  `effector_id` char(12) NOT NULL,
  `conformation_id` char(12) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `effector`
--

DROP TABLE IF EXISTS `effector`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `effector` (
  `effector_id` char(12) NOT NULL,
  `effector_name` varchar(255) NOT NULL,
  `category` varchar(10) DEFAULT NULL,
  `effector_type` varchar(100) DEFAULT NULL,
  `effector_note` varchar(2000) DEFAULT NULL,
  `effector_internal_comment` longtext,
  `key_id_org` varchar(5) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `evidence`
--

DROP TABLE IF EXISTS `evidence`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `evidence` (
  `evidence_id` char(12) NOT NULL,
  `evidence_name` varchar(2000) NOT NULL,
  `type_object` varchar(200) DEFAULT NULL,
  `evidence_code` varchar(30) DEFAULT NULL,
  `evidence_note` varchar(2000) DEFAULT NULL,
  `evidence_internal_comment` longtext,
  `key_id_org` varchar(5) NOT NULL,
  `evidence_type` char(3) DEFAULT NULL,
  `evidence_category` varchar(200) DEFAULT NULL,
  `head` varchar(12) DEFAULT NULL,
  `example` varchar(500) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `external_db`
--

DROP TABLE IF EXISTS `external_db`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `external_db` (
  `external_db_id` char(12) NOT NULL,
  `external_db_name` varchar(255) NOT NULL,
  `external_db_description` varchar(255) DEFAULT NULL,
  `url` varchar(255) NOT NULL,
  `external_db_note` varchar(2000) DEFAULT NULL,
  `ext_db_internal_comment` longtext
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `functional_class`
--

DROP TABLE IF EXISTS `functional_class`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `functional_class` (
  `functional_class_id` char(12) NOT NULL,
  `fc_description` varchar(500) NOT NULL,
  `fc_label_index` varchar(50) NOT NULL,
  `head_class` char(12) DEFAULT NULL,
  `color_class` varchar(20) DEFAULT NULL,
  `fc_reference` varchar(255) NOT NULL,
  `fc_note` varchar(2000) DEFAULT NULL,
  `fc_internal_comment` longtext,
  `key_id_org` varchar(5) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `gene`
--

DROP TABLE IF EXISTS `gene`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `gene` (
  `gene_id` char(12) NOT NULL,
  `gene_name` varchar(255) DEFAULT NULL,
  `gene_posleft` decimal(10,0) DEFAULT NULL,
  `gene_posright` decimal(10,0) DEFAULT NULL,
  `gene_strand` varchar(10) DEFAULT NULL,
  `gene_sequence` longtext,
  `gc_content` decimal(15,10) DEFAULT NULL,
  `cri_score` decimal(15,10) DEFAULT NULL,
  `gene_note` varchar(2000) DEFAULT NULL,
  `gene_internal_comment` longtext,
  `key_id_org` varchar(5) NOT NULL,
  `gene_type` varchar(100) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `gene_head_fc`
--

DROP TABLE IF EXISTS `gene_head_fc`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `gene_head_fc` (
  `gene_id` char(12) DEFAULT NULL,
  `gene_name` varchar(255) DEFAULT NULL,
  `head_fc_id` char(12) DEFAULT NULL,
  `head_fc_description` varchar(500) DEFAULT NULL,
  `head_fc_label_index` varchar(50) DEFAULT NULL,
  `head_fc_reference` varchar(255) DEFAULT NULL,
  `child_fc_id` char(12) DEFAULT NULL,
  `child_fc_description` varchar(500) DEFAULT NULL,
  `child_fc_label_index` varchar(50) DEFAULT NULL,
  `child_fc_reference` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `gene_product_link`
--

DROP TABLE IF EXISTS `gene_product_link`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `gene_product_link` (
  `gene_id` char(12) NOT NULL,
  `product_id` char(12) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `generegulation_tmp`
--

DROP TABLE IF EXISTS `generegulation_tmp`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `generegulation_tmp` (
  `gene_id_regulator` char(12) DEFAULT NULL,
  `gene_name_regulator` varchar(255) DEFAULT NULL,
  `tf_id_regulator` char(12) DEFAULT NULL,
  `transcription_factor_name` varchar(255) DEFAULT NULL,
  `tf_conformation` varchar(2000) DEFAULT NULL,
  `conformation_status` varchar(5) DEFAULT NULL,
  `gene_id_regulated` char(12) DEFAULT NULL,
  `gene_name_regulated` varchar(255) DEFAULT NULL,
  `generegulation_function` varchar(9) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `gensorunit`
--

DROP TABLE IF EXISTS `gensorunit`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `gensorunit` (
  `gu_id` char(12) NOT NULL,
  `gu_name` varchar(255) NOT NULL,
  `gu_description` varchar(2000) DEFAULT NULL,
  `gu_map_file` varchar(250) NOT NULL,
  `gu_status` varchar(50) DEFAULT NULL,
  `effector_feedback_paths` varchar(2000) DEFAULT NULL,
  `proteincomplex_regulated_by_tf` varchar(100) DEFAULT NULL,
  `total_enzymes_withconnectivity` varchar(100) DEFAULT NULL,
  `enzymes_withconnectivity` varchar(2000) DEFAULT NULL,
  `note` longtext
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `groups`
--

DROP TABLE IF EXISTS `groups`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `groups` (
  `group_id` char(12) NOT NULL,
  `group_name` varchar(1000) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `gu_component_link`
--

DROP TABLE IF EXISTS `gu_component_link`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `gu_component_link` (
  `gu_id` char(12) NOT NULL,
  `component_id` char(12) NOT NULL,
  `component_function` varchar(50) NOT NULL,
  `note` longtext
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `gu_fc_link`
--

DROP TABLE IF EXISTS `gu_fc_link`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `gu_fc_link` (
  `gu_id` char(12) NOT NULL,
  `functional_class_id` char(12) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `gu_group_link`
--

DROP TABLE IF EXISTS `gu_group_link`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `gu_group_link` (
  `gu_id` char(12) NOT NULL,
  `group_id` char(12) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `gu_reaction_link`
--

DROP TABLE IF EXISTS `gu_reaction_link`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `gu_reaction_link` (
  `gu_id` char(12) NOT NULL,
  `reaction_id` char(12) NOT NULL,
  `reaction_number` varchar(50) NOT NULL,
  `reaction_order` decimal(10,0) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `interaction`
--

DROP TABLE IF EXISTS `interaction`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `interaction` (
  `interaction_id` varchar(12) NOT NULL,
  `regulator_id` varchar(12) DEFAULT NULL,
  `promoter_id` char(12) DEFAULT NULL,
  `site_id` char(12) DEFAULT NULL,
  `interaction_function` varchar(12) DEFAULT NULL,
  `center_position` decimal(20,2) DEFAULT NULL,
  `interaction_first_gene_id` varchar(12) DEFAULT NULL,
  `affinity_exp` decimal(20,5) DEFAULT NULL,
  `interaction_note` varchar(2000) DEFAULT NULL,
  `interaction_internal_comment` longtext,
  `interaction_sequence` varchar(100) DEFAULT NULL,
  `key_id_org` varchar(5) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `interaction_condition_link`
--

DROP TABLE IF EXISTS `interaction_condition_link`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `interaction_condition_link` (
  `interaction_id` varchar(12) NOT NULL,
  `condition_id` char(12) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `intermediate`
--

DROP TABLE IF EXISTS `intermediate`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `intermediate` (
  `intermediate_id` char(12) NOT NULL,
  `intermediate_name` varchar(255) NOT NULL,
  `intermediate_note` varchar(2000) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `matrix`
--

DROP TABLE IF EXISTS `matrix`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `matrix` (
  `tf_matrix_id` char(12) NOT NULL,
  `num_col` decimal(10,0) NOT NULL,
  `freq_a` decimal(10,0) NOT NULL,
  `freq_c` decimal(10,0) NOT NULL,
  `freq_g` decimal(10,0) NOT NULL,
  `freq_t` decimal(10,0) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `medium`
--

DROP TABLE IF EXISTS `medium`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `medium` (
  `medium_id` char(12) NOT NULL,
  `medium_name` varchar(300) NOT NULL,
  `medium_description` varchar(2000) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `method`
--

DROP TABLE IF EXISTS `method`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `method` (
  `method_id` char(12) NOT NULL,
  `method_name` varchar(200) NOT NULL,
  `method_description` varchar(2000) DEFAULT NULL,
  `parameter_used` varchar(2000) DEFAULT NULL,
  `method_note` varchar(2000) DEFAULT NULL,
  `method_internal_comment` longtext,
  `key_id_org` varchar(5) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `motif`
--

DROP TABLE IF EXISTS `motif`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `motif` (
  `motif_id` char(12) NOT NULL,
  `product_id` char(12) NOT NULL,
  `motif_posleft` decimal(10,0) NOT NULL,
  `motif_posright` decimal(10,0) NOT NULL,
  `motif_sequence` varchar(3000) DEFAULT NULL,
  `motif_description` varchar(4000) DEFAULT NULL,
  `motif_type` varchar(255) DEFAULT NULL,
  `motif_note` varchar(2000) DEFAULT NULL,
  `motif_internal_comment` longtext,
  `key_id_org` varchar(5) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `object_ev_method_pub_link`
--

DROP TABLE IF EXISTS `object_ev_method_pub_link`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `object_ev_method_pub_link` (
  `object_id` char(12) NOT NULL,
  `evidence_id` char(12) DEFAULT NULL,
  `method_id` char(12) DEFAULT NULL,
  `publication_id` char(12) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `object_external_db_link`
--

DROP TABLE IF EXISTS `object_external_db_link`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `object_external_db_link` (
  `object_id` char(12) NOT NULL,
  `external_db_id` char(12) NOT NULL,
  `ext_reference_id` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `object_synonym`
--

DROP TABLE IF EXISTS `object_synonym`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `object_synonym` (
  `object_id` char(12) NOT NULL,
  `object_synonym_name` varchar(255) NOT NULL,
  `os_internal_comment` longtext,
  `key_id_org` varchar(5) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `objects`
--

DROP TABLE IF EXISTS `objects`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `objects` (
  `object_id` decimal(10,0) DEFAULT NULL,
  `object_description` varchar(4000) DEFAULT NULL,
  `object_table_name` varchar(50) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `operon`
--

DROP TABLE IF EXISTS `operon`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `operon` (
  `operon_id` char(12) NOT NULL,
  `operon_name` varchar(255) NOT NULL,
  `firstgeneposleft` decimal(10,0) NOT NULL,
  `lastgeneposright` decimal(10,0) NOT NULL,
  `regulationposleft` decimal(10,0) NOT NULL,
  `regulationposright` decimal(10,0) NOT NULL,
  `operon_strand` varchar(10) DEFAULT NULL,
  `operon_internal_comment` longtext,
  `key_id_org` varchar(5) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `operon_d_tmp`
--

DROP TABLE IF EXISTS `operon_d_tmp`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `operon_d_tmp` (
  `op_id` decimal(10,0) NOT NULL,
  `operon_id` char(12) DEFAULT NULL,
  `operon_name` varchar(255) DEFAULT NULL,
  `operon_tu_group` decimal(10,0) DEFAULT NULL,
  `operon_gene_group` decimal(10,0) DEFAULT NULL,
  `operon_sf_group` decimal(10,0) DEFAULT NULL,
  `operon_site_group` decimal(10,0) DEFAULT NULL,
  `operon_promoter_group` decimal(10,0) DEFAULT NULL,
  `operon_tf_group` decimal(10,0) DEFAULT NULL,
  `operon_terminator_group` decimal(10,0) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `organism`
--

DROP TABLE IF EXISTS `organism`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `organism` (
  `organism_id` char(12) NOT NULL,
  `organism_name` varchar(1000) NOT NULL,
  `organism_description` varchar(2000) DEFAULT NULL,
  `organism_note` varchar(2000) DEFAULT NULL,
  `organism_internal_comment` longtext
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `product`
--

DROP TABLE IF EXISTS `product`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `product` (
  `product_id` char(12) NOT NULL,
  `product_name` varchar(255) DEFAULT NULL,
  `molecular_weigth` decimal(20,5) DEFAULT NULL,
  `isoelectric_point` decimal(20,10) DEFAULT NULL,
  `location` varchar(1000) DEFAULT NULL,
  `anticodon` varchar(10) DEFAULT NULL,
  `product_sequence` varchar(4000) DEFAULT NULL,
  `product_note` longtext,
  `product_internal_comment` longtext,
  `key_id_org` varchar(5) NOT NULL,
  `product_type` varchar(25) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `product_fc_link`
--

DROP TABLE IF EXISTS `product_fc_link`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `product_fc_link` (
  `product_id` char(12) NOT NULL,
  `functional_class_id` char(12) NOT NULL,
  `prod_fc_l_id` char(12) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `product_tf_link`
--

DROP TABLE IF EXISTS `product_tf_link`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `product_tf_link` (
  `transcription_factor_id` char(12) NOT NULL,
  `product_id` char(12) NOT NULL,
  `compon_coefficient` decimal(10,0) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `promoter`
--

DROP TABLE IF EXISTS `promoter`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `promoter` (
  `promoter_id` char(12) NOT NULL,
  `promoter_name` varchar(255) DEFAULT NULL,
  `promoter_strand` varchar(10) DEFAULT NULL,
  `pos_1` decimal(10,0) DEFAULT NULL,
  `sigma_factor` varchar(80) DEFAULT NULL,
  `basal_trans_val` decimal(20,5) DEFAULT NULL,
  `equilibrium_const` decimal(20,5) DEFAULT NULL,
  `kinetic_const` decimal(20,5) DEFAULT NULL,
  `strength_seq` decimal(20,5) DEFAULT NULL,
  `promoter_sequence` varchar(200) DEFAULT NULL,
  `promoter_note` varchar(4000) DEFAULT NULL,
  `promoter_internal_comment` longtext,
  `key_id_org` varchar(5) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `promoter_feature`
--

DROP TABLE IF EXISTS `promoter_feature`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `promoter_feature` (
  `promoter_feature_id` char(12) DEFAULT NULL,
  `promoter_id` char(12) DEFAULT NULL,
  `box_10_left` decimal(10,0) DEFAULT NULL,
  `box_10_right` decimal(10,0) DEFAULT NULL,
  `box_35_left` decimal(10,0) DEFAULT NULL,
  `box_35_right` decimal(10,0) DEFAULT NULL,
  `box_10_sequence` varchar(100) DEFAULT NULL,
  `box_35_sequence` varchar(100) DEFAULT NULL,
  `score` decimal(6,2) DEFAULT NULL,
  `relative_box_10_left` decimal(10,0) DEFAULT NULL,
  `relative_box_10_right` decimal(10,0) DEFAULT NULL,
  `relative_box_35_left` decimal(10,0) DEFAULT NULL,
  `relative_box_35_right` decimal(10,0) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `publication`
--

DROP TABLE IF EXISTS `publication`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `publication` (
  `publication_id` char(12) NOT NULL,
  `reference_id` varchar(255) NOT NULL,
  `external_db_id` char(12) NOT NULL,
  `author` varchar(2000) DEFAULT NULL,
  `title` varchar(2000) DEFAULT NULL,
  `source` varchar(2000) DEFAULT NULL,
  `years` varchar(50) DEFAULT NULL,
  `publication_note` varchar(2000) DEFAULT NULL,
  `publication_internal_comment` longtext
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `reaction`
--

DROP TABLE IF EXISTS `reaction`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `reaction` (
  `reaction_id` char(12) NOT NULL,
  `reaction_name` varchar(1000) NOT NULL,
  `reaction_description` varchar(2000) DEFAULT NULL,
  `reaction_type` varchar(250) NOT NULL,
  `note` longtext
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `reaction_component_link`
--

DROP TABLE IF EXISTS `reaction_component_link`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `reaction_component_link` (
  `reaction_component_id` char(12) NOT NULL,
  `reaction_id` char(12) NOT NULL,
  `component_id` char(12) NOT NULL,
  `role` varchar(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `reg_phrase`
--

DROP TABLE IF EXISTS `reg_phrase`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `reg_phrase` (
  `reg_phrase_id` char(12) NOT NULL,
  `reg_phrase_description` varchar(255) DEFAULT NULL,
  `regulation_ratio` varchar(20) DEFAULT NULL,
  `on_half_life` decimal(20,5) DEFAULT NULL,
  `off_half_life` decimal(20,5) DEFAULT NULL,
  `phrase` varchar(2000) NOT NULL,
  `reg_phrase_function` varchar(25) DEFAULT NULL,
  `reg_phrase_note` varchar(2000) DEFAULT NULL,
  `reg_phrase_internal_comment` longtext,
  `key_id_org` varchar(5) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `reg_phrase_ri_link`
--

DROP TABLE IF EXISTS `reg_phrase_ri_link`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `reg_phrase_ri_link` (
  `reg_phrase_id` char(12) NOT NULL,
  `regulatory_interaction_id` char(12) NOT NULL,
  `type` varchar(20) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `regulator`
--

DROP TABLE IF EXISTS `regulator`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `regulator` (
  `regulator_id` varchar(12) NOT NULL,
  `product_id` char(12) DEFAULT NULL,
  `regulator_name` varchar(250) DEFAULT NULL,
  `regulator_internal_commnet` varchar(2000) DEFAULT NULL,
  `regulator_note` longtext,
  `key_id_org` varchar(5) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `regulatory_interaction`
--

DROP TABLE IF EXISTS `regulatory_interaction`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `regulatory_interaction` (
  `regulatory_interaction_id` char(12) NOT NULL,
  `conformation_id` char(12) NOT NULL,
  `promoter_id` char(12) DEFAULT NULL,
  `site_id` char(12) NOT NULL,
  `ri_function` varchar(9) DEFAULT NULL,
  `center_position` decimal(20,2) DEFAULT NULL,
  `ri_dist_first_gene` decimal(20,2) DEFAULT NULL,
  `ri_first_gene_id` char(12) DEFAULT NULL,
  `affinity_exp` decimal(20,5) DEFAULT NULL,
  `regulatory_interaction_note` varchar(2000) DEFAULT NULL,
  `ri_internal_comment` longtext,
  `key_id_org` varchar(5) NOT NULL,
  `ri_sequence` varchar(100) DEFAULT NULL,
  `ri_orientation` varchar(35) DEFAULT NULL,
  `ri_sequence_orientation` varchar(100) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `regulon_d_tmp`
--

DROP TABLE IF EXISTS `regulon_d_tmp`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `regulon_d_tmp` (
  `re_id` decimal(10,0) NOT NULL,
  `regulon_id` char(12) NOT NULL,
  `regulon_name` varchar(500) DEFAULT NULL,
  `regulon_key_id_org` char(5) NOT NULL,
  `regulon_tf_group` decimal(10,0) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `regulon_tmp`
--

DROP TABLE IF EXISTS `regulon_tmp`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `regulon_tmp` (
  `regulon_id` char(12) NOT NULL,
  `regulon_name` varchar(500) NOT NULL,
  `key_id_org` varchar(5) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `repeat`
--

DROP TABLE IF EXISTS `repeat`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `repeat` (
  `repeat_id` char(12) NOT NULL,
  `repeat_posleft` decimal(10,0) NOT NULL,
  `repeat_posright` decimal(10,0) NOT NULL,
  `repeat_family` varchar(255) DEFAULT NULL,
  `repeat_note` varchar(2000) DEFAULT NULL,
  `repeat_internal_comment` longtext,
  `key_id_org` varchar(5) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `rfam`
--

DROP TABLE IF EXISTS `rfam`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `rfam` (
  `rfam_id` varchar(12) NOT NULL,
  `gene_id` char(12) DEFAULT NULL,
  `rfam_type` varchar(100) DEFAULT NULL,
  `rfam_description` varchar(2000) DEFAULT NULL,
  `rfam_score` decimal(10,5) DEFAULT NULL,
  `rfam_strand` varchar(12) DEFAULT NULL,
  `rfam_posleft` decimal(10,0) DEFAULT NULL,
  `rfam_posright` decimal(10,0) DEFAULT NULL,
  `rfam_sequence` varchar(1000) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `shine_dalgarno`
--

DROP TABLE IF EXISTS `shine_dalgarno`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `shine_dalgarno` (
  `shine_dalgarno_id` char(12) NOT NULL,
  `gene_id` char(12) NOT NULL,
  `shine_dalgarno_dist_gene` decimal(10,0) NOT NULL,
  `shine_dalgarno_posleft` decimal(10,0) DEFAULT NULL,
  `shine_dalgarno_posright` decimal(10,0) DEFAULT NULL,
  `shine_dalgarno_sequence` varchar(500) DEFAULT NULL,
  `shine_dalgarno_note` varchar(2000) DEFAULT NULL,
  `sd_internal_comment` longtext,
  `key_id_org` varchar(5) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `sigma_tmp`
--

DROP TABLE IF EXISTS `sigma_tmp`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `sigma_tmp` (
  `sigma_id` varchar(12) NOT NULL,
  `sigma_name` varchar(50) NOT NULL,
  `sigma_synonyms` varchar(50) DEFAULT NULL,
  `sigma_gene_id` varchar(12) DEFAULT NULL,
  `sigma_gene_name` varchar(250) DEFAULT NULL,
  `sigma_coregulators` varchar(2000) DEFAULT NULL,
  `sigma_notes` varchar(4000) DEFAULT NULL,
  `sigma_sigmulon_genes` varchar(4000) DEFAULT NULL,
  `key_id_org` varchar(5) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `site`
--

DROP TABLE IF EXISTS `site`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `site` (
  `site_id` char(12) NOT NULL,
  `site_posleft` decimal(10,0) NOT NULL,
  `site_posright` decimal(10,0) NOT NULL,
  `site_sequence` varchar(100) DEFAULT NULL,
  `site_note` varchar(2000) DEFAULT NULL,
  `site_internal_comment` longtext,
  `key_id_org` varchar(5) NOT NULL,
  `site_length` decimal(10,0) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `srna_interaction`
--

DROP TABLE IF EXISTS `srna_interaction`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `srna_interaction` (
  `srna_id` char(12) NOT NULL,
  `srna_gene_id` char(12) DEFAULT NULL,
  `srna_gene_regulated_id` char(12) DEFAULT NULL,
  `srna_tu_regulated_id` char(12) DEFAULT NULL,
  `srna_function` varchar(2000) DEFAULT NULL,
  `srna_posleft` decimal(10,0) DEFAULT NULL,
  `srna_posright` decimal(10,0) DEFAULT NULL,
  `srna_sequence` varchar(2000) DEFAULT NULL,
  `srna_regulation_type` varchar(2000) DEFAULT NULL,
  `srna_mechanis` varchar(1000) DEFAULT NULL,
  `srna_note` varchar(1000) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `strand_d_tmp`
--

DROP TABLE IF EXISTS `strand_d_tmp`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `strand_d_tmp` (
  `st_id` decimal(10,0) NOT NULL,
  `strand_name` varchar(10) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `t_factor_d_tmp`
--

DROP TABLE IF EXISTS `t_factor_d_tmp`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `t_factor_d_tmp` (
  `tf_id` decimal(10,0) NOT NULL,
  `t_factor_id` char(12) NOT NULL,
  `t_factor_name` varchar(255) DEFAULT NULL,
  `t_factor_site_length` decimal(10,0) DEFAULT NULL,
  `t_factor_key_id_org` char(5) NOT NULL,
  `t_factor_site_group` decimal(10,0) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `terminator`
--

DROP TABLE IF EXISTS `terminator`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `terminator` (
  `terminator_id` char(12) NOT NULL,
  `terminator_dist_gene` decimal(10,0) DEFAULT NULL,
  `terminator_posleft` decimal(10,0) DEFAULT NULL,
  `terminator_posright` decimal(10,0) DEFAULT NULL,
  `terminator_class` varchar(30) DEFAULT NULL,
  `terminator_sequence` varchar(200) DEFAULT NULL,
  `terminator_note` varchar(2000) DEFAULT NULL,
  `terminator_internal_comment` longtext,
  `key_id_org` varchar(5) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tf_alignment`
--

DROP TABLE IF EXISTS `tf_alignment`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tf_alignment` (
  `tf_alignment_id` char(12) NOT NULL,
  `transcription_factor_id` char(12) NOT NULL,
  `tf_matrix_id` char(12) DEFAULT NULL,
  `tf_alignment_name` varchar(255) NOT NULL,
  `tf_alignment_note` varchar(2000) DEFAULT NULL,
  `tf_alignment_internal_comment` longtext,
  `key_id_org` varchar(5) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tf_matrix`
--

DROP TABLE IF EXISTS `tf_matrix`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tf_matrix` (
  `tf_matrix_id` char(12) NOT NULL,
  `transcription_factor_id` char(12) NOT NULL,
  `tf_matrix_name` varchar(255) DEFAULT NULL,
  `media` decimal(5,2) NOT NULL,
  `standar_desv` decimal(8,3) NOT NULL,
  `score_low` decimal(5,2) NOT NULL,
  `score_high` decimal(5,2) DEFAULT NULL,
  `tf_matrix_note` varchar(2000) DEFAULT NULL,
  `tf_matrix_internal_comment` longtext,
  `key_id_org` varchar(5) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tf_matrix_align_link`
--

DROP TABLE IF EXISTS `tf_matrix_align_link`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tf_matrix_align_link` (
  `tf_matrix_id` char(12) NOT NULL,
  `tf_alignment_id` char(12) DEFAULT NULL,
  `transcription_factor_id` char(12) NOT NULL,
  `tf_matrix_name` varchar(255) DEFAULT NULL,
  `media` decimal(5,2) NOT NULL,
  `standar_desv` decimal(8,3) NOT NULL,
  `score_low` decimal(5,2) NOT NULL,
  `score_high` decimal(5,2) DEFAULT NULL,
  `tf_matrix_note` varchar(2000) DEFAULT NULL,
  `tf_matrix_internal_comment` longtext,
  `key_id_org` varchar(5) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `transcription_factor`
--

DROP TABLE IF EXISTS `transcription_factor`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `transcription_factor` (
  `transcription_factor_id` char(12) NOT NULL,
  `transcription_factor_name` varchar(255) NOT NULL,
  `site_length` decimal(10,0) DEFAULT NULL,
  `symmetry` varchar(50) DEFAULT NULL,
  `transcription_factor_family` varchar(250) DEFAULT NULL,
  `tf_internal_comment` longtext,
  `key_id_org` varchar(5) NOT NULL,
  `transcription_factor_note` longtext,
  `connectivity_class` varchar(100) DEFAULT NULL,
  `sensing_class` varchar(100) DEFAULT NULL,
  `consensus_sequence` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `transcription_unit`
--

DROP TABLE IF EXISTS `transcription_unit`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `transcription_unit` (
  `transcription_unit_id` char(12) NOT NULL,
  `promoter_id` char(12) DEFAULT NULL,
  `transcription_unit_name` varchar(255) DEFAULT NULL,
  `operon_id` char(12) DEFAULT NULL,
  `transcription_unit_note` varchar(4000) DEFAULT NULL,
  `tu_internal_comment` longtext,
  `key_id_org` varchar(5) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tu_gene_link`
--

DROP TABLE IF EXISTS `tu_gene_link`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tu_gene_link` (
  `transcription_unit_id` char(12) NOT NULL,
  `gene_id` char(12) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tu_terminator_link`
--

DROP TABLE IF EXISTS `tu_terminator_link`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `tu_terminator_link` (
  `transcription_unit_id` char(12) NOT NULL,
  `terminator_id` char(12) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `version`
--

DROP TABLE IF EXISTS `version`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `version` (
  `version_regulon` varchar(10) NOT NULL,
  `version_ecocyc` varchar(100) NOT NULL,
  `version_date_time` varchar(100) NOT NULL,
  `version_internal_comment` varchar(4000) DEFAULT NULL,
  `head_citation` varchar(4000) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping events for database 'regulondb_93'
--

--
-- Dumping routines for database 'regulondb_93'
--
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2017-03-29 23:21:44
