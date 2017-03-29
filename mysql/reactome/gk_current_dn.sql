CREATE DATABASE  IF NOT EXISTS `gk_current_dn` /*!40100 DEFAULT CHARACTER SET utf8 */;
USE `gk_current_dn`;
-- MySQL dump 10.13  Distrib 5.7.9, for Win64 (x86_64)
--
-- Host: localhost    Database: gk_current_dn
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
-- Table structure for table `id_to_externalidentifier`
--

DROP TABLE IF EXISTS `id_to_externalidentifier`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `id_to_externalidentifier` (
  `id` int(32) NOT NULL DEFAULT '0',
  `referenceDatabase` varchar(255) NOT NULL DEFAULT '',
  `externalIdentifier` varchar(32) NOT NULL DEFAULT '',
  `description` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`id`,`referenceDatabase`,`externalIdentifier`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `pathway`
--

DROP TABLE IF EXISTS `pathway`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `pathway` (
  `id` int(32) NOT NULL,
  `displayName` varchar(255) NOT NULL,
  `species` varchar(255) NOT NULL,
  `stableId` varchar(32) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `stableId` (`stableId`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `pathway_to_reactionlikeevent`
--

DROP TABLE IF EXISTS `pathway_to_reactionlikeevent`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `pathway_to_reactionlikeevent` (
  `pathwayId` int(32) NOT NULL DEFAULT '0',
  `reactionLikeEventId` int(32) NOT NULL DEFAULT '0',
  PRIMARY KEY (`pathwayId`,`reactionLikeEventId`),
  KEY `reactionLikeEventId` (`reactionLikeEventId`),
  CONSTRAINT `Pathway_To_ReactionLikeEvent_ibfk_1` FOREIGN KEY (`pathwayId`) REFERENCES `pathway` (`id`),
  CONSTRAINT `Pathway_To_ReactionLikeEvent_ibfk_2` FOREIGN KEY (`reactionLikeEventId`) REFERENCES `reactionlikeevent` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `pathwayhierarchy`
--

DROP TABLE IF EXISTS `pathwayhierarchy`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `pathwayhierarchy` (
  `pathwayId` int(32) NOT NULL DEFAULT '0',
  `childPathwayId` int(32) NOT NULL DEFAULT '0',
  PRIMARY KEY (`pathwayId`,`childPathwayId`),
  CONSTRAINT `PathwayHierarchy_ibfk_1` FOREIGN KEY (`pathwayId`) REFERENCES `pathway` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `physicalentity`
--

DROP TABLE IF EXISTS `physicalentity`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `physicalentity` (
  `id` int(32) NOT NULL,
  `displayName` varchar(255) NOT NULL,
  `species` varchar(255) DEFAULT NULL,
  `class` varchar(255) NOT NULL,
  `stableId` varchar(32) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `stableId` (`stableId`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `physicalentityhierarchy`
--

DROP TABLE IF EXISTS `physicalentityhierarchy`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `physicalentityhierarchy` (
  `physicalEntityId` int(32) NOT NULL DEFAULT '0',
  `childPhysicalEntityId` int(32) NOT NULL DEFAULT '0',
  PRIMARY KEY (`physicalEntityId`,`childPhysicalEntityId`),
  CONSTRAINT `PhysicalEntityHierarchy_ibfk_1` FOREIGN KEY (`physicalEntityId`) REFERENCES `physicalentity` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `reactionlikeevent`
--

DROP TABLE IF EXISTS `reactionlikeevent`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `reactionlikeevent` (
  `id` int(32) NOT NULL,
  `displayName` varchar(255) NOT NULL,
  `species` varchar(255) NOT NULL,
  `class` varchar(255) NOT NULL,
  `stableId` varchar(32) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `stableId` (`stableId`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `reactionlikeevent_to_physicalentity`
--

DROP TABLE IF EXISTS `reactionlikeevent_to_physicalentity`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `reactionlikeevent_to_physicalentity` (
  `reactionLikeEventId` int(32) NOT NULL DEFAULT '0',
  `physicalEntityId` int(32) NOT NULL DEFAULT '0',
  PRIMARY KEY (`reactionLikeEventId`,`physicalEntityId`),
  KEY `physicalEntityId` (`physicalEntityId`),
  CONSTRAINT `ReactionLikeEvent_To_PhysicalEntity_ibfk_1` FOREIGN KEY (`reactionLikeEventId`) REFERENCES `reactionlikeevent` (`id`),
  CONSTRAINT `ReactionLikeEvent_To_PhysicalEntity_ibfk_2` FOREIGN KEY (`physicalEntityId`) REFERENCES `physicalentity` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping events for database 'gk_current_dn'
--

--
-- Dumping routines for database 'gk_current_dn'
--
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2017-03-29 21:34:53
