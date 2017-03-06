CREATE DATABASE  IF NOT EXISTS `jp_kegg2` /*!40100 DEFAULT CHARACTER SET utf8 */;
USE `jp_kegg2`;
-- MySQL dump 10.13  Distrib 5.7.12, for Win64 (x86_64)
--
-- Host: localhost    Database: jp_kegg2
-- ------------------------------------------------------
-- Server version	5.7.17-log

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
-- Dumping data for table `disease`
--

LOCK TABLES `disease` WRITE;
/*!40000 ALTER TABLE `disease` DISABLE KEYS */;
/*!40000 ALTER TABLE `disease` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Dumping data for table `genes`
--

LOCK TABLES `genes` WRITE;
/*!40000 ALTER TABLE `genes` DISABLE KEYS */;
/*!40000 ALTER TABLE `genes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Dumping data for table `module`
--

LOCK TABLES `module` WRITE;
/*!40000 ALTER TABLE `module` DISABLE KEYS */;
/*!40000 ALTER TABLE `module` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Dumping data for table `orthology`
--

LOCK TABLES `orthology` WRITE;
/*!40000 ALTER TABLE `orthology` DISABLE KEYS */;
/*!40000 ALTER TABLE `orthology` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Dumping data for table `orthology_diseases`
--

LOCK TABLES `orthology_diseases` WRITE;
/*!40000 ALTER TABLE `orthology_diseases` DISABLE KEYS */;
/*!40000 ALTER TABLE `orthology_diseases` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Dumping data for table `orthology_genes`
--

LOCK TABLES `orthology_genes` WRITE;
/*!40000 ALTER TABLE `orthology_genes` DISABLE KEYS */;
/*!40000 ALTER TABLE `orthology_genes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Dumping data for table `orthology_modules`
--

LOCK TABLES `orthology_modules` WRITE;
/*!40000 ALTER TABLE `orthology_modules` DISABLE KEYS */;
/*!40000 ALTER TABLE `orthology_modules` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Dumping data for table `orthology_pathways`
--

LOCK TABLES `orthology_pathways` WRITE;
/*!40000 ALTER TABLE `orthology_pathways` DISABLE KEYS */;
/*!40000 ALTER TABLE `orthology_pathways` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Dumping data for table `orthology_references`
--

LOCK TABLES `orthology_references` WRITE;
/*!40000 ALTER TABLE `orthology_references` DISABLE KEYS */;
/*!40000 ALTER TABLE `orthology_references` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Dumping data for table `pathway`
--

LOCK TABLES `pathway` WRITE;
/*!40000 ALTER TABLE `pathway` DISABLE KEYS */;
/*!40000 ALTER TABLE `pathway` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Dumping data for table `reference`
--

LOCK TABLES `reference` WRITE;
/*!40000 ALTER TABLE `reference` DISABLE KEYS */;
/*!40000 ALTER TABLE `reference` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Dumping data for table `xref_ko2cog`
--

LOCK TABLES `xref_ko2cog` WRITE;
/*!40000 ALTER TABLE `xref_ko2cog` DISABLE KEYS */;
/*!40000 ALTER TABLE `xref_ko2cog` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Dumping data for table `xref_ko2go`
--

LOCK TABLES `xref_ko2go` WRITE;
/*!40000 ALTER TABLE `xref_ko2go` DISABLE KEYS */;
/*!40000 ALTER TABLE `xref_ko2go` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Dumping data for table `xref_ko2rn`
--

LOCK TABLES `xref_ko2rn` WRITE;
/*!40000 ALTER TABLE `xref_ko2rn` DISABLE KEYS */;
/*!40000 ALTER TABLE `xref_ko2rn` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Dumping events for database 'jp_kegg2'
--

--
-- Dumping routines for database 'jp_kegg2'
--
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2017-03-06 15:28:41
