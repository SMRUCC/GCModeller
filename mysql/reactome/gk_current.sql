CREATE DATABASE  IF NOT EXISTS `gk_current` /*!40100 DEFAULT CHARACTER SET utf8 */;
USE `gk_current`;
-- MySQL dump 10.13  Distrib 5.7.9, for Win64 (x86_64)
--
-- Host: localhost    Database: gk_current
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
-- Table structure for table `_attributevaluebeforechange`
--

DROP TABLE IF EXISTS `_attributevaluebeforechange`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `_attributevaluebeforechange` (
  `DB_ID` int(10) unsigned NOT NULL,
  `changedAttribute` text,
  PRIMARY KEY (`DB_ID`),
  FULLTEXT KEY `changedAttribute` (`changedAttribute`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `_attributevaluebeforechange_2_previousvalue`
--

DROP TABLE IF EXISTS `_attributevaluebeforechange_2_previousvalue`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `_attributevaluebeforechange_2_previousvalue` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `previousValue_rank` int(10) unsigned DEFAULT NULL,
  `previousValue` text,
  KEY `DB_ID` (`DB_ID`),
  FULLTEXT KEY `previousValue` (`previousValue`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `_deleted`
--

DROP TABLE IF EXISTS `_deleted`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `_deleted` (
  `DB_ID` int(10) unsigned NOT NULL,
  `curatorComment` text,
  `reason` int(10) unsigned DEFAULT NULL,
  `reason_class` varchar(64) DEFAULT NULL,
  PRIMARY KEY (`DB_ID`),
  KEY `reason` (`reason`),
  FULLTEXT KEY `curatorComment` (`curatorComment`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `_deleted_2_deletedinstancedb_id`
--

DROP TABLE IF EXISTS `_deleted_2_deletedinstancedb_id`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `_deleted_2_deletedinstancedb_id` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `deletedInstanceDB_ID_rank` int(10) unsigned DEFAULT NULL,
  `deletedInstanceDB_ID` int(10) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `deletedInstanceDB_ID` (`deletedInstanceDB_ID`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `_deleted_2_replacementinstances`
--

DROP TABLE IF EXISTS `_deleted_2_replacementinstances`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `_deleted_2_replacementinstances` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `replacementInstances_rank` int(10) unsigned DEFAULT NULL,
  `replacementInstances` int(10) unsigned DEFAULT NULL,
  `replacementInstances_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `replacementInstances` (`replacementInstances`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `_instancebeforechange`
--

DROP TABLE IF EXISTS `_instancebeforechange`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `_instancebeforechange` (
  `DB_ID` int(10) unsigned NOT NULL,
  `changedInstanceDB_ID` int(10) DEFAULT NULL,
  `instanceEdit` int(10) unsigned DEFAULT NULL,
  `instanceEdit_class` varchar(64) DEFAULT NULL,
  PRIMARY KEY (`DB_ID`),
  KEY `changedInstanceDB_ID` (`changedInstanceDB_ID`),
  KEY `instanceEdit` (`instanceEdit`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `_instancebeforechange_2_attributevaluesbeforechange`
--

DROP TABLE IF EXISTS `_instancebeforechange_2_attributevaluesbeforechange`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `_instancebeforechange_2_attributevaluesbeforechange` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `attributeValuesBeforeChange_rank` int(10) unsigned DEFAULT NULL,
  `attributeValuesBeforeChange` int(10) unsigned DEFAULT NULL,
  `attributeValuesBeforeChange_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `attributeValuesBeforeChange` (`attributeValuesBeforeChange`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `_release`
--

DROP TABLE IF EXISTS `_release`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `_release` (
  `DB_ID` int(10) unsigned NOT NULL,
  `releaseDate` text,
  `releaseNumber` int(10) DEFAULT NULL,
  PRIMARY KEY (`DB_ID`),
  KEY `releaseNumber` (`releaseNumber`),
  FULLTEXT KEY `releaseDate` (`releaseDate`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `abstractmodifiedresidue`
--

DROP TABLE IF EXISTS `abstractmodifiedresidue`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `abstractmodifiedresidue` (
  `DB_ID` int(10) unsigned NOT NULL,
  `referenceSequence` int(10) unsigned DEFAULT NULL,
  `referenceSequence_class` varchar(64) DEFAULT NULL,
  PRIMARY KEY (`DB_ID`),
  KEY `referenceSequence` (`referenceSequence`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `affiliation`
--

DROP TABLE IF EXISTS `affiliation`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `affiliation` (
  `DB_ID` int(10) unsigned NOT NULL,
  `address` text,
  PRIMARY KEY (`DB_ID`),
  FULLTEXT KEY `address` (`address`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `affiliation_2_name`
--

DROP TABLE IF EXISTS `affiliation_2_name`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `affiliation_2_name` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `name_rank` int(10) unsigned DEFAULT NULL,
  `name` text,
  KEY `DB_ID` (`DB_ID`),
  FULLTEXT KEY `name` (`name`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `blackboxevent`
--

DROP TABLE IF EXISTS `blackboxevent`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `blackboxevent` (
  `DB_ID` int(10) unsigned NOT NULL,
  `templateEvent` int(10) unsigned DEFAULT NULL,
  `templateEvent_class` varchar(64) DEFAULT NULL,
  PRIMARY KEY (`DB_ID`),
  KEY `templateEvent` (`templateEvent`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `blackboxevent_2_hasevent`
--

DROP TABLE IF EXISTS `blackboxevent_2_hasevent`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `blackboxevent_2_hasevent` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `hasEvent_rank` int(10) unsigned DEFAULT NULL,
  `hasEvent` int(10) unsigned DEFAULT NULL,
  `hasEvent_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `hasEvent` (`hasEvent`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `book`
--

DROP TABLE IF EXISTS `book`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `book` (
  `DB_ID` int(10) unsigned NOT NULL,
  `ISBN` text,
  `chapterTitle` text,
  `pages` text,
  `publisher` int(10) unsigned DEFAULT NULL,
  `publisher_class` varchar(64) DEFAULT NULL,
  `year` int(10) DEFAULT NULL,
  PRIMARY KEY (`DB_ID`),
  KEY `publisher` (`publisher`),
  KEY `year` (`year`),
  FULLTEXT KEY `ISBN` (`ISBN`),
  FULLTEXT KEY `chapterTitle` (`chapterTitle`),
  FULLTEXT KEY `pages` (`pages`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `book_2_chapterauthors`
--

DROP TABLE IF EXISTS `book_2_chapterauthors`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `book_2_chapterauthors` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `chapterAuthors_rank` int(10) unsigned DEFAULT NULL,
  `chapterAuthors` int(10) unsigned DEFAULT NULL,
  `chapterAuthors_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `chapterAuthors` (`chapterAuthors`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `candidateset`
--

DROP TABLE IF EXISTS `candidateset`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `candidateset` (
  `DB_ID` int(10) unsigned NOT NULL,
  PRIMARY KEY (`DB_ID`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `candidateset_2_hascandidate`
--

DROP TABLE IF EXISTS `candidateset_2_hascandidate`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `candidateset_2_hascandidate` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `hasCandidate_rank` int(10) unsigned DEFAULT NULL,
  `hasCandidate` int(10) unsigned DEFAULT NULL,
  `hasCandidate_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `hasCandidate` (`hasCandidate`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `catalystactivity`
--

DROP TABLE IF EXISTS `catalystactivity`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `catalystactivity` (
  `DB_ID` int(10) unsigned NOT NULL,
  `activity` int(10) unsigned DEFAULT NULL,
  `activity_class` varchar(64) DEFAULT NULL,
  `physicalEntity` int(10) unsigned DEFAULT NULL,
  `physicalEntity_class` varchar(64) DEFAULT NULL,
  PRIMARY KEY (`DB_ID`),
  KEY `activity` (`activity`),
  KEY `physicalEntity` (`physicalEntity`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `catalystactivity_2_activeunit`
--

DROP TABLE IF EXISTS `catalystactivity_2_activeunit`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `catalystactivity_2_activeunit` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `activeUnit_rank` int(10) unsigned DEFAULT NULL,
  `activeUnit` int(10) unsigned DEFAULT NULL,
  `activeUnit_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `activeUnit` (`activeUnit`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `catalystactivity_2_literaturereference`
--

DROP TABLE IF EXISTS `catalystactivity_2_literaturereference`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `catalystactivity_2_literaturereference` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `literatureReference_rank` int(10) unsigned DEFAULT NULL,
  `literatureReference` int(10) unsigned DEFAULT NULL,
  `literatureReference_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `literatureReference` (`literatureReference`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `celltype`
--

DROP TABLE IF EXISTS `celltype`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `celltype` (
  `DB_ID` int(10) unsigned NOT NULL,
  PRIMARY KEY (`DB_ID`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `compartment`
--

DROP TABLE IF EXISTS `compartment`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `compartment` (
  `DB_ID` int(10) unsigned NOT NULL,
  PRIMARY KEY (`DB_ID`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `complex`
--

DROP TABLE IF EXISTS `complex`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `complex` (
  `DB_ID` int(10) unsigned NOT NULL,
  `isChimeric` enum('TRUE','FALSE') DEFAULT NULL,
  `totalProt` text,
  `maxHomologues` text,
  `inferredProt` text,
  PRIMARY KEY (`DB_ID`),
  KEY `isChimeric` (`isChimeric`),
  FULLTEXT KEY `totalProt` (`totalProt`),
  FULLTEXT KEY `maxHomologues` (`maxHomologues`),
  FULLTEXT KEY `inferredProt` (`inferredProt`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `complex_2_entityonothercell`
--

DROP TABLE IF EXISTS `complex_2_entityonothercell`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `complex_2_entityonothercell` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `entityOnOtherCell_rank` int(10) unsigned DEFAULT NULL,
  `entityOnOtherCell` int(10) unsigned DEFAULT NULL,
  `entityOnOtherCell_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `entityOnOtherCell` (`entityOnOtherCell`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `complex_2_hascomponent`
--

DROP TABLE IF EXISTS `complex_2_hascomponent`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `complex_2_hascomponent` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `hasComponent_rank` int(10) unsigned DEFAULT NULL,
  `hasComponent` int(10) unsigned DEFAULT NULL,
  `hasComponent_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `hasComponent` (`hasComponent`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `complex_2_includedlocation`
--

DROP TABLE IF EXISTS `complex_2_includedlocation`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `complex_2_includedlocation` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `includedLocation_rank` int(10) unsigned DEFAULT NULL,
  `includedLocation` int(10) unsigned DEFAULT NULL,
  `includedLocation_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `includedLocation` (`includedLocation`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `complex_2_interactionidentifier`
--

DROP TABLE IF EXISTS `complex_2_interactionidentifier`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `complex_2_interactionidentifier` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `interactionIdentifier_rank` int(10) unsigned DEFAULT NULL,
  `interactionIdentifier` int(10) unsigned DEFAULT NULL,
  `interactionIdentifier_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `interactionIdentifier` (`interactionIdentifier`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `complex_2_species`
--

DROP TABLE IF EXISTS `complex_2_species`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `complex_2_species` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `species_rank` int(10) unsigned DEFAULT NULL,
  `species` int(10) unsigned DEFAULT NULL,
  `species_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `species` (`species`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `concurrenteventset`
--

DROP TABLE IF EXISTS `concurrenteventset`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `concurrenteventset` (
  `DB_ID` int(10) unsigned NOT NULL,
  PRIMARY KEY (`DB_ID`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `concurrenteventset_2_concurrentevents`
--

DROP TABLE IF EXISTS `concurrenteventset_2_concurrentevents`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `concurrenteventset_2_concurrentevents` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `concurrentEvents_rank` int(10) unsigned DEFAULT NULL,
  `concurrentEvents` int(10) unsigned DEFAULT NULL,
  `concurrentEvents_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `concurrentEvents` (`concurrentEvents`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `concurrenteventset_2_focusentity`
--

DROP TABLE IF EXISTS `concurrenteventset_2_focusentity`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `concurrenteventset_2_focusentity` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `focusEntity_rank` int(10) unsigned DEFAULT NULL,
  `focusEntity` int(10) unsigned DEFAULT NULL,
  `focusEntity_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `focusEntity` (`focusEntity`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `controlledvocabulary`
--

DROP TABLE IF EXISTS `controlledvocabulary`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `controlledvocabulary` (
  `DB_ID` int(10) unsigned NOT NULL,
  `definition` text,
  PRIMARY KEY (`DB_ID`),
  FULLTEXT KEY `definition` (`definition`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `controlledvocabulary_2_name`
--

DROP TABLE IF EXISTS `controlledvocabulary_2_name`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `controlledvocabulary_2_name` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `name_rank` int(10) unsigned DEFAULT NULL,
  `name` text,
  KEY `DB_ID` (`DB_ID`),
  FULLTEXT KEY `name` (`name`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `crosslinkedresidue`
--

DROP TABLE IF EXISTS `crosslinkedresidue`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `crosslinkedresidue` (
  `DB_ID` int(10) unsigned NOT NULL,
  `modification` int(10) unsigned DEFAULT NULL,
  `modification_class` varchar(64) DEFAULT NULL,
  PRIMARY KEY (`DB_ID`),
  KEY `modification` (`modification`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `crosslinkedresidue_2_secondcoordinate`
--

DROP TABLE IF EXISTS `crosslinkedresidue_2_secondcoordinate`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `crosslinkedresidue_2_secondcoordinate` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `secondCoordinate_rank` int(10) unsigned DEFAULT NULL,
  `secondCoordinate` int(10) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `secondCoordinate` (`secondCoordinate`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `databaseidentifier`
--

DROP TABLE IF EXISTS `databaseidentifier`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `databaseidentifier` (
  `DB_ID` int(10) unsigned NOT NULL,
  `identifier` varchar(20) DEFAULT NULL,
  `referenceDatabase` int(10) unsigned DEFAULT NULL,
  `referenceDatabase_class` varchar(64) DEFAULT NULL,
  PRIMARY KEY (`DB_ID`),
  KEY `identifier` (`identifier`),
  KEY `referenceDatabase` (`referenceDatabase`),
  FULLTEXT KEY `identifier_fulltext` (`identifier`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `databaseidentifier_2_crossreference`
--

DROP TABLE IF EXISTS `databaseidentifier_2_crossreference`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `databaseidentifier_2_crossreference` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `crossReference_rank` int(10) unsigned DEFAULT NULL,
  `crossReference` int(10) unsigned DEFAULT NULL,
  `crossReference_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `crossReference` (`crossReference`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `databaseobject`
--

DROP TABLE IF EXISTS `databaseobject`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `databaseobject` (
  `DB_ID` int(10) NOT NULL AUTO_INCREMENT,
  `_Protege_id` varchar(255) DEFAULT NULL,
  `__is_ghost` enum('TRUE') DEFAULT NULL,
  `_class` varchar(64) DEFAULT NULL,
  `_displayName` text,
  `_timestamp` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `created` int(10) unsigned DEFAULT NULL,
  `created_class` varchar(64) DEFAULT NULL,
  `stableIdentifier` int(10) unsigned DEFAULT NULL,
  `stableIdentifier_class` varchar(64) DEFAULT NULL,
  PRIMARY KEY (`DB_ID`),
  KEY `_Protege_id` (`_Protege_id`),
  KEY `__is_ghost` (`__is_ghost`),
  KEY `_class` (`_class`),
  KEY `_timestamp` (`_timestamp`),
  KEY `created` (`created`),
  KEY `stableIdentifier` (`stableIdentifier`),
  FULLTEXT KEY `_Protege_id_fulltext` (`_Protege_id`),
  FULLTEXT KEY `_class_fulltext` (`_class`),
  FULLTEXT KEY `_displayName` (`_displayName`)
) ENGINE=MyISAM AUTO_INCREMENT=8835475 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `databaseobject_2_modified`
--

DROP TABLE IF EXISTS `databaseobject_2_modified`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `databaseobject_2_modified` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `modified_rank` int(10) unsigned DEFAULT NULL,
  `modified` int(10) unsigned DEFAULT NULL,
  `modified_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `modified` (`modified`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `datamodel`
--

DROP TABLE IF EXISTS `datamodel`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `datamodel` (
  `thing` varchar(255) NOT NULL,
  `thing_class` enum('SchemaClass','SchemaClassAttribute','Schema') DEFAULT NULL,
  `property_name` varchar(255) NOT NULL,
  `property_value` text,
  `property_value_type` enum('INTEGER','SYMBOL','STRING','INSTANCE','SchemaClass','SchemaClassAttribute') DEFAULT NULL,
  `property_value_rank` int(10) unsigned DEFAULT '0'
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `definedset`
--

DROP TABLE IF EXISTS `definedset`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `definedset` (
  `DB_ID` int(10) unsigned NOT NULL,
  PRIMARY KEY (`DB_ID`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `deletedcontrolledvocabulary`
--

DROP TABLE IF EXISTS `deletedcontrolledvocabulary`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `deletedcontrolledvocabulary` (
  `DB_ID` int(10) unsigned NOT NULL,
  PRIMARY KEY (`DB_ID`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `depolymerisation`
--

DROP TABLE IF EXISTS `depolymerisation`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `depolymerisation` (
  `DB_ID` int(10) unsigned NOT NULL,
  PRIMARY KEY (`DB_ID`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `disease`
--

DROP TABLE IF EXISTS `disease`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `disease` (
  `DB_ID` int(10) unsigned NOT NULL,
  PRIMARY KEY (`DB_ID`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `edge`
--

DROP TABLE IF EXISTS `edge`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `edge` (
  `DB_ID` int(10) unsigned NOT NULL,
  `edgeType` int(10) DEFAULT NULL,
  `pathwayDiagram` int(10) unsigned DEFAULT NULL,
  `pathwayDiagram_class` varchar(64) DEFAULT NULL,
  `pointCoordinates` text,
  `sourceVertex` int(10) unsigned DEFAULT NULL,
  `sourceVertex_class` varchar(64) DEFAULT NULL,
  `targetVertex` int(10) unsigned DEFAULT NULL,
  `targetVertex_class` varchar(64) DEFAULT NULL,
  PRIMARY KEY (`DB_ID`),
  KEY `edgeType` (`edgeType`),
  KEY `pathwayDiagram` (`pathwayDiagram`),
  KEY `sourceVertex` (`sourceVertex`),
  KEY `targetVertex` (`targetVertex`),
  FULLTEXT KEY `pointCoordinates` (`pointCoordinates`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `entitycompartment`
--

DROP TABLE IF EXISTS `entitycompartment`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `entitycompartment` (
  `DB_ID` int(10) unsigned NOT NULL,
  PRIMARY KEY (`DB_ID`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `entityfunctionalstatus`
--

DROP TABLE IF EXISTS `entityfunctionalstatus`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `entityfunctionalstatus` (
  `DB_ID` int(10) unsigned NOT NULL,
  `physicalEntity` int(10) unsigned DEFAULT NULL,
  `physicalEntity_class` varchar(64) DEFAULT NULL,
  PRIMARY KEY (`DB_ID`),
  KEY `physicalEntity` (`physicalEntity`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `entityfunctionalstatus_2_functionalstatus`
--

DROP TABLE IF EXISTS `entityfunctionalstatus_2_functionalstatus`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `entityfunctionalstatus_2_functionalstatus` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `functionalStatus_rank` int(10) unsigned DEFAULT NULL,
  `functionalStatus` int(10) unsigned DEFAULT NULL,
  `functionalStatus_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `functionalStatus` (`functionalStatus`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `entityset`
--

DROP TABLE IF EXISTS `entityset`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `entityset` (
  `DB_ID` int(10) unsigned NOT NULL,
  `isOrdered` enum('TRUE','FALSE') DEFAULT NULL,
  `totalProt` text,
  `inferredProt` text,
  `maxHomologues` text,
  PRIMARY KEY (`DB_ID`),
  KEY `isOrdered` (`isOrdered`),
  FULLTEXT KEY `totalProt` (`totalProt`),
  FULLTEXT KEY `inferredProt` (`inferredProt`),
  FULLTEXT KEY `maxHomologues` (`maxHomologues`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `entityset_2_hasmember`
--

DROP TABLE IF EXISTS `entityset_2_hasmember`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `entityset_2_hasmember` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `hasMember_rank` int(10) unsigned DEFAULT NULL,
  `hasMember` int(10) unsigned DEFAULT NULL,
  `hasMember_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `hasMember` (`hasMember`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `entityset_2_species`
--

DROP TABLE IF EXISTS `entityset_2_species`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `entityset_2_species` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `species_rank` int(10) unsigned DEFAULT NULL,
  `species` int(10) unsigned DEFAULT NULL,
  `species_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `species` (`species`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `entityvertex`
--

DROP TABLE IF EXISTS `entityvertex`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `entityvertex` (
  `DB_ID` int(10) unsigned NOT NULL,
  `containedInEntityVertex` int(10) unsigned DEFAULT NULL,
  `containedInEntityVertex_class` varchar(64) DEFAULT NULL,
  PRIMARY KEY (`DB_ID`),
  KEY `containedInEntityVertex` (`containedInEntityVertex`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `entitywithaccessionedsequence`
--

DROP TABLE IF EXISTS `entitywithaccessionedsequence`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `entitywithaccessionedsequence` (
  `DB_ID` int(10) unsigned NOT NULL,
  `endCoordinate` int(10) DEFAULT NULL,
  `referenceEntity` int(10) unsigned DEFAULT NULL,
  `referenceEntity_class` varchar(64) DEFAULT NULL,
  `startCoordinate` int(10) DEFAULT NULL,
  PRIMARY KEY (`DB_ID`),
  KEY `endCoordinate` (`endCoordinate`),
  KEY `referenceEntity` (`referenceEntity`),
  KEY `startCoordinate` (`startCoordinate`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `entitywithaccessionedsequence_2_hasmodifiedresidue`
--

DROP TABLE IF EXISTS `entitywithaccessionedsequence_2_hasmodifiedresidue`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `entitywithaccessionedsequence_2_hasmodifiedresidue` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `hasModifiedResidue_rank` int(10) unsigned DEFAULT NULL,
  `hasModifiedResidue` int(10) unsigned DEFAULT NULL,
  `hasModifiedResidue_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `hasModifiedResidue` (`hasModifiedResidue`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `event`
--

DROP TABLE IF EXISTS `event`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `event` (
  `DB_ID` int(10) unsigned NOT NULL,
  `_doRelease` enum('TRUE','FALSE') DEFAULT NULL,
  `definition` text,
  `evidenceType` int(10) unsigned DEFAULT NULL,
  `evidenceType_class` varchar(64) DEFAULT NULL,
  `goBiologicalProcess` int(10) unsigned DEFAULT NULL,
  `goBiologicalProcess_class` varchar(64) DEFAULT NULL,
  `releaseDate` date DEFAULT NULL,
  `releaseStatus` text,
  PRIMARY KEY (`DB_ID`),
  KEY `_doRelease` (`_doRelease`),
  KEY `evidenceType` (`evidenceType`),
  KEY `goBiologicalProcess` (`goBiologicalProcess`),
  KEY `releaseDate` (`releaseDate`),
  FULLTEXT KEY `definition` (`definition`),
  FULLTEXT KEY `releaseStatus` (`releaseStatus`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `event_2_authored`
--

DROP TABLE IF EXISTS `event_2_authored`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `event_2_authored` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `authored_rank` int(10) unsigned DEFAULT NULL,
  `authored` int(10) unsigned DEFAULT NULL,
  `authored_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `authored` (`authored`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `event_2_compartment`
--

DROP TABLE IF EXISTS `event_2_compartment`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `event_2_compartment` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `compartment_rank` int(10) unsigned DEFAULT NULL,
  `compartment` int(10) unsigned DEFAULT NULL,
  `compartment_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `compartment` (`compartment`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `event_2_crossreference`
--

DROP TABLE IF EXISTS `event_2_crossreference`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `event_2_crossreference` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `crossReference_rank` int(10) unsigned DEFAULT NULL,
  `crossReference` int(10) unsigned DEFAULT NULL,
  `crossReference_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `crossReference` (`crossReference`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `event_2_disease`
--

DROP TABLE IF EXISTS `event_2_disease`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `event_2_disease` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `disease_rank` int(10) unsigned DEFAULT NULL,
  `disease` int(10) unsigned DEFAULT NULL,
  `disease_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `disease` (`disease`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `event_2_edited`
--

DROP TABLE IF EXISTS `event_2_edited`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `event_2_edited` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `edited_rank` int(10) unsigned DEFAULT NULL,
  `edited` int(10) unsigned DEFAULT NULL,
  `edited_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `edited` (`edited`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `event_2_figure`
--

DROP TABLE IF EXISTS `event_2_figure`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `event_2_figure` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `figure_rank` int(10) unsigned DEFAULT NULL,
  `figure` int(10) unsigned DEFAULT NULL,
  `figure_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `figure` (`figure`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `event_2_inferredfrom`
--

DROP TABLE IF EXISTS `event_2_inferredfrom`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `event_2_inferredfrom` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `inferredFrom_rank` int(10) unsigned DEFAULT NULL,
  `inferredFrom` int(10) unsigned DEFAULT NULL,
  `inferredFrom_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `inferredFrom` (`inferredFrom`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `event_2_literaturereference`
--

DROP TABLE IF EXISTS `event_2_literaturereference`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `event_2_literaturereference` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `literatureReference_rank` int(10) unsigned DEFAULT NULL,
  `literatureReference` int(10) unsigned DEFAULT NULL,
  `literatureReference_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `literatureReference` (`literatureReference`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `event_2_name`
--

DROP TABLE IF EXISTS `event_2_name`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `event_2_name` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `name_rank` int(10) unsigned DEFAULT NULL,
  `name` text,
  KEY `DB_ID` (`DB_ID`),
  FULLTEXT KEY `name` (`name`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `event_2_orthologousevent`
--

DROP TABLE IF EXISTS `event_2_orthologousevent`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `event_2_orthologousevent` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `orthologousEvent_rank` int(10) unsigned DEFAULT NULL,
  `orthologousEvent` int(10) unsigned DEFAULT NULL,
  `orthologousEvent_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `orthologousEvent` (`orthologousEvent`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `event_2_precedingevent`
--

DROP TABLE IF EXISTS `event_2_precedingevent`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `event_2_precedingevent` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `precedingEvent_rank` int(10) unsigned DEFAULT NULL,
  `precedingEvent` int(10) unsigned DEFAULT NULL,
  `precedingEvent_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `precedingEvent` (`precedingEvent`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `event_2_relatedspecies`
--

DROP TABLE IF EXISTS `event_2_relatedspecies`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `event_2_relatedspecies` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `relatedSpecies_rank` int(10) unsigned DEFAULT NULL,
  `relatedSpecies` int(10) unsigned DEFAULT NULL,
  `relatedSpecies_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `relatedSpecies` (`relatedSpecies`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `event_2_reviewed`
--

DROP TABLE IF EXISTS `event_2_reviewed`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `event_2_reviewed` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `reviewed_rank` int(10) unsigned DEFAULT NULL,
  `reviewed` int(10) unsigned DEFAULT NULL,
  `reviewed_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `reviewed` (`reviewed`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `event_2_revised`
--

DROP TABLE IF EXISTS `event_2_revised`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `event_2_revised` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `revised_rank` int(10) unsigned DEFAULT NULL,
  `revised` int(10) unsigned DEFAULT NULL,
  `revised_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `revised` (`revised`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `event_2_species`
--

DROP TABLE IF EXISTS `event_2_species`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `event_2_species` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `species_rank` int(10) unsigned DEFAULT NULL,
  `species` int(10) unsigned DEFAULT NULL,
  `species_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `species` (`species`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `event_2_summation`
--

DROP TABLE IF EXISTS `event_2_summation`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `event_2_summation` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `summation_rank` int(10) unsigned DEFAULT NULL,
  `summation` int(10) unsigned DEFAULT NULL,
  `summation_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `summation` (`summation`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `evidencetype`
--

DROP TABLE IF EXISTS `evidencetype`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `evidencetype` (
  `DB_ID` int(10) unsigned NOT NULL,
  `definition` text,
  PRIMARY KEY (`DB_ID`),
  FULLTEXT KEY `definition` (`definition`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `evidencetype_2_instanceof`
--

DROP TABLE IF EXISTS `evidencetype_2_instanceof`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `evidencetype_2_instanceof` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `instanceOf_rank` int(10) unsigned DEFAULT NULL,
  `instanceOf` int(10) unsigned DEFAULT NULL,
  `instanceOf_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `instanceOf` (`instanceOf`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `evidencetype_2_name`
--

DROP TABLE IF EXISTS `evidencetype_2_name`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `evidencetype_2_name` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `name_rank` int(10) unsigned DEFAULT NULL,
  `name` text,
  KEY `DB_ID` (`DB_ID`),
  FULLTEXT KEY `name` (`name`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `externalontology`
--

DROP TABLE IF EXISTS `externalontology`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `externalontology` (
  `DB_ID` int(10) unsigned NOT NULL,
  `definition` text,
  `identifier` text,
  `referenceDatabase` int(10) unsigned DEFAULT NULL,
  `referenceDatabase_class` varchar(64) DEFAULT NULL,
  PRIMARY KEY (`DB_ID`),
  KEY `referenceDatabase` (`referenceDatabase`),
  FULLTEXT KEY `definition` (`definition`),
  FULLTEXT KEY `identifier` (`identifier`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `externalontology_2_instanceof`
--

DROP TABLE IF EXISTS `externalontology_2_instanceof`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `externalontology_2_instanceof` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `instanceOf_rank` int(10) unsigned DEFAULT NULL,
  `instanceOf` int(10) unsigned DEFAULT NULL,
  `instanceOf_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `instanceOf` (`instanceOf`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `externalontology_2_name`
--

DROP TABLE IF EXISTS `externalontology_2_name`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `externalontology_2_name` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `name_rank` int(10) unsigned DEFAULT NULL,
  `name` text,
  KEY `DB_ID` (`DB_ID`),
  FULLTEXT KEY `name` (`name`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `externalontology_2_synonym`
--

DROP TABLE IF EXISTS `externalontology_2_synonym`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `externalontology_2_synonym` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `synonym_rank` int(10) unsigned DEFAULT NULL,
  `synonym` text,
  KEY `DB_ID` (`DB_ID`),
  FULLTEXT KEY `synonym` (`synonym`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `failedreaction`
--

DROP TABLE IF EXISTS `failedreaction`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `failedreaction` (
  `DB_ID` int(10) unsigned NOT NULL,
  PRIMARY KEY (`DB_ID`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `figure`
--

DROP TABLE IF EXISTS `figure`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `figure` (
  `DB_ID` int(10) unsigned NOT NULL,
  `url` text,
  PRIMARY KEY (`DB_ID`),
  FULLTEXT KEY `url` (`url`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `fragmentdeletionmodification`
--

DROP TABLE IF EXISTS `fragmentdeletionmodification`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `fragmentdeletionmodification` (
  `DB_ID` int(10) unsigned NOT NULL,
  PRIMARY KEY (`DB_ID`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `fragmentinsertionmodification`
--

DROP TABLE IF EXISTS `fragmentinsertionmodification`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `fragmentinsertionmodification` (
  `DB_ID` int(10) unsigned NOT NULL,
  `coordinate` int(10) DEFAULT NULL,
  PRIMARY KEY (`DB_ID`),
  KEY `coordinate` (`coordinate`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `fragmentmodification`
--

DROP TABLE IF EXISTS `fragmentmodification`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `fragmentmodification` (
  `DB_ID` int(10) unsigned NOT NULL,
  `endPositionInReferenceSequence` int(10) DEFAULT NULL,
  `startPositionInReferenceSequence` int(10) DEFAULT NULL,
  PRIMARY KEY (`DB_ID`),
  KEY `endPositionInReferenceSequence` (`endPositionInReferenceSequence`),
  KEY `startPositionInReferenceSequence` (`startPositionInReferenceSequence`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `fragmentreplacedmodification`
--

DROP TABLE IF EXISTS `fragmentreplacedmodification`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `fragmentreplacedmodification` (
  `DB_ID` int(10) unsigned NOT NULL,
  `alteredAminoAcidFragment` text,
  PRIMARY KEY (`DB_ID`),
  FULLTEXT KEY `alteredAminoAcidFragment` (`alteredAminoAcidFragment`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `frontpage`
--

DROP TABLE IF EXISTS `frontpage`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `frontpage` (
  `DB_ID` int(10) unsigned NOT NULL,
  PRIMARY KEY (`DB_ID`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `frontpage_2_frontpageitem`
--

DROP TABLE IF EXISTS `frontpage_2_frontpageitem`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `frontpage_2_frontpageitem` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `frontPageItem_rank` int(10) unsigned DEFAULT NULL,
  `frontPageItem` int(10) unsigned DEFAULT NULL,
  `frontPageItem_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `frontPageItem` (`frontPageItem`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `functionalstatus`
--

DROP TABLE IF EXISTS `functionalstatus`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `functionalstatus` (
  `DB_ID` int(10) unsigned NOT NULL,
  `functionalStatusType` int(10) unsigned DEFAULT NULL,
  `functionalStatusType_class` varchar(64) DEFAULT NULL,
  `structuralVariant` int(10) unsigned DEFAULT NULL,
  `structuralVariant_class` varchar(64) DEFAULT NULL,
  PRIMARY KEY (`DB_ID`),
  KEY `functionalStatusType` (`functionalStatusType`),
  KEY `structuralVariant` (`structuralVariant`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `functionalstatustype`
--

DROP TABLE IF EXISTS `functionalstatustype`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `functionalstatustype` (
  `DB_ID` int(10) unsigned NOT NULL,
  `definition` text,
  PRIMARY KEY (`DB_ID`),
  FULLTEXT KEY `definition` (`definition`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `functionalstatustype_2_name`
--

DROP TABLE IF EXISTS `functionalstatustype_2_name`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `functionalstatustype_2_name` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `name_rank` int(10) unsigned DEFAULT NULL,
  `name` text,
  KEY `DB_ID` (`DB_ID`),
  FULLTEXT KEY `name` (`name`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `geneticallymodifiedresidue`
--

DROP TABLE IF EXISTS `geneticallymodifiedresidue`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `geneticallymodifiedresidue` (
  `DB_ID` int(10) unsigned NOT NULL,
  PRIMARY KEY (`DB_ID`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `genomeencodedentity`
--

DROP TABLE IF EXISTS `genomeencodedentity`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `genomeencodedentity` (
  `DB_ID` int(10) unsigned NOT NULL,
  `species` int(10) unsigned DEFAULT NULL,
  `species_class` varchar(64) DEFAULT NULL,
  PRIMARY KEY (`DB_ID`),
  KEY `species` (`species`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `go_biologicalprocess`
--

DROP TABLE IF EXISTS `go_biologicalprocess`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `go_biologicalprocess` (
  `DB_ID` int(10) unsigned NOT NULL,
  `accession` text,
  `definition` text,
  `referenceDatabase` int(10) unsigned DEFAULT NULL,
  `referenceDatabase_class` varchar(64) DEFAULT NULL,
  PRIMARY KEY (`DB_ID`),
  KEY `referenceDatabase` (`referenceDatabase`),
  FULLTEXT KEY `accession` (`accession`),
  FULLTEXT KEY `definition` (`definition`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `go_biologicalprocess_2_componentof`
--

DROP TABLE IF EXISTS `go_biologicalprocess_2_componentof`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `go_biologicalprocess_2_componentof` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `componentOf_rank` int(10) unsigned DEFAULT NULL,
  `componentOf` int(10) unsigned DEFAULT NULL,
  `componentOf_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `componentOf` (`componentOf`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `go_biologicalprocess_2_haspart`
--

DROP TABLE IF EXISTS `go_biologicalprocess_2_haspart`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `go_biologicalprocess_2_haspart` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `hasPart_rank` int(10) unsigned DEFAULT NULL,
  `hasPart` int(10) unsigned DEFAULT NULL,
  `hasPart_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `hasPart` (`hasPart`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `go_biologicalprocess_2_instanceof`
--

DROP TABLE IF EXISTS `go_biologicalprocess_2_instanceof`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `go_biologicalprocess_2_instanceof` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `instanceOf_rank` int(10) unsigned DEFAULT NULL,
  `instanceOf` int(10) unsigned DEFAULT NULL,
  `instanceOf_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `instanceOf` (`instanceOf`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `go_biologicalprocess_2_name`
--

DROP TABLE IF EXISTS `go_biologicalprocess_2_name`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `go_biologicalprocess_2_name` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `name_rank` int(10) unsigned DEFAULT NULL,
  `name` text,
  KEY `DB_ID` (`DB_ID`),
  FULLTEXT KEY `name` (`name`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `go_biologicalprocess_2_negativelyregulate`
--

DROP TABLE IF EXISTS `go_biologicalprocess_2_negativelyregulate`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `go_biologicalprocess_2_negativelyregulate` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `negativelyRegulate_rank` int(10) unsigned DEFAULT NULL,
  `negativelyRegulate` int(10) unsigned DEFAULT NULL,
  `negativelyRegulate_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `negativelyRegulate` (`negativelyRegulate`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `go_biologicalprocess_2_positivelyregulate`
--

DROP TABLE IF EXISTS `go_biologicalprocess_2_positivelyregulate`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `go_biologicalprocess_2_positivelyregulate` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `positivelyRegulate_rank` int(10) unsigned DEFAULT NULL,
  `positivelyRegulate` int(10) unsigned DEFAULT NULL,
  `positivelyRegulate_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `positivelyRegulate` (`positivelyRegulate`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `go_biologicalprocess_2_regulate`
--

DROP TABLE IF EXISTS `go_biologicalprocess_2_regulate`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `go_biologicalprocess_2_regulate` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `regulate_rank` int(10) unsigned DEFAULT NULL,
  `regulate` int(10) unsigned DEFAULT NULL,
  `regulate_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `regulate` (`regulate`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `go_cellularcomponent`
--

DROP TABLE IF EXISTS `go_cellularcomponent`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `go_cellularcomponent` (
  `DB_ID` int(10) unsigned NOT NULL,
  `accession` text,
  `definition` text,
  `referenceDatabase` int(10) unsigned DEFAULT NULL,
  `referenceDatabase_class` varchar(64) DEFAULT NULL,
  PRIMARY KEY (`DB_ID`),
  KEY `referenceDatabase` (`referenceDatabase`),
  FULLTEXT KEY `accession` (`accession`),
  FULLTEXT KEY `definition` (`definition`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `go_cellularcomponent_2_componentof`
--

DROP TABLE IF EXISTS `go_cellularcomponent_2_componentof`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `go_cellularcomponent_2_componentof` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `componentOf_rank` int(10) unsigned DEFAULT NULL,
  `componentOf` int(10) unsigned DEFAULT NULL,
  `componentOf_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `componentOf` (`componentOf`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `go_cellularcomponent_2_haspart`
--

DROP TABLE IF EXISTS `go_cellularcomponent_2_haspart`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `go_cellularcomponent_2_haspart` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `hasPart_rank` int(10) unsigned DEFAULT NULL,
  `hasPart` int(10) unsigned DEFAULT NULL,
  `hasPart_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `hasPart` (`hasPart`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `go_cellularcomponent_2_instanceof`
--

DROP TABLE IF EXISTS `go_cellularcomponent_2_instanceof`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `go_cellularcomponent_2_instanceof` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `instanceOf_rank` int(10) unsigned DEFAULT NULL,
  `instanceOf` int(10) unsigned DEFAULT NULL,
  `instanceOf_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `instanceOf` (`instanceOf`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `go_cellularcomponent_2_name`
--

DROP TABLE IF EXISTS `go_cellularcomponent_2_name`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `go_cellularcomponent_2_name` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `name_rank` int(10) unsigned DEFAULT NULL,
  `name` text,
  KEY `DB_ID` (`DB_ID`),
  FULLTEXT KEY `name` (`name`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `go_molecularfunction`
--

DROP TABLE IF EXISTS `go_molecularfunction`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `go_molecularfunction` (
  `DB_ID` int(10) unsigned NOT NULL,
  `accession` text,
  `definition` text,
  `referenceDatabase` int(10) unsigned DEFAULT NULL,
  `referenceDatabase_class` varchar(64) DEFAULT NULL,
  PRIMARY KEY (`DB_ID`),
  KEY `referenceDatabase` (`referenceDatabase`),
  FULLTEXT KEY `accession` (`accession`),
  FULLTEXT KEY `definition` (`definition`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `go_molecularfunction_2_componentof`
--

DROP TABLE IF EXISTS `go_molecularfunction_2_componentof`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `go_molecularfunction_2_componentof` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `componentOf_rank` int(10) unsigned DEFAULT NULL,
  `componentOf` int(10) unsigned DEFAULT NULL,
  `componentOf_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `componentOf` (`componentOf`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `go_molecularfunction_2_ecnumber`
--

DROP TABLE IF EXISTS `go_molecularfunction_2_ecnumber`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `go_molecularfunction_2_ecnumber` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `ecNumber_rank` int(10) unsigned DEFAULT NULL,
  `ecNumber` text,
  KEY `DB_ID` (`DB_ID`),
  FULLTEXT KEY `ecNumber` (`ecNumber`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `go_molecularfunction_2_haspart`
--

DROP TABLE IF EXISTS `go_molecularfunction_2_haspart`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `go_molecularfunction_2_haspart` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `hasPart_rank` int(10) unsigned DEFAULT NULL,
  `hasPart` int(10) unsigned DEFAULT NULL,
  `hasPart_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `hasPart` (`hasPart`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `go_molecularfunction_2_instanceof`
--

DROP TABLE IF EXISTS `go_molecularfunction_2_instanceof`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `go_molecularfunction_2_instanceof` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `instanceOf_rank` int(10) unsigned DEFAULT NULL,
  `instanceOf` int(10) unsigned DEFAULT NULL,
  `instanceOf_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `instanceOf` (`instanceOf`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `go_molecularfunction_2_name`
--

DROP TABLE IF EXISTS `go_molecularfunction_2_name`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `go_molecularfunction_2_name` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `name_rank` int(10) unsigned DEFAULT NULL,
  `name` text,
  KEY `DB_ID` (`DB_ID`),
  FULLTEXT KEY `name` (`name`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `go_molecularfunction_2_negativelyregulate`
--

DROP TABLE IF EXISTS `go_molecularfunction_2_negativelyregulate`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `go_molecularfunction_2_negativelyregulate` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `negativelyRegulate_rank` int(10) unsigned DEFAULT NULL,
  `negativelyRegulate` int(10) unsigned DEFAULT NULL,
  `negativelyRegulate_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `negativelyRegulate` (`negativelyRegulate`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `go_molecularfunction_2_positivelyregulate`
--

DROP TABLE IF EXISTS `go_molecularfunction_2_positivelyregulate`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `go_molecularfunction_2_positivelyregulate` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `positivelyRegulate_rank` int(10) unsigned DEFAULT NULL,
  `positivelyRegulate` int(10) unsigned DEFAULT NULL,
  `positivelyRegulate_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `positivelyRegulate` (`positivelyRegulate`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `go_molecularfunction_2_regulate`
--

DROP TABLE IF EXISTS `go_molecularfunction_2_regulate`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `go_molecularfunction_2_regulate` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `regulate_rank` int(10) unsigned DEFAULT NULL,
  `regulate` int(10) unsigned DEFAULT NULL,
  `regulate_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `regulate` (`regulate`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `groupmodifiedresidue`
--

DROP TABLE IF EXISTS `groupmodifiedresidue`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `groupmodifiedresidue` (
  `DB_ID` int(10) unsigned NOT NULL,
  `modification` int(10) unsigned DEFAULT NULL,
  `modification_class` varchar(64) DEFAULT NULL,
  PRIMARY KEY (`DB_ID`),
  KEY `modification` (`modification`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `instanceedit`
--

DROP TABLE IF EXISTS `instanceedit`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `instanceedit` (
  `DB_ID` int(10) unsigned NOT NULL,
  `_applyToAllEditedInstances` text,
  `dateTime` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `note` text,
  PRIMARY KEY (`DB_ID`),
  KEY `dateTime` (`dateTime`),
  FULLTEXT KEY `_applyToAllEditedInstances` (`_applyToAllEditedInstances`),
  FULLTEXT KEY `note` (`note`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `instanceedit_2_author`
--

DROP TABLE IF EXISTS `instanceedit_2_author`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `instanceedit_2_author` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `author_rank` int(10) unsigned DEFAULT NULL,
  `author` int(10) unsigned DEFAULT NULL,
  `author_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `author` (`author`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `interchaincrosslinkedresidue`
--

DROP TABLE IF EXISTS `interchaincrosslinkedresidue`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `interchaincrosslinkedresidue` (
  `DB_ID` int(10) unsigned NOT NULL,
  PRIMARY KEY (`DB_ID`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `interchaincrosslinkedresidue_2_equivalentto`
--

DROP TABLE IF EXISTS `interchaincrosslinkedresidue_2_equivalentto`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `interchaincrosslinkedresidue_2_equivalentto` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `equivalentTo_rank` int(10) unsigned DEFAULT NULL,
  `equivalentTo` int(10) unsigned DEFAULT NULL,
  `equivalentTo_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `equivalentTo` (`equivalentTo`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `interchaincrosslinkedresidue_2_secondreferencesequence`
--

DROP TABLE IF EXISTS `interchaincrosslinkedresidue_2_secondreferencesequence`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `interchaincrosslinkedresidue_2_secondreferencesequence` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `secondReferenceSequence_rank` int(10) unsigned DEFAULT NULL,
  `secondReferenceSequence` int(10) unsigned DEFAULT NULL,
  `secondReferenceSequence_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `secondReferenceSequence` (`secondReferenceSequence`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `intrachaincrosslinkedresidue`
--

DROP TABLE IF EXISTS `intrachaincrosslinkedresidue`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `intrachaincrosslinkedresidue` (
  `DB_ID` int(10) unsigned NOT NULL,
  PRIMARY KEY (`DB_ID`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `literaturereference`
--

DROP TABLE IF EXISTS `literaturereference`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `literaturereference` (
  `DB_ID` int(10) unsigned NOT NULL,
  `journal` varchar(255) DEFAULT NULL,
  `pages` text,
  `pubMedIdentifier` int(10) DEFAULT NULL,
  `volume` int(10) DEFAULT NULL,
  `year` int(10) DEFAULT NULL,
  PRIMARY KEY (`DB_ID`),
  KEY `journal` (`journal`),
  KEY `pubMedIdentifier` (`pubMedIdentifier`),
  KEY `volume` (`volume`),
  KEY `year` (`year`),
  FULLTEXT KEY `journal_fulltext` (`journal`),
  FULLTEXT KEY `pages` (`pages`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `modifiedresidue`
--

DROP TABLE IF EXISTS `modifiedresidue`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `modifiedresidue` (
  `DB_ID` int(10) unsigned NOT NULL,
  PRIMARY KEY (`DB_ID`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `negativeregulation`
--

DROP TABLE IF EXISTS `negativeregulation`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `negativeregulation` (
  `DB_ID` int(10) unsigned NOT NULL,
  PRIMARY KEY (`DB_ID`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `ontology`
--

DROP TABLE IF EXISTS `ontology`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `ontology` (
  `ontology` longblob
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `openset`
--

DROP TABLE IF EXISTS `openset`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `openset` (
  `DB_ID` int(10) unsigned NOT NULL,
  `referenceEntity` int(10) unsigned DEFAULT NULL,
  `referenceEntity_class` varchar(64) DEFAULT NULL,
  PRIMARY KEY (`DB_ID`),
  KEY `referenceEntity` (`referenceEntity`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `otherentity`
--

DROP TABLE IF EXISTS `otherentity`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `otherentity` (
  `DB_ID` int(10) unsigned NOT NULL,
  PRIMARY KEY (`DB_ID`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `parameters`
--

DROP TABLE IF EXISTS `parameters`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `parameters` (
  `parameter` varchar(64) NOT NULL,
  `value` longblob,
  PRIMARY KEY (`parameter`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `pathway`
--

DROP TABLE IF EXISTS `pathway`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `pathway` (
  `DB_ID` int(10) unsigned NOT NULL,
  `doi` varchar(64) DEFAULT NULL,
  `isCanonical` enum('TRUE','FALSE') DEFAULT NULL,
  `normalPathway` int(10) unsigned DEFAULT NULL,
  `normalPathway_class` varchar(64) DEFAULT NULL,
  PRIMARY KEY (`DB_ID`),
  KEY `doi` (`doi`),
  KEY `isCanonical` (`isCanonical`),
  KEY `normalPathway` (`normalPathway`),
  FULLTEXT KEY `doi_fulltext` (`doi`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `pathway_2_hasevent`
--

DROP TABLE IF EXISTS `pathway_2_hasevent`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `pathway_2_hasevent` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `hasEvent_rank` int(10) unsigned DEFAULT NULL,
  `hasEvent` int(10) unsigned DEFAULT NULL,
  `hasEvent_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `hasEvent` (`hasEvent`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `pathwaycoordinates`
--

DROP TABLE IF EXISTS `pathwaycoordinates`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `pathwaycoordinates` (
  `DB_ID` int(10) unsigned NOT NULL,
  `locatedEvent` int(10) unsigned DEFAULT NULL,
  `locatedEvent_class` varchar(64) DEFAULT NULL,
  `maxX` int(10) DEFAULT NULL,
  `maxY` int(10) DEFAULT NULL,
  `minX` int(10) DEFAULT NULL,
  `minY` int(10) DEFAULT NULL,
  PRIMARY KEY (`DB_ID`),
  KEY `locatedEvent` (`locatedEvent`),
  KEY `maxX` (`maxX`),
  KEY `maxY` (`maxY`),
  KEY `minX` (`minX`),
  KEY `minY` (`minY`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `pathwaydiagram`
--

DROP TABLE IF EXISTS `pathwaydiagram`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `pathwaydiagram` (
  `DB_ID` int(10) unsigned NOT NULL,
  `height` int(10) DEFAULT NULL,
  `storedATXML` longblob,
  `width` int(10) DEFAULT NULL,
  PRIMARY KEY (`DB_ID`),
  KEY `height` (`height`),
  KEY `width` (`width`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `pathwaydiagram_2_representedpathway`
--

DROP TABLE IF EXISTS `pathwaydiagram_2_representedpathway`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `pathwaydiagram_2_representedpathway` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `representedPathway_rank` int(10) unsigned DEFAULT NULL,
  `representedPathway` int(10) unsigned DEFAULT NULL,
  `representedPathway_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `representedPathway` (`representedPathway`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `pathwaydiagramitem`
--

DROP TABLE IF EXISTS `pathwaydiagramitem`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `pathwaydiagramitem` (
  `DB_ID` int(10) unsigned NOT NULL,
  PRIMARY KEY (`DB_ID`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `pathwayvertex`
--

DROP TABLE IF EXISTS `pathwayvertex`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `pathwayvertex` (
  `DB_ID` int(10) unsigned NOT NULL,
  PRIMARY KEY (`DB_ID`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `person`
--

DROP TABLE IF EXISTS `person`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `person` (
  `DB_ID` int(10) unsigned NOT NULL,
  `eMailAddress` varchar(255) DEFAULT NULL,
  `firstname` text,
  `initial` varchar(10) DEFAULT NULL,
  `project` text,
  `surname` varchar(255) DEFAULT NULL,
  `url` text,
  PRIMARY KEY (`DB_ID`),
  KEY `eMailAddress` (`eMailAddress`),
  KEY `initial` (`initial`),
  KEY `surname` (`surname`),
  FULLTEXT KEY `eMailAddress_fulltext` (`eMailAddress`),
  FULLTEXT KEY `firstname` (`firstname`),
  FULLTEXT KEY `initial_fulltext` (`initial`),
  FULLTEXT KEY `project` (`project`),
  FULLTEXT KEY `surname_fulltext` (`surname`),
  FULLTEXT KEY `url` (`url`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `person_2_affiliation`
--

DROP TABLE IF EXISTS `person_2_affiliation`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `person_2_affiliation` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `affiliation_rank` int(10) unsigned DEFAULT NULL,
  `affiliation` int(10) unsigned DEFAULT NULL,
  `affiliation_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `affiliation` (`affiliation`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `person_2_crossreference`
--

DROP TABLE IF EXISTS `person_2_crossreference`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `person_2_crossreference` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `crossReference_rank` int(10) unsigned DEFAULT NULL,
  `crossReference` int(10) unsigned DEFAULT NULL,
  `crossReference_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `crossReference` (`crossReference`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `person_2_figure`
--

DROP TABLE IF EXISTS `person_2_figure`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `person_2_figure` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `figure_rank` int(10) unsigned DEFAULT NULL,
  `figure` int(10) unsigned DEFAULT NULL,
  `figure_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `figure` (`figure`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `physicalentity`
--

DROP TABLE IF EXISTS `physicalentity`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `physicalentity` (
  `DB_ID` int(10) unsigned NOT NULL,
  `authored` int(10) unsigned DEFAULT NULL,
  `authored_class` varchar(64) DEFAULT NULL,
  `cellType` int(10) unsigned DEFAULT NULL,
  `cellType_class` varchar(64) DEFAULT NULL,
  `definition` text,
  `goCellularComponent` int(10) unsigned DEFAULT NULL,
  `goCellularComponent_class` varchar(64) DEFAULT NULL,
  `systematicName` text,
  PRIMARY KEY (`DB_ID`),
  KEY `authored` (`authored`),
  KEY `cellType` (`cellType`),
  KEY `goCellularComponent` (`goCellularComponent`),
  FULLTEXT KEY `definition` (`definition`),
  FULLTEXT KEY `systematicName` (`systematicName`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `physicalentity_2_compartment`
--

DROP TABLE IF EXISTS `physicalentity_2_compartment`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `physicalentity_2_compartment` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `compartment_rank` int(10) unsigned DEFAULT NULL,
  `compartment` int(10) unsigned DEFAULT NULL,
  `compartment_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `compartment` (`compartment`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `physicalentity_2_crossreference`
--

DROP TABLE IF EXISTS `physicalentity_2_crossreference`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `physicalentity_2_crossreference` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `crossReference_rank` int(10) unsigned DEFAULT NULL,
  `crossReference` int(10) unsigned DEFAULT NULL,
  `crossReference_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `crossReference` (`crossReference`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `physicalentity_2_disease`
--

DROP TABLE IF EXISTS `physicalentity_2_disease`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `physicalentity_2_disease` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `disease_rank` int(10) unsigned DEFAULT NULL,
  `disease` int(10) unsigned DEFAULT NULL,
  `disease_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `disease` (`disease`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `physicalentity_2_edited`
--

DROP TABLE IF EXISTS `physicalentity_2_edited`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `physicalentity_2_edited` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `edited_rank` int(10) unsigned DEFAULT NULL,
  `edited` int(10) unsigned DEFAULT NULL,
  `edited_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `edited` (`edited`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `physicalentity_2_figure`
--

DROP TABLE IF EXISTS `physicalentity_2_figure`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `physicalentity_2_figure` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `figure_rank` int(10) unsigned DEFAULT NULL,
  `figure` int(10) unsigned DEFAULT NULL,
  `figure_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `figure` (`figure`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `physicalentity_2_inferredfrom`
--

DROP TABLE IF EXISTS `physicalentity_2_inferredfrom`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `physicalentity_2_inferredfrom` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `inferredFrom_rank` int(10) unsigned DEFAULT NULL,
  `inferredFrom` int(10) unsigned DEFAULT NULL,
  `inferredFrom_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `inferredFrom` (`inferredFrom`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `physicalentity_2_inferredto`
--

DROP TABLE IF EXISTS `physicalentity_2_inferredto`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `physicalentity_2_inferredto` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `inferredTo_rank` int(10) unsigned DEFAULT NULL,
  `inferredTo` int(10) unsigned DEFAULT NULL,
  `inferredTo_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `inferredTo` (`inferredTo`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `physicalentity_2_literaturereference`
--

DROP TABLE IF EXISTS `physicalentity_2_literaturereference`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `physicalentity_2_literaturereference` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `literatureReference_rank` int(10) unsigned DEFAULT NULL,
  `literatureReference` int(10) unsigned DEFAULT NULL,
  `literatureReference_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `literatureReference` (`literatureReference`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `physicalentity_2_name`
--

DROP TABLE IF EXISTS `physicalentity_2_name`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `physicalentity_2_name` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `name_rank` int(10) unsigned DEFAULT NULL,
  `name` text,
  KEY `DB_ID` (`DB_ID`),
  FULLTEXT KEY `name` (`name`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `physicalentity_2_reviewed`
--

DROP TABLE IF EXISTS `physicalentity_2_reviewed`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `physicalentity_2_reviewed` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `reviewed_rank` int(10) unsigned DEFAULT NULL,
  `reviewed` int(10) unsigned DEFAULT NULL,
  `reviewed_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `reviewed` (`reviewed`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `physicalentity_2_revised`
--

DROP TABLE IF EXISTS `physicalentity_2_revised`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `physicalentity_2_revised` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `revised_rank` int(10) unsigned DEFAULT NULL,
  `revised` int(10) unsigned DEFAULT NULL,
  `revised_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `revised` (`revised`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `physicalentity_2_summation`
--

DROP TABLE IF EXISTS `physicalentity_2_summation`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `physicalentity_2_summation` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `summation_rank` int(10) unsigned DEFAULT NULL,
  `summation` int(10) unsigned DEFAULT NULL,
  `summation_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `summation` (`summation`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `polymer`
--

DROP TABLE IF EXISTS `polymer`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `polymer` (
  `DB_ID` int(10) unsigned NOT NULL,
  `maxUnitCount` int(10) DEFAULT NULL,
  `minUnitCount` int(10) DEFAULT NULL,
  `totalProt` text,
  `maxHomologues` text,
  `inferredProt` text,
  PRIMARY KEY (`DB_ID`),
  KEY `maxUnitCount` (`maxUnitCount`),
  KEY `minUnitCount` (`minUnitCount`),
  FULLTEXT KEY `totalProt` (`totalProt`),
  FULLTEXT KEY `maxHomologues` (`maxHomologues`),
  FULLTEXT KEY `inferredProt` (`inferredProt`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `polymer_2_repeatedunit`
--

DROP TABLE IF EXISTS `polymer_2_repeatedunit`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `polymer_2_repeatedunit` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `repeatedUnit_rank` int(10) unsigned DEFAULT NULL,
  `repeatedUnit` int(10) unsigned DEFAULT NULL,
  `repeatedUnit_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `repeatedUnit` (`repeatedUnit`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `polymer_2_species`
--

DROP TABLE IF EXISTS `polymer_2_species`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `polymer_2_species` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `species_rank` int(10) unsigned DEFAULT NULL,
  `species` int(10) unsigned DEFAULT NULL,
  `species_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `species` (`species`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `polymerisation`
--

DROP TABLE IF EXISTS `polymerisation`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `polymerisation` (
  `DB_ID` int(10) unsigned NOT NULL,
  PRIMARY KEY (`DB_ID`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `positiveregulation`
--

DROP TABLE IF EXISTS `positiveregulation`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `positiveregulation` (
  `DB_ID` int(10) unsigned NOT NULL,
  PRIMARY KEY (`DB_ID`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `psimod`
--

DROP TABLE IF EXISTS `psimod`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `psimod` (
  `DB_ID` int(10) unsigned NOT NULL,
  PRIMARY KEY (`DB_ID`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `publication`
--

DROP TABLE IF EXISTS `publication`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `publication` (
  `DB_ID` int(10) unsigned NOT NULL,
  `title` text,
  PRIMARY KEY (`DB_ID`),
  FULLTEXT KEY `title` (`title`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `publication_2_author`
--

DROP TABLE IF EXISTS `publication_2_author`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `publication_2_author` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `author_rank` int(10) unsigned DEFAULT NULL,
  `author` int(10) unsigned DEFAULT NULL,
  `author_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `author` (`author`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `reaction`
--

DROP TABLE IF EXISTS `reaction`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `reaction` (
  `DB_ID` int(10) unsigned NOT NULL,
  `reverseReaction` int(10) unsigned DEFAULT NULL,
  `reverseReaction_class` varchar(64) DEFAULT NULL,
  `totalProt` text,
  `maxHomologues` text,
  `inferredProt` text,
  PRIMARY KEY (`DB_ID`),
  KEY `reverseReaction` (`reverseReaction`),
  FULLTEXT KEY `totalProt` (`totalProt`),
  FULLTEXT KEY `maxHomologues` (`maxHomologues`),
  FULLTEXT KEY `inferredProt` (`inferredProt`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `reactioncoordinates`
--

DROP TABLE IF EXISTS `reactioncoordinates`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `reactioncoordinates` (
  `DB_ID` int(10) unsigned NOT NULL,
  `locatedEvent` int(10) unsigned DEFAULT NULL,
  `locatedEvent_class` varchar(64) DEFAULT NULL,
  `locationContext` int(10) unsigned DEFAULT NULL,
  `locationContext_class` varchar(64) DEFAULT NULL,
  `sourceX` int(10) DEFAULT NULL,
  `sourceY` int(10) DEFAULT NULL,
  `targetX` int(10) DEFAULT NULL,
  `targetY` int(10) DEFAULT NULL,
  PRIMARY KEY (`DB_ID`),
  KEY `locatedEvent` (`locatedEvent`),
  KEY `locationContext` (`locationContext`),
  KEY `sourceX` (`sourceX`),
  KEY `sourceY` (`sourceY`),
  KEY `targetX` (`targetX`),
  KEY `targetY` (`targetY`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `reactionlikeevent`
--

DROP TABLE IF EXISTS `reactionlikeevent`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `reactionlikeevent` (
  `DB_ID` int(10) unsigned NOT NULL,
  `isChimeric` enum('TRUE','FALSE') DEFAULT NULL,
  `systematicName` text,
  PRIMARY KEY (`DB_ID`),
  KEY `isChimeric` (`isChimeric`),
  FULLTEXT KEY `systematicName` (`systematicName`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `reactionlikeevent_2_catalystactivity`
--

DROP TABLE IF EXISTS `reactionlikeevent_2_catalystactivity`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `reactionlikeevent_2_catalystactivity` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `catalystActivity_rank` int(10) unsigned DEFAULT NULL,
  `catalystActivity` int(10) unsigned DEFAULT NULL,
  `catalystActivity_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `catalystActivity` (`catalystActivity`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `reactionlikeevent_2_entityfunctionalstatus`
--

DROP TABLE IF EXISTS `reactionlikeevent_2_entityfunctionalstatus`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `reactionlikeevent_2_entityfunctionalstatus` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `entityFunctionalStatus_rank` int(10) unsigned DEFAULT NULL,
  `entityFunctionalStatus` int(10) unsigned DEFAULT NULL,
  `entityFunctionalStatus_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `entityFunctionalStatus` (`entityFunctionalStatus`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `reactionlikeevent_2_entityonothercell`
--

DROP TABLE IF EXISTS `reactionlikeevent_2_entityonothercell`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `reactionlikeevent_2_entityonothercell` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `entityOnOtherCell_rank` int(10) unsigned DEFAULT NULL,
  `entityOnOtherCell` int(10) unsigned DEFAULT NULL,
  `entityOnOtherCell_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `entityOnOtherCell` (`entityOnOtherCell`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `reactionlikeevent_2_input`
--

DROP TABLE IF EXISTS `reactionlikeevent_2_input`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `reactionlikeevent_2_input` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `input_rank` int(10) unsigned DEFAULT NULL,
  `input` int(10) unsigned DEFAULT NULL,
  `input_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `input` (`input`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `reactionlikeevent_2_normalreaction`
--

DROP TABLE IF EXISTS `reactionlikeevent_2_normalreaction`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `reactionlikeevent_2_normalreaction` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `normalReaction_rank` int(10) unsigned DEFAULT NULL,
  `normalReaction` int(10) unsigned DEFAULT NULL,
  `normalReaction_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `normalReaction` (`normalReaction`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `reactionlikeevent_2_output`
--

DROP TABLE IF EXISTS `reactionlikeevent_2_output`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `reactionlikeevent_2_output` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `output_rank` int(10) unsigned DEFAULT NULL,
  `output` int(10) unsigned DEFAULT NULL,
  `output_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `output` (`output`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `reactionlikeevent_2_requiredinputcomponent`
--

DROP TABLE IF EXISTS `reactionlikeevent_2_requiredinputcomponent`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `reactionlikeevent_2_requiredinputcomponent` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `requiredInputComponent_rank` int(10) unsigned DEFAULT NULL,
  `requiredInputComponent` int(10) unsigned DEFAULT NULL,
  `requiredInputComponent_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `requiredInputComponent` (`requiredInputComponent`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `reactionvertex`
--

DROP TABLE IF EXISTS `reactionvertex`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `reactionvertex` (
  `DB_ID` int(10) unsigned NOT NULL,
  `pointCoordinates` text,
  PRIMARY KEY (`DB_ID`),
  FULLTEXT KEY `pointCoordinates` (`pointCoordinates`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `referencedatabase`
--

DROP TABLE IF EXISTS `referencedatabase`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `referencedatabase` (
  `DB_ID` int(10) unsigned NOT NULL,
  `accessUrl` text,
  `url` text,
  PRIMARY KEY (`DB_ID`),
  FULLTEXT KEY `accessUrl` (`accessUrl`),
  FULLTEXT KEY `url` (`url`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `referencedatabase_2_name`
--

DROP TABLE IF EXISTS `referencedatabase_2_name`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `referencedatabase_2_name` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `name_rank` int(10) unsigned DEFAULT NULL,
  `name` text,
  KEY `DB_ID` (`DB_ID`),
  FULLTEXT KEY `name` (`name`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `referencednasequence`
--

DROP TABLE IF EXISTS `referencednasequence`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `referencednasequence` (
  `DB_ID` int(10) unsigned NOT NULL,
  PRIMARY KEY (`DB_ID`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `referenceentity`
--

DROP TABLE IF EXISTS `referenceentity`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `referenceentity` (
  `DB_ID` int(10) unsigned NOT NULL,
  `identifier` text,
  `referenceDatabase` int(10) unsigned DEFAULT NULL,
  `referenceDatabase_class` varchar(64) DEFAULT NULL,
  PRIMARY KEY (`DB_ID`),
  KEY `referenceDatabase` (`referenceDatabase`),
  FULLTEXT KEY `identifier` (`identifier`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `referenceentity_2_crossreference`
--

DROP TABLE IF EXISTS `referenceentity_2_crossreference`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `referenceentity_2_crossreference` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `crossReference_rank` int(10) unsigned DEFAULT NULL,
  `crossReference` int(10) unsigned DEFAULT NULL,
  `crossReference_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `crossReference` (`crossReference`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `referenceentity_2_name`
--

DROP TABLE IF EXISTS `referenceentity_2_name`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `referenceentity_2_name` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `name_rank` int(10) unsigned DEFAULT NULL,
  `name` text,
  KEY `DB_ID` (`DB_ID`),
  FULLTEXT KEY `name` (`name`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `referenceentity_2_otheridentifier`
--

DROP TABLE IF EXISTS `referenceentity_2_otheridentifier`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `referenceentity_2_otheridentifier` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `otherIdentifier_rank` int(10) unsigned DEFAULT NULL,
  `otherIdentifier` text,
  KEY `DB_ID` (`DB_ID`),
  FULLTEXT KEY `otherIdentifier` (`otherIdentifier`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `referencegeneproduct`
--

DROP TABLE IF EXISTS `referencegeneproduct`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `referencegeneproduct` (
  `DB_ID` int(10) unsigned NOT NULL,
  `_chainChangeLog` text,
  PRIMARY KEY (`DB_ID`),
  FULLTEXT KEY `_chainChangeLog` (`_chainChangeLog`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `referencegeneproduct_2_chain`
--

DROP TABLE IF EXISTS `referencegeneproduct_2_chain`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `referencegeneproduct_2_chain` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `chain_rank` int(10) unsigned DEFAULT NULL,
  `chain` text,
  KEY `DB_ID` (`DB_ID`),
  FULLTEXT KEY `chain` (`chain`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `referencegeneproduct_2_referencegene`
--

DROP TABLE IF EXISTS `referencegeneproduct_2_referencegene`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `referencegeneproduct_2_referencegene` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `referenceGene_rank` int(10) unsigned DEFAULT NULL,
  `referenceGene` int(10) unsigned DEFAULT NULL,
  `referenceGene_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `referenceGene` (`referenceGene`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `referencegeneproduct_2_referencetranscript`
--

DROP TABLE IF EXISTS `referencegeneproduct_2_referencetranscript`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `referencegeneproduct_2_referencetranscript` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `referenceTranscript_rank` int(10) unsigned DEFAULT NULL,
  `referenceTranscript` int(10) unsigned DEFAULT NULL,
  `referenceTranscript_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `referenceTranscript` (`referenceTranscript`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `referencegroup`
--

DROP TABLE IF EXISTS `referencegroup`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `referencegroup` (
  `DB_ID` int(10) unsigned NOT NULL,
  `atomicConnectivity` text,
  `formula` text,
  PRIMARY KEY (`DB_ID`),
  FULLTEXT KEY `atomicConnectivity` (`atomicConnectivity`),
  FULLTEXT KEY `formula` (`formula`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `referenceisoform`
--

DROP TABLE IF EXISTS `referenceisoform`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `referenceisoform` (
  `DB_ID` int(10) unsigned NOT NULL,
  `variantIdentifier` text,
  PRIMARY KEY (`DB_ID`),
  FULLTEXT KEY `variantIdentifier` (`variantIdentifier`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `referenceisoform_2_isoformparent`
--

DROP TABLE IF EXISTS `referenceisoform_2_isoformparent`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `referenceisoform_2_isoformparent` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `isoformParent_rank` int(10) unsigned DEFAULT NULL,
  `isoformParent` int(10) unsigned DEFAULT NULL,
  `isoformParent_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `isoformParent` (`isoformParent`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `referencemolecule`
--

DROP TABLE IF EXISTS `referencemolecule`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `referencemolecule` (
  `DB_ID` int(10) unsigned NOT NULL,
  `atomicConnectivity` text,
  `formula` text,
  PRIMARY KEY (`DB_ID`),
  FULLTEXT KEY `atomicConnectivity` (`atomicConnectivity`),
  FULLTEXT KEY `formula` (`formula`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `referencernasequence`
--

DROP TABLE IF EXISTS `referencernasequence`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `referencernasequence` (
  `DB_ID` int(10) unsigned NOT NULL,
  PRIMARY KEY (`DB_ID`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `referencernasequence_2_referencegene`
--

DROP TABLE IF EXISTS `referencernasequence_2_referencegene`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `referencernasequence_2_referencegene` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `referenceGene_rank` int(10) unsigned DEFAULT NULL,
  `referenceGene` int(10) unsigned DEFAULT NULL,
  `referenceGene_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `referenceGene` (`referenceGene`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `referencesequence`
--

DROP TABLE IF EXISTS `referencesequence`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `referencesequence` (
  `DB_ID` int(10) unsigned NOT NULL,
  `checksum` text,
  `isSequenceChanged` text,
  `sequenceLength` int(10) DEFAULT NULL,
  `species` int(10) unsigned DEFAULT NULL,
  `species_class` varchar(64) DEFAULT NULL,
  PRIMARY KEY (`DB_ID`),
  KEY `sequenceLength` (`sequenceLength`),
  KEY `species` (`species`),
  FULLTEXT KEY `checksum` (`checksum`),
  FULLTEXT KEY `isSequenceChanged` (`isSequenceChanged`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `referencesequence_2_comment`
--

DROP TABLE IF EXISTS `referencesequence_2_comment`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `referencesequence_2_comment` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `comment_rank` int(10) unsigned DEFAULT NULL,
  `comment` text,
  KEY `DB_ID` (`DB_ID`),
  FULLTEXT KEY `comment` (`comment`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `referencesequence_2_description`
--

DROP TABLE IF EXISTS `referencesequence_2_description`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `referencesequence_2_description` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `description_rank` int(10) unsigned DEFAULT NULL,
  `description` text,
  KEY `DB_ID` (`DB_ID`),
  FULLTEXT KEY `description` (`description`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `referencesequence_2_genename`
--

DROP TABLE IF EXISTS `referencesequence_2_genename`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `referencesequence_2_genename` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `geneName_rank` int(10) unsigned DEFAULT NULL,
  `geneName` text,
  KEY `DB_ID` (`DB_ID`),
  FULLTEXT KEY `geneName` (`geneName`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `referencesequence_2_keyword`
--

DROP TABLE IF EXISTS `referencesequence_2_keyword`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `referencesequence_2_keyword` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `keyword_rank` int(10) unsigned DEFAULT NULL,
  `keyword` text,
  KEY `DB_ID` (`DB_ID`),
  FULLTEXT KEY `keyword` (`keyword`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `referencesequence_2_secondaryidentifier`
--

DROP TABLE IF EXISTS `referencesequence_2_secondaryidentifier`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `referencesequence_2_secondaryidentifier` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `secondaryIdentifier_rank` int(10) unsigned DEFAULT NULL,
  `secondaryIdentifier` text,
  KEY `DB_ID` (`DB_ID`),
  FULLTEXT KEY `secondaryIdentifier` (`secondaryIdentifier`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `regulation`
--

DROP TABLE IF EXISTS `regulation`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `regulation` (
  `DB_ID` int(10) unsigned NOT NULL,
  `authored` int(10) unsigned DEFAULT NULL,
  `authored_class` varchar(64) DEFAULT NULL,
  `regulatedEntity` int(10) unsigned DEFAULT NULL,
  `regulatedEntity_class` varchar(64) DEFAULT NULL,
  `regulationType` int(10) unsigned DEFAULT NULL,
  `regulationType_class` varchar(64) DEFAULT NULL,
  `regulator` int(10) unsigned DEFAULT NULL,
  `regulator_class` varchar(64) DEFAULT NULL,
  `releaseDate` date DEFAULT NULL,
  PRIMARY KEY (`DB_ID`),
  KEY `authored` (`authored`),
  KEY `regulatedEntity` (`regulatedEntity`),
  KEY `regulationType` (`regulationType`),
  KEY `regulator` (`regulator`),
  KEY `releaseDate` (`releaseDate`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `regulation_2_containedinpathway`
--

DROP TABLE IF EXISTS `regulation_2_containedinpathway`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `regulation_2_containedinpathway` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `containedInPathway_rank` int(10) unsigned DEFAULT NULL,
  `containedInPathway` int(10) unsigned DEFAULT NULL,
  `containedInPathway_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `containedInPathway` (`containedInPathway`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `regulation_2_edited`
--

DROP TABLE IF EXISTS `regulation_2_edited`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `regulation_2_edited` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `edited_rank` int(10) unsigned DEFAULT NULL,
  `edited` int(10) unsigned DEFAULT NULL,
  `edited_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `edited` (`edited`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `regulation_2_figure`
--

DROP TABLE IF EXISTS `regulation_2_figure`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `regulation_2_figure` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `figure_rank` int(10) unsigned DEFAULT NULL,
  `figure` int(10) unsigned DEFAULT NULL,
  `figure_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `figure` (`figure`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `regulation_2_literaturereference`
--

DROP TABLE IF EXISTS `regulation_2_literaturereference`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `regulation_2_literaturereference` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `literatureReference_rank` int(10) unsigned DEFAULT NULL,
  `literatureReference` int(10) unsigned DEFAULT NULL,
  `literatureReference_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `literatureReference` (`literatureReference`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `regulation_2_name`
--

DROP TABLE IF EXISTS `regulation_2_name`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `regulation_2_name` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `name_rank` int(10) unsigned DEFAULT NULL,
  `name` text,
  KEY `DB_ID` (`DB_ID`),
  FULLTEXT KEY `name` (`name`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `regulation_2_reviewed`
--

DROP TABLE IF EXISTS `regulation_2_reviewed`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `regulation_2_reviewed` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `reviewed_rank` int(10) unsigned DEFAULT NULL,
  `reviewed` int(10) unsigned DEFAULT NULL,
  `reviewed_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `reviewed` (`reviewed`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `regulation_2_revised`
--

DROP TABLE IF EXISTS `regulation_2_revised`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `regulation_2_revised` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `revised_rank` int(10) unsigned DEFAULT NULL,
  `revised` int(10) unsigned DEFAULT NULL,
  `revised_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `revised` (`revised`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `regulation_2_summation`
--

DROP TABLE IF EXISTS `regulation_2_summation`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `regulation_2_summation` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `summation_rank` int(10) unsigned DEFAULT NULL,
  `summation` int(10) unsigned DEFAULT NULL,
  `summation_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `summation` (`summation`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `regulationtype`
--

DROP TABLE IF EXISTS `regulationtype`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `regulationtype` (
  `DB_ID` int(10) unsigned NOT NULL,
  PRIMARY KEY (`DB_ID`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `regulationtype_2_instanceof`
--

DROP TABLE IF EXISTS `regulationtype_2_instanceof`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `regulationtype_2_instanceof` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `instanceOf_rank` int(10) unsigned DEFAULT NULL,
  `instanceOf` int(10) unsigned DEFAULT NULL,
  `instanceOf_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `instanceOf` (`instanceOf`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `regulationtype_2_name`
--

DROP TABLE IF EXISTS `regulationtype_2_name`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `regulationtype_2_name` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `name_rank` int(10) unsigned DEFAULT NULL,
  `name` varchar(255) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `name` (`name`),
  FULLTEXT KEY `name_fulltext` (`name`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `replacedresidue`
--

DROP TABLE IF EXISTS `replacedresidue`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `replacedresidue` (
  `DB_ID` int(10) unsigned NOT NULL,
  `coordinate` int(10) DEFAULT NULL,
  PRIMARY KEY (`DB_ID`),
  KEY `coordinate` (`coordinate`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `replacedresidue_2_psimod`
--

DROP TABLE IF EXISTS `replacedresidue_2_psimod`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `replacedresidue_2_psimod` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `psiMod_rank` int(10) unsigned DEFAULT NULL,
  `psiMod` int(10) unsigned DEFAULT NULL,
  `psiMod_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `psiMod` (`psiMod`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `requirement`
--

DROP TABLE IF EXISTS `requirement`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `requirement` (
  `DB_ID` int(10) unsigned NOT NULL,
  PRIMARY KEY (`DB_ID`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `sequenceontology`
--

DROP TABLE IF EXISTS `sequenceontology`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `sequenceontology` (
  `DB_ID` int(10) unsigned NOT NULL,
  PRIMARY KEY (`DB_ID`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `simpleentity`
--

DROP TABLE IF EXISTS `simpleentity`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `simpleentity` (
  `DB_ID` int(10) unsigned NOT NULL,
  `referenceEntity` int(10) unsigned DEFAULT NULL,
  `referenceEntity_class` varchar(64) DEFAULT NULL,
  `species` int(10) unsigned DEFAULT NULL,
  `species_class` varchar(64) DEFAULT NULL,
  PRIMARY KEY (`DB_ID`),
  KEY `referenceEntity` (`referenceEntity`),
  KEY `species` (`species`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `species`
--

DROP TABLE IF EXISTS `species`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `species` (
  `DB_ID` int(10) unsigned NOT NULL,
  PRIMARY KEY (`DB_ID`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `species_2_figure`
--

DROP TABLE IF EXISTS `species_2_figure`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `species_2_figure` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `figure_rank` int(10) unsigned DEFAULT NULL,
  `figure` int(10) unsigned DEFAULT NULL,
  `figure_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `figure` (`figure`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `stableidentifier`
--

DROP TABLE IF EXISTS `stableidentifier`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `stableidentifier` (
  `DB_ID` int(10) unsigned NOT NULL,
  `identifier` text,
  `identifierVersion` text,
  `oldIdentifier` text,
  `oldIdentifierVersion` text,
  `referenceDatabase` int(10) unsigned DEFAULT NULL,
  `referenceDatabase_class` varchar(64) DEFAULT NULL,
  PRIMARY KEY (`DB_ID`),
  KEY `referenceDatabase` (`referenceDatabase`),
  FULLTEXT KEY `identifier` (`identifier`),
  FULLTEXT KEY `identifierVersion` (`identifierVersion`),
  FULLTEXT KEY `oldIdentifier` (`oldIdentifier`),
  FULLTEXT KEY `oldIdentifierVersion` (`oldIdentifierVersion`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `summation`
--

DROP TABLE IF EXISTS `summation`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `summation` (
  `DB_ID` int(10) unsigned NOT NULL,
  `text` text,
  PRIMARY KEY (`DB_ID`),
  FULLTEXT KEY `text` (`text`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `summation_2_literaturereference`
--

DROP TABLE IF EXISTS `summation_2_literaturereference`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `summation_2_literaturereference` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `literatureReference_rank` int(10) unsigned DEFAULT NULL,
  `literatureReference` int(10) unsigned DEFAULT NULL,
  `literatureReference_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `literatureReference` (`literatureReference`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `taxon`
--

DROP TABLE IF EXISTS `taxon`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `taxon` (
  `DB_ID` int(10) unsigned NOT NULL,
  `superTaxon` int(10) unsigned DEFAULT NULL,
  `superTaxon_class` varchar(64) DEFAULT NULL,
  PRIMARY KEY (`DB_ID`),
  KEY `superTaxon` (`superTaxon`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `taxon_2_crossreference`
--

DROP TABLE IF EXISTS `taxon_2_crossreference`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `taxon_2_crossreference` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `crossReference_rank` int(10) unsigned DEFAULT NULL,
  `crossReference` int(10) unsigned DEFAULT NULL,
  `crossReference_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `crossReference` (`crossReference`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `taxon_2_name`
--

DROP TABLE IF EXISTS `taxon_2_name`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `taxon_2_name` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `name_rank` int(10) unsigned DEFAULT NULL,
  `name` varchar(255) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `name` (`name`),
  FULLTEXT KEY `name_fulltext` (`name`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `translationalmodification`
--

DROP TABLE IF EXISTS `translationalmodification`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `translationalmodification` (
  `DB_ID` int(10) unsigned NOT NULL,
  `coordinate` int(10) DEFAULT NULL,
  `psiMod` int(10) unsigned DEFAULT NULL,
  `psiMod_class` varchar(64) DEFAULT NULL,
  PRIMARY KEY (`DB_ID`),
  KEY `coordinate` (`coordinate`),
  KEY `psiMod` (`psiMod`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `url`
--

DROP TABLE IF EXISTS `url`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `url` (
  `DB_ID` int(10) unsigned NOT NULL,
  `uniformResourceLocator` text,
  PRIMARY KEY (`DB_ID`),
  FULLTEXT KEY `uniformResourceLocator` (`uniformResourceLocator`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `vertex`
--

DROP TABLE IF EXISTS `vertex`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `vertex` (
  `DB_ID` int(10) unsigned NOT NULL,
  `height` int(10) DEFAULT NULL,
  `pathwayDiagram` int(10) unsigned DEFAULT NULL,
  `pathwayDiagram_class` varchar(64) DEFAULT NULL,
  `representedInstance` int(10) unsigned DEFAULT NULL,
  `representedInstance_class` varchar(64) DEFAULT NULL,
  `width` int(10) DEFAULT NULL,
  `x` int(10) DEFAULT NULL,
  `y` int(10) DEFAULT NULL,
  PRIMARY KEY (`DB_ID`),
  KEY `height` (`height`),
  KEY `pathwayDiagram` (`pathwayDiagram`),
  KEY `representedInstance` (`representedInstance`),
  KEY `width` (`width`),
  KEY `x` (`x`),
  KEY `y` (`y`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `vertexsearchableterm`
--

DROP TABLE IF EXISTS `vertexsearchableterm`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `vertexsearchableterm` (
  `DB_ID` int(10) unsigned NOT NULL,
  `providerCount` int(10) DEFAULT NULL,
  `searchableTerm` varchar(255) DEFAULT NULL,
  `species` int(10) unsigned DEFAULT NULL,
  `species_class` varchar(64) DEFAULT NULL,
  `vertexCount` int(10) DEFAULT NULL,
  PRIMARY KEY (`DB_ID`),
  KEY `providerCount` (`providerCount`),
  KEY `searchableTerm` (`searchableTerm`),
  KEY `species` (`species`),
  KEY `vertexCount` (`vertexCount`),
  FULLTEXT KEY `searchableTerm_fulltext` (`searchableTerm`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `vertexsearchableterm_2_termprovider`
--

DROP TABLE IF EXISTS `vertexsearchableterm_2_termprovider`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `vertexsearchableterm_2_termprovider` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `termProvider_rank` int(10) unsigned DEFAULT NULL,
  `termProvider` int(10) unsigned DEFAULT NULL,
  `termProvider_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `termProvider` (`termProvider`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `vertexsearchableterm_2_vertex`
--

DROP TABLE IF EXISTS `vertexsearchableterm_2_vertex`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `vertexsearchableterm_2_vertex` (
  `DB_ID` int(10) unsigned DEFAULT NULL,
  `vertex_rank` int(10) unsigned DEFAULT NULL,
  `vertex` int(10) unsigned DEFAULT NULL,
  `vertex_class` varchar(64) DEFAULT NULL,
  KEY `DB_ID` (`DB_ID`),
  KEY `vertex` (`vertex`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping events for database 'gk_current'
--

--
-- Dumping routines for database 'gk_current'
--
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2017-03-29 21:34:14
