---
title: zonegroup
---

# zonegroup
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `zonegroup`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `zonegroup` (
 `WID` bigint(20) NOT NULL,
 `DataSetWID` bigint(20) NOT NULL,
 `PhysicalArrayDesign_ZoneGroups` bigint(20) DEFAULT NULL,
 `SpacingsBetweenZonesX` float DEFAULT NULL,
 `SpacingsBetweenZonesY` float DEFAULT NULL,
 `ZonesPerX` smallint(6) DEFAULT NULL,
 `ZonesPerY` smallint(6) DEFAULT NULL,
 `ZoneGroup_DistanceUnit` bigint(20) DEFAULT NULL,
 `ZoneGroup_ZoneLayout` bigint(20) DEFAULT NULL,
 PRIMARY KEY (`WID`),
 KEY `FK_ZoneGroup1` (`DataSetWID`),
 KEY `FK_ZoneGroup2` (`PhysicalArrayDesign_ZoneGroups`),
 KEY `FK_ZoneGroup3` (`ZoneGroup_DistanceUnit`),
 KEY `FK_ZoneGroup4` (`ZoneGroup_ZoneLayout`),
 CONSTRAINT `FK_ZoneGroup1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_ZoneGroup2` FOREIGN KEY (`PhysicalArrayDesign_ZoneGroups`) REFERENCES `arraydesign` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_ZoneGroup3` FOREIGN KEY (`ZoneGroup_DistanceUnit`) REFERENCES `unit` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_ZoneGroup4` FOREIGN KEY (`ZoneGroup_ZoneLayout`) REFERENCES `zonelayout` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




