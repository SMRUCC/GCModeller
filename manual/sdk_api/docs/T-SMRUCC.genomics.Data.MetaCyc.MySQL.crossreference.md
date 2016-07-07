---
title: crossreference
---

# crossreference
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `crossreference`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `crossreference` (
 `OtherWID` bigint(20) NOT NULL,
 `CrossWID` bigint(20) DEFAULT NULL,
 `XID` varchar(50) DEFAULT NULL,
 `Type` varchar(20) DEFAULT NULL,
 `Version` varchar(10) DEFAULT NULL,
 `Relationship` varchar(50) DEFAULT NULL,
 `DatabaseName` varchar(255) DEFAULT NULL
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




