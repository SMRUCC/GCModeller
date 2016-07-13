---
title: zonedefect
---

# zonedefect
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `zonedefect`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `zonedefect` (
 `WID` bigint(20) NOT NULL,
 `DataSetWID` bigint(20) NOT NULL,
 `ArrayManufactureDeviation` bigint(20) DEFAULT NULL,
 `ZoneDefect_DefectType` bigint(20) DEFAULT NULL,
 `ZoneDefect_PositionDelta` bigint(20) DEFAULT NULL,
 `Zone` bigint(20) DEFAULT NULL,
 PRIMARY KEY (`WID`),
 KEY `FK_ZoneDefect1` (`DataSetWID`),
 KEY `FK_ZoneDefect2` (`ArrayManufactureDeviation`),
 KEY `FK_ZoneDefect3` (`ZoneDefect_DefectType`),
 KEY `FK_ZoneDefect4` (`ZoneDefect_PositionDelta`),
 KEY `FK_ZoneDefect5` (`Zone`),
 CONSTRAINT `FK_ZoneDefect1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_ZoneDefect2` FOREIGN KEY (`ArrayManufactureDeviation`) REFERENCES `arraymanufacturedeviation` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_ZoneDefect3` FOREIGN KEY (`ZoneDefect_DefectType`) REFERENCES `term` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_ZoneDefect4` FOREIGN KEY (`ZoneDefect_PositionDelta`) REFERENCES `positiondelta` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_ZoneDefect5` FOREIGN KEY (`Zone`) REFERENCES `zone` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




