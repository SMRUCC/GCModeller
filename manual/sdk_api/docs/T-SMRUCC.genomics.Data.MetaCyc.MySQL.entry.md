---
title: entry
---

# entry
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `entry`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `entry` (
 `OtherWID` bigint(20) NOT NULL,
 `InsertDate` datetime NOT NULL,
 `CreationDate` datetime DEFAULT NULL,
 `ModifiedDate` datetime DEFAULT NULL,
 `LoadError` char(1) NOT NULL,
 `LineNumber` int(11) DEFAULT NULL,
 `ErrorMessage` text,
 `DatasetWID` bigint(20) NOT NULL,
 PRIMARY KEY (`OtherWID`),
 KEY `FK_Entry` (`DatasetWID`),
 CONSTRAINT `FK_Entry` FOREIGN KEY (`DatasetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




