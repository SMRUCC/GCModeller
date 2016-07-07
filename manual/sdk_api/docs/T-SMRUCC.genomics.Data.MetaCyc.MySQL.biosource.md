---
title: biosource
---

# biosource
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `biosource`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `biosource` (
 `WID` bigint(20) NOT NULL,
 `MAGEClass` varchar(100) DEFAULT NULL,
 `TaxonWID` bigint(20) DEFAULT NULL,
 `Name` varchar(200) DEFAULT NULL,
 `Strain` varchar(220) DEFAULT NULL,
 `Organ` varchar(50) DEFAULT NULL,
 `Organelle` varchar(50) DEFAULT NULL,
 `Tissue` varchar(100) DEFAULT NULL,
 `CellType` varchar(50) DEFAULT NULL,
 `CellLine` varchar(50) DEFAULT NULL,
 `ATCCId` varchar(50) DEFAULT NULL,
 `Diseased` char(1) DEFAULT NULL,
 `Disease` varchar(250) DEFAULT NULL,
 `DevelopmentStage` varchar(50) DEFAULT NULL,
 `Sex` varchar(15) DEFAULT NULL,
 `DataSetWID` bigint(20) NOT NULL,
 PRIMARY KEY (`WID`),
 KEY `FK_BioSource1` (`TaxonWID`),
 KEY `FK_BioSource2` (`DataSetWID`),
 CONSTRAINT `FK_BioSource1` FOREIGN KEY (`TaxonWID`) REFERENCES `taxon` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_BioSource2` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




