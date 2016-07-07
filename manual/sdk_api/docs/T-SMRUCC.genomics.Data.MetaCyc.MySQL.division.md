---
title: division
---

# division
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `division`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `division` (
 `WID` bigint(20) NOT NULL,
 `Code` varchar(10) DEFAULT NULL,
 `Name` varchar(100) DEFAULT NULL,
 `DataSetWID` bigint(20) NOT NULL,
 PRIMARY KEY (`WID`),
 KEY `FK_Division` (`DataSetWID`),
 CONSTRAINT `FK_Division` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




