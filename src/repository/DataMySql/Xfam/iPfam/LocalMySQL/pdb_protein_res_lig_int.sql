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
-- Table structure for table `pdb_protein_res_lig_int`
--

DROP TABLE IF EXISTS `pdb_protein_res_lig_int`;
SET @saved_cs_client     = @@character_set_client;
SET character_set_client = utf8;
CREATE TABLE `pdb_protein_res_lig_int` (
  `auto_pdb_protein_res_lig_int` bigint(20) NOT NULL AUTO_INCREMENT,
  `pdb_id` varchar(4) NOT NULL,
  `auto_reg_lig_int` bigint(20) NOT NULL,
  `auto_ligand` int(8) NOT NULL,
  `residue` int(10) DEFAULT NULL,
  `ligand` int(10) DEFAULT NULL,
  `bond` enum('vdw','hbback','hbside','electro','covalent') DEFAULT NULL,
  PRIMARY KEY (`auto_pdb_protein_res_lig_int`),
  KEY `fk_pdb_protein_red_int_copy1_pdb_protein_region_lig_int1_idx` (`auto_reg_lig_int`),
  KEY `fk_pdb_protein_red_int_copy_ligand1_idx` (`auto_ligand`),
  CONSTRAINT `fk_pdb_protein_reg_int_copy1_pdb_protein_region_lig_int1` FOREIGN KEY (`auto_reg_lig_int`) REFERENCES `pdb_protein_region_lig_int` (`auto_reg_lig_int`) ON DELETE CASCADE ON UPDATE NO ACTION,
  CONSTRAINT `fk_pdb_protein_reg_int_copy_ligand1` FOREIGN KEY (`auto_ligand`) REFERENCES `ligand` (`auto_ligand`) ON DELETE CASCADE ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=1597354 DEFAULT CHARSET=latin1;
SET character_set_client = @saved_cs_client;

/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2013-09-08 14:01:46
