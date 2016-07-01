CREATE DATABASE  IF NOT EXISTS `ceg` /*!40100 DEFAULT CHARACTER SET utf8 */;
USE `ceg`;
-- MySQL dump 10.13  Distrib 5.6.13, for Win32 (x86)
--
-- Host: localhost    Database: ceg
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
-- Table structure for table `aa_seq`
--

DROP TABLE IF EXISTS `aa_seq`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `aa_seq` (
  `gid` varchar(25) DEFAULT NULL,
  `aalength` varchar(8) DEFAULT NULL,
  `aaseq` longtext
) ENGINE=MyISAM AUTO_INCREMENT=7687 DEFAULT CHARSET=gb2312;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `annotation`
--

DROP TABLE IF EXISTS `annotation`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `annotation` (
  `gid` varchar(25) DEFAULT NULL,
  `Gene_Name` varchar(80) DEFAULT NULL,
  `fundescrp` varchar(255) DEFAULT NULL
) ENGINE=MyISAM AUTO_INCREMENT=11862 DEFAULT CHARSET=gb2312;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `ceg_base`
--

DROP TABLE IF EXISTS `ceg_base`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `ceg_base` (
  `access_num` varchar(255) DEFAULT NULL,
  `koid` varchar(255) DEFAULT NULL,
  `cogid` varchar(255) NOT NULL,
  `description` varchar(255) NOT NULL,
  `ec` varchar(100) NOT NULL
) ENGINE=MyISAM DEFAULT CHARSET=gb2312;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `ceg_core`
--

DROP TABLE IF EXISTS `ceg_core`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `ceg_core` (
  `access_num` varchar(50) DEFAULT NULL,
  `gid` varchar(25) NOT NULL DEFAULT '',
  `koid` varchar(30) DEFAULT NULL,
  `cogid` varchar(255) NOT NULL,
  `hprd_nid` varchar(12) DEFAULT NULL,
  `nhit_ref` varchar(12) DEFAULT NULL,
  `nevalue` varchar(12) DEFAULT NULL,
  `nscore` varchar(20) DEFAULT NULL,
  `hprd_aid` varchar(20) NOT NULL,
  `ahit_ref` varchar(20) NOT NULL,
  `aevalue` varchar(20) NOT NULL,
  `ascore` varchar(20) NOT NULL,
  `degid` varchar(15) NOT NULL,
  `organismid` int(4) NOT NULL,
  PRIMARY KEY (`gid`),
  FULLTEXT KEY `gid` (`gid`,`access_num`)
) ENGINE=MyISAM AUTO_INCREMENT=7687 DEFAULT CHARSET=gb2312;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `clade`
--

DROP TABLE IF EXISTS `clade`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `clade` (
  `oganismid` int(4) NOT NULL,
  `phylum` varchar(100) DEFAULT NULL,
  `abbr` varchar(100) DEFAULT NULL,
  `class` varchar(100) DEFAULT NULL,
  `order` varchar(100) NOT NULL,
  `family` varchar(100) DEFAULT NULL,
  `genus` text NOT NULL,
  PRIMARY KEY (`oganismid`)
) ENGINE=MyISAM AUTO_INCREMENT=7687 DEFAULT CHARSET=gb2312;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `na_seq`
--

DROP TABLE IF EXISTS `na_seq`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `na_seq` (
  `gid` varchar(25) DEFAULT NULL,
  `aalength` varchar(8) DEFAULT NULL,
  `aaseq` longtext
) ENGINE=MyISAM AUTO_INCREMENT=7687 DEFAULT CHARSET=gb2312;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `reference`
--

DROP TABLE IF EXISTS `reference`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `reference` (
  `oganismid` int(4) DEFAULT NULL,
  `abbr` varchar(5) NOT NULL,
  `oganism` varchar(255) DEFAULT NULL,
  `pubmedid` varchar(20) DEFAULT NULL,
  `pub_title` text NOT NULL
) ENGINE=MyISAM AUTO_INCREMENT=7687 DEFAULT CHARSET=gb2312;
/*!40101 SET character_set_client = @saved_cs_client */;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2015-10-09  2:15:39
