---
title: taxon
---

# taxon
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `taxon`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `taxon` (
 `WID` bigint(20) NOT NULL,
 `ParentWID` bigint(20) DEFAULT NULL,
 `Name` varchar(100) DEFAULT NULL,
 `Rank` varchar(100) DEFAULT NULL,
 `DivisionWID` bigint(20) DEFAULT NULL,
 `InheritedDivision` char(1) DEFAULT NULL,
 `GencodeWID` bigint(20) DEFAULT NULL,
 `InheritedGencode` char(1) DEFAULT NULL,
 `MCGencodeWID` bigint(20) DEFAULT NULL,
 `InheritedMCGencode` char(1) DEFAULT NULL,
 `DataSetWID` bigint(20) NOT NULL,
 PRIMARY KEY (`WID`),
 KEY `FK_Taxon_Division` (`DivisionWID`),
 KEY `FK_Taxon_GeneticCode` (`GencodeWID`),
 KEY `FK_Taxon` (`DataSetWID`),
 CONSTRAINT `FK_Taxon` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_Taxon_Division` FOREIGN KEY (`DivisionWID`) REFERENCES `division` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_Taxon_GeneticCode` FOREIGN KEY (`GencodeWID`) REFERENCES `geneticcode` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




