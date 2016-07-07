---
title: position_
---

# position_
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `position_`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `position_` (
 `WID` bigint(20) NOT NULL,
 `DataSetWID` bigint(20) NOT NULL,
 `X` float DEFAULT NULL,
 `Y` float DEFAULT NULL,
 `Position_DistanceUnit` bigint(20) DEFAULT NULL,
 PRIMARY KEY (`WID`),
 KEY `FK_Position_1` (`DataSetWID`),
 KEY `FK_Position_2` (`Position_DistanceUnit`),
 CONSTRAINT `FK_Position_1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_Position_2` FOREIGN KEY (`Position_DistanceUnit`) REFERENCES `unit` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




