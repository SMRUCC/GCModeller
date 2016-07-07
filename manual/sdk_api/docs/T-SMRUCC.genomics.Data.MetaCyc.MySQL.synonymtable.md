---
title: synonymtable
---

# synonymtable
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `synonymtable`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `synonymtable` (
 `OtherWID` bigint(20) NOT NULL,
 `Syn` varchar(255) NOT NULL,
 KEY `SYNONYM_OTHERWID_SYN` (`OtherWID`,`Syn`)
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




