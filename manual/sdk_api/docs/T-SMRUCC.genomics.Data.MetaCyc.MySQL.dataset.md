---
title: dataset
---

# dataset
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `dataset`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `dataset` (
 `WID` bigint(20) NOT NULL,
 `Name` varchar(255) NOT NULL,
 `Version` varchar(50) DEFAULT NULL,
 `ReleaseDate` datetime DEFAULT NULL,
 `LoadDate` datetime NOT NULL,
 `ChangeDate` datetime DEFAULT NULL,
 `HomeURL` varchar(255) DEFAULT NULL,
 `QueryURL` varchar(255) DEFAULT NULL,
 `LoadedBy` varchar(255) DEFAULT NULL,
 `Application` varchar(255) DEFAULT NULL,
 `ApplicationVersion` varchar(255) DEFAULT NULL,
 PRIMARY KEY (`WID`)
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




