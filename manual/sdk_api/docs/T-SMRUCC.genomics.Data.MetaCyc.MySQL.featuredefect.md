---
title: featuredefect
---

# featuredefect
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `featuredefect`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `featuredefect` (
 `WID` bigint(20) NOT NULL,
 `DataSetWID` bigint(20) NOT NULL,
 `ArrayManufactureDeviation` bigint(20) DEFAULT NULL,
 `FeatureDefect_DefectType` bigint(20) DEFAULT NULL,
 `FeatureDefect_PositionDelta` bigint(20) DEFAULT NULL,
 `Feature` bigint(20) DEFAULT NULL,
 PRIMARY KEY (`WID`),
 KEY `FK_FeatureDefect1` (`DataSetWID`),
 KEY `FK_FeatureDefect2` (`ArrayManufactureDeviation`),
 KEY `FK_FeatureDefect3` (`FeatureDefect_DefectType`),
 KEY `FK_FeatureDefect4` (`FeatureDefect_PositionDelta`),
 KEY `FK_FeatureDefect5` (`Feature`),
 CONSTRAINT `FK_FeatureDefect1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_FeatureDefect2` FOREIGN KEY (`ArrayManufactureDeviation`) REFERENCES `arraymanufacturedeviation` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_FeatureDefect3` FOREIGN KEY (`FeatureDefect_DefectType`) REFERENCES `term` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_FeatureDefect4` FOREIGN KEY (`FeatureDefect_PositionDelta`) REFERENCES `positiondelta` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_FeatureDefect5` FOREIGN KEY (`Feature`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




