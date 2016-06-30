CREATE DATABASE  IF NOT EXISTS `uniprot` /*!40100 DEFAULT CHARACTER SET utf8 */;
USE `uniprot`;
-- MySQL dump 10.13  Distrib 5.6.13, for Win32 (x86)
--
-- Host: localhost    Database: uniprot
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
-- Table structure for table `id_mappings`
--

DROP TABLE IF EXISTS `id_mappings`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `id_mappings` (
  `UniProtKB_AC` int(11) NOT NULL,
  `UniProtKB_ID` varchar(45) DEFAULT NULL,
  `GeneID_EntrezGene` varchar(45) DEFAULT NULL,
  `RefSeq` varchar(45) DEFAULT NULL,
  `GI` varchar(45) DEFAULT NULL,
  `pdb` varchar(45) DEFAULT NULL,
  `go` varchar(45) DEFAULT NULL,
  `UniRef100` varchar(45) DEFAULT NULL,
  `UniRef90` varchar(45) DEFAULT NULL,
  `UniRef50` varchar(45) DEFAULT NULL,
  `UniParc` varchar(45) DEFAULT NULL,
  `pir` varchar(45) DEFAULT NULL,
  `NCBI_Taxon` varchar(45) DEFAULT NULL,
  `MIM` varchar(45) DEFAULT NULL,
  `UniGene` varchar(45) DEFAULT NULL,
  `PubMed` varchar(45) DEFAULT NULL,
  `EMBL` varchar(45) DEFAULT NULL,
  `EMBL_CDS` varchar(45) DEFAULT NULL,
  `Ensembl` varchar(45) DEFAULT NULL,
  `Ensembl_TRS` varchar(45) DEFAULT NULL,
  `Ensembl_PRO` varchar(45) DEFAULT NULL,
  `Additional_PubMed` text,
  PRIMARY KEY (`UniProtKB_AC`),
  UNIQUE KEY `UniProtKB_AC_UNIQUE` (`UniProtKB_AC`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `uniprotkb`
--

DROP TABLE IF EXISTS `uniprotkb`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `uniprotkb` (
  `uniprot_id` varchar(45) NOT NULL COMMENT 'UniqueIdentifier Is the primary accession number of the UniProtKB entry.',
  `entry_name` varchar(45) DEFAULT NULL COMMENT 'EntryName Is the entry name of the UniProtKB entry.',
  `orgnsm_sp_name` tinytext COMMENT 'OrganismName Is the scientific name of the organism of the UniProtKB entry.',
  `gn` varchar(45) DEFAULT NULL COMMENT 'GeneName Is the first gene name of the UniProtKB entry. If there Is no gene name, OrderedLocusName Or ORFname, the GN field Is Not listed.',
  `pe` varchar(45) DEFAULT NULL COMMENT 'ProteinExistence Is the numerical value describing the evidence for the existence of the protein.',
  `sv` varchar(45) DEFAULT NULL COMMENT 'SequenceVersion Is the version number of the sequence.',
  `prot_name` tinytext COMMENT 'ProteinName Is the recommended name of the UniProtKB entry as annotated in the RecName field. For UniProtKB/TrEMBL entries without a RecName field, the SubName field Is used. In case of multiple SubNames, the first one Is used. The ''precursor'' attribute is excluded, ''Fragment'' is included with the name if applicable.',
  `length` int(11) DEFAULT NULL COMMENT 'length of the protein sequence',
  `sequence` text COMMENT 'protein sequence',
  PRIMARY KEY (`uniprot_id`),
  UNIQUE KEY `uniprot_id_UNIQUE` (`uniprot_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2015-12-03 20:59:22
