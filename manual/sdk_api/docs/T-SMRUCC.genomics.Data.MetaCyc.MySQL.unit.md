---
title: unit
---

# unit
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `unit`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `unit` (
 `WID` bigint(20) NOT NULL,
 `DataSetWID` bigint(20) NOT NULL,
 `MAGEClass` varchar(100) NOT NULL,
 `UnitName` varchar(255) DEFAULT NULL,
 `UnitNameCV` varchar(25) DEFAULT NULL,
 `UnitNameCV2` varchar(25) DEFAULT NULL,
 `UnitNameCV3` varchar(25) DEFAULT NULL,
 `UnitNameCV4` varchar(25) DEFAULT NULL,
 `UnitNameCV5` varchar(25) DEFAULT NULL,
 `UnitNameCV6` varchar(25) DEFAULT NULL,
 `UnitNameCV7` varchar(25) DEFAULT NULL,
 PRIMARY KEY (`WID`),
 KEY `FK_Unit1` (`DataSetWID`),
 CONSTRAINT `FK_Unit1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




