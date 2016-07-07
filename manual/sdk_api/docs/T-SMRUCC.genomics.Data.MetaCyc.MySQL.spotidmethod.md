---
title: spotidmethod
---

# spotidmethod
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `spotidmethod`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `spotidmethod` (
 `WID` bigint(20) NOT NULL,
 `MethodName` varchar(100) NOT NULL,
 `MethodDesc` varchar(500) DEFAULT NULL,
 `MethodAbbrev` varchar(10) DEFAULT NULL,
 `DatasetWID` bigint(20) NOT NULL,
 PRIMARY KEY (`WID`),
 KEY `FK_SpotIdMethDataset` (`DatasetWID`),
 CONSTRAINT `FK_SpotIdMethDataset` FOREIGN KEY (`DatasetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




