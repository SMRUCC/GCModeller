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
-- Table structure for table `ligand`
--

DROP TABLE IF EXISTS `ligand`;
SET @saved_cs_client     = @@character_set_client;
SET character_set_client = utf8;
CREATE TABLE `ligand` (
  `auto_ligand` int(8) NOT NULL AUTO_INCREMENT,
  `pdb_id` varchar(4) NOT NULL,
  `ligand_id` varchar(3) NOT NULL,
  `chain` varchar(4) NOT NULL,
  `residue` int(11) DEFAULT NULL,
  `atom_start` int(11) DEFAULT NULL,
  `atom_end` int(11) DEFAULT NULL,
  PRIMARY KEY (`auto_ligand`),
  KEY `fk_ligand_ligand_chemistry1_idx` (`ligand_id`),
  KEY `fk_ligand_pdb_entry1_idx` (`pdb_id`),
  CONSTRAINT `fk_ligand_ligand_chemistry1` FOREIGN KEY (`ligand_id`) REFERENCES `ligand_chemistry` (`ligand_id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_ligand_pdb_entry1` FOREIGN KEY (`pdb_id`) REFERENCES `pdb_entry` (`pdb_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=1001098 DEFAULT CHARSET=latin1;
SET character_set_client = @saved_cs_client;

/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2013-09-08 14:00:35
