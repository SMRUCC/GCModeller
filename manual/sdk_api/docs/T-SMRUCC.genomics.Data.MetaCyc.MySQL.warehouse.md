---
title: warehouse
---

# warehouse
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `warehouse`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `warehouse` (
 `Version` decimal(6,3) NOT NULL,
 `LoadDate` datetime NOT NULL,
 `MaxSpecialWID` bigint(20) NOT NULL,
 `MaxReservedWID` bigint(20) NOT NULL,
 `Description` text
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




