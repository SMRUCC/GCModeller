---
title: dbid
---

# dbid
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `dbid`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `dbid` (
 `OtherWID` bigint(20) NOT NULL,
 `XID` varchar(150) NOT NULL,
 `Type` varchar(20) DEFAULT NULL,
 `Version` varchar(10) DEFAULT NULL,
 KEY `DBID_XID_OTHERWID` (`XID`,`OtherWID`),
 KEY `DBID_OTHERWID` (`OtherWID`)
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




