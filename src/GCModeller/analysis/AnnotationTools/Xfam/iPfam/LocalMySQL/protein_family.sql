-- MySQL dump 10.11
--
-- Host: localhost    Database: ipfam_1_0
-- ------------------------------------------------------
-- Server version	5.5.14-log

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `protein_family`
--

DROP TABLE IF EXISTS `protein_family`;
SET @saved_cs_client     = @@character_set_client;
SET character_set_client = utf8;
CREATE TABLE `protein_family` (
  `auto_prot_fam` int(11) NOT NULL AUTO_INCREMENT,
  `accession` varchar(45) DEFAULT NULL,
  `identifier` varchar(45) DEFAULT NULL,
  `description` text,
  `comment` longtext,
  `type` enum('family','domain','motif','repeat') DEFAULT NULL,
  `source_db` enum('pfama') DEFAULT NULL,
  `colour` varchar(7) DEFAULT NULL,
  `number_fam_int` int(5) DEFAULT '0',
  `number_lig_int` int(5) DEFAULT '0',
  `number_pdbs` int(5) DEFAULT '0',
  PRIMARY KEY (`auto_prot_fam`),
  UNIQUE KEY `accession_UNIQUE` (`accession`,`source_db`),
  UNIQUE KEY `identifier_UNIQUE` (`identifier`,`source_db`)
) ENGINE=InnoDB AUTO_INCREMENT=29663 DEFAULT CHARSET=latin1;
SET character_set_client = @saved_cs_client;

/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2013-09-09 15:36:55
