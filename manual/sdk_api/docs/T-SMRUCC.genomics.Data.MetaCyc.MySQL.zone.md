---
title: zone
---

# zone
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `zone`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `zone` (
 `WID` bigint(20) NOT NULL,
 `DataSetWID` bigint(20) NOT NULL,
 `Identifier` varchar(255) DEFAULT NULL,
 `Name` varchar(255) DEFAULT NULL,
 `Row_` smallint(6) DEFAULT NULL,
 `Column_` smallint(6) DEFAULT NULL,
 `UpperLeftX` float DEFAULT NULL,
 `UpperLeftY` float DEFAULT NULL,
 `LowerRightX` float DEFAULT NULL,
 `LowerRightY` float DEFAULT NULL,
 `Zone_DistanceUnit` bigint(20) DEFAULT NULL,
 `ZoneGroup_ZoneLocations` bigint(20) DEFAULT NULL,
 PRIMARY KEY (`WID`),
 KEY `FK_Zone1` (`DataSetWID`),
 KEY `FK_Zone3` (`Zone_DistanceUnit`),
 KEY `FK_Zone4` (`ZoneGroup_ZoneLocations`),
 CONSTRAINT `FK_Zone1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_Zone3` FOREIGN KEY (`Zone_DistanceUnit`) REFERENCES `unit` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_Zone4` FOREIGN KEY (`ZoneGroup_ZoneLocations`) REFERENCES `zonegroup` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




