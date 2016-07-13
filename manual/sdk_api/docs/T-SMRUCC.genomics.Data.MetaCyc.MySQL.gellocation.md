---
title: gellocation
---

# gellocation
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `gellocation`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `gellocation` (
 `WID` bigint(20) NOT NULL,
 `SpotWID` bigint(20) NOT NULL,
 `Xcoord` float DEFAULT NULL,
 `Ycoord` float DEFAULT NULL,
 `refGel` varchar(1) DEFAULT NULL,
 `ExperimentWID` bigint(20) NOT NULL,
 `DatasetWID` bigint(20) NOT NULL,
 PRIMARY KEY (`WID`),
 KEY `FK_GelLocSpotWid` (`SpotWID`),
 KEY `FK_GelLocExp` (`ExperimentWID`),
 KEY `FK_GelLocDataset` (`DatasetWID`),
 CONSTRAINT `FK_GelLocDataset` FOREIGN KEY (`DatasetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_GelLocExp` FOREIGN KEY (`ExperimentWID`) REFERENCES `experiment` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_GelLocSpotWid` FOREIGN KEY (`SpotWID`) REFERENCES `spot` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




