---
title: term
---

# term
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `term`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `term` (
 `WID` bigint(20) NOT NULL,
 `Name` varchar(255) NOT NULL,
 `Definition` text,
 `Hierarchical` char(1) DEFAULT NULL,
 `Root` char(1) DEFAULT NULL,
 `Obsolete` char(1) DEFAULT NULL,
 `DataSetWID` bigint(20) NOT NULL,
 PRIMARY KEY (`WID`)
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




