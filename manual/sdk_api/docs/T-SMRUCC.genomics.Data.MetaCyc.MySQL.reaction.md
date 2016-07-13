---
title: reaction
---

# reaction
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `reaction`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `reaction` (
 `WID` bigint(20) NOT NULL,
 `Name` varchar(250) DEFAULT NULL,
 `DeltaG` varchar(50) DEFAULT NULL,
 `ECNumber` varchar(50) DEFAULT NULL,
 `ECNumberProposed` varchar(50) DEFAULT NULL,
 `Spontaneous` char(1) DEFAULT NULL,
 `DataSetWID` bigint(20) NOT NULL,
 PRIMARY KEY (`WID`),
 KEY `REACTION_DWID` (`DataSetWID`),
 CONSTRAINT `FK_Reaction` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




