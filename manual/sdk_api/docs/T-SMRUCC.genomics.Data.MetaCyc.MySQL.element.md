---
title: element
---

# element
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `element`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `element` (
 `WID` bigint(20) NOT NULL,
 `Name` varchar(15) NOT NULL,
 `ElementSymbol` varchar(2) NOT NULL,
 `AtomicWeight` float NOT NULL,
 `AtomicNumber` smallint(6) NOT NULL,
 PRIMARY KEY (`WID`)
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




