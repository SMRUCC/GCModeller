---
title: spot
---

# spot
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `spot`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `spot` (
 `WID` bigint(20) NOT NULL,
 `SpotId` varchar(25) DEFAULT NULL,
 `MolecularWeightEst` float DEFAULT NULL,
 `PIEst` varchar(50) DEFAULT NULL,
 `DatasetWID` bigint(20) NOT NULL,
 PRIMARY KEY (`WID`),
 KEY `FK_Spot` (`DatasetWID`),
 CONSTRAINT `FK_Spot` FOREIGN KEY (`DatasetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




