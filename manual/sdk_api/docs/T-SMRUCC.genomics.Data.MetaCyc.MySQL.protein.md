---
title: protein
---

# protein
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `protein`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `protein` (
 `WID` bigint(20) NOT NULL,
 `Name` text,
 `AASequence` longtext,
 `Length` int(11) DEFAULT NULL,
 `LengthApproximate` varchar(10) DEFAULT NULL,
 `Charge` smallint(6) DEFAULT NULL,
 `Fragment` char(1) DEFAULT NULL,
 `MolecularWeightCalc` float DEFAULT NULL,
 `MolecularWeightExp` float DEFAULT NULL,
 `PICalc` varchar(50) DEFAULT NULL,
 `PIExp` varchar(50) DEFAULT NULL,
 `DataSetWID` bigint(20) NOT NULL,
 PRIMARY KEY (`WID`),
 KEY `PROTEIN_DWID` (`DataSetWID`),
 CONSTRAINT `FK_Protein` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




