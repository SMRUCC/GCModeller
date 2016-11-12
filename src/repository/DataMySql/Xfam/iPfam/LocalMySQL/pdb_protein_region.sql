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
-- Table structure for table `pdb_protein_region`
--

DROP TABLE IF EXISTS `pdb_protein_region`;
SET @saved_cs_client     = @@character_set_client;
SET character_set_client = utf8;
CREATE TABLE `pdb_protein_region` (
  `region_id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `auto_prot_fam` int(11) NOT NULL,
  `prot_fam_acc` varchar(45) NOT NULL,
  `pdb_id` varchar(4) NOT NULL,
  `chain` varchar(1) DEFAULT NULL,
  `start` int(11) NOT NULL,
  `start_icode` varchar(1) DEFAULT NULL,
  `end` int(11) NOT NULL,
  `end_icode` varchar(1) DEFAULT NULL,
  `region_source_db` varchar(12) NOT NULL,
  PRIMARY KEY (`region_id`),
  KEY `region_protein_accession_start_end_Idx` (`prot_fam_acc`,`start`,`end`),
  KEY `fk_region_proteins` (`prot_fam_acc`),
  KEY `fk_pdb_protein_region_pdb_entry1_idx` (`pdb_id`),
  KEY `fk_pdb_protein_region_protein_family1_idx1` (`auto_prot_fam`),
  CONSTRAINT `fk_pdb_protein_region_pdb_entry1` FOREIGN KEY (`pdb_id`) REFERENCES `pdb_entry` (`pdb_id`) ON DELETE CASCADE ON UPDATE NO ACTION,
  CONSTRAINT `fk_pdb_protein_region_protein_family1` FOREIGN KEY (`auto_prot_fam`) REFERENCES `protein_family` (`auto_prot_fam`) ON DELETE CASCADE ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=1215166 DEFAULT CHARSET=latin1;
SET character_set_client = @saved_cs_client;

/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2013-09-08 14:01:17
