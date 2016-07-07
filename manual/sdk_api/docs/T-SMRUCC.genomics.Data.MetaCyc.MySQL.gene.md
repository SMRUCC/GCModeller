---
title: gene
---

# gene
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `gene`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `gene` (
 `WID` bigint(20) NOT NULL,
 `Name` varchar(255) DEFAULT NULL,
 `NucleicAcidWID` bigint(20) DEFAULT NULL,
 `SubsequenceWID` bigint(20) DEFAULT NULL,
 `Type` varchar(100) DEFAULT NULL,
 `GenomeID` varchar(35) DEFAULT NULL,
 `CodingRegionStart` int(11) DEFAULT NULL,
 `CodingRegionEnd` int(11) DEFAULT NULL,
 `CodingRegionStartApproximate` varchar(10) DEFAULT NULL,
 `CodingRegionEndApproximate` varchar(10) DEFAULT NULL,
 `Direction` varchar(25) DEFAULT NULL,
 `Interrupted` char(1) DEFAULT NULL,
 `DataSetWID` bigint(20) NOT NULL,
 PRIMARY KEY (`WID`),
 KEY `GENE_DATASETWID` (`DataSetWID`),
 KEY `FK_Gene1` (`NucleicAcidWID`),
 CONSTRAINT `FK_Gene1` FOREIGN KEY (`NucleicAcidWID`) REFERENCES `nucleicacid` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_Gene2` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




