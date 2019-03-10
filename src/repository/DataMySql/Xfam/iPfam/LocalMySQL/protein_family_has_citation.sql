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
-- Table structure for table `protein_family_has_citation`
--

DROP TABLE IF EXISTS `protein_family_has_citation`;
SET @saved_cs_client     = @@character_set_client;
SET character_set_client = utf8;
CREATE TABLE `protein_family_has_citation` (
  `auto_prot_fam` int(11) NOT NULL,
  `pmid` int(10) unsigned NOT NULL,
  `order_added` int(4) NOT NULL,
  KEY `fk_protein_family_has_citation_protein_family1_idx` (`auto_prot_fam`),
  KEY `fk_protein_family_has_citation_citation1_idx` (`pmid`),
  CONSTRAINT `fk_protein_family_has_citation_citation1` FOREIGN KEY (`pmid`) REFERENCES `citation` (`pmid`) ON DELETE NO ACTION ON UPDATE CASCADE,
  CONSTRAINT `fk_protein_family_has_citation_protein_family1` FOREIGN KEY (`auto_prot_fam`) REFERENCES `protein_family` (`auto_prot_fam`) ON DELETE CASCADE ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
SET character_set_client = @saved_cs_client;

/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2013-09-08 14:05:20
