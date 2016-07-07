---
title: fiducial
---

# fiducial
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `fiducial`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `fiducial` (
 `WID` bigint(20) NOT NULL,
 `DataSetWID` bigint(20) NOT NULL,
 `ArrayGroup_Fiducials` bigint(20) DEFAULT NULL,
 `Fiducial_FiducialType` bigint(20) DEFAULT NULL,
 `Fiducial_DistanceUnit` bigint(20) DEFAULT NULL,
 `Fiducial_Position` bigint(20) DEFAULT NULL,
 PRIMARY KEY (`WID`),
 KEY `FK_Fiducial1` (`DataSetWID`),
 KEY `FK_Fiducial3` (`ArrayGroup_Fiducials`),
 KEY `FK_Fiducial4` (`Fiducial_FiducialType`),
 KEY `FK_Fiducial5` (`Fiducial_DistanceUnit`),
 KEY `FK_Fiducial6` (`Fiducial_Position`),
 CONSTRAINT `FK_Fiducial1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_Fiducial3` FOREIGN KEY (`ArrayGroup_Fiducials`) REFERENCES `arraygroup` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_Fiducial4` FOREIGN KEY (`Fiducial_FiducialType`) REFERENCES `term` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_Fiducial5` FOREIGN KEY (`Fiducial_DistanceUnit`) REFERENCES `unit` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_Fiducial6` FOREIGN KEY (`Fiducial_Position`) REFERENCES `position_` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




