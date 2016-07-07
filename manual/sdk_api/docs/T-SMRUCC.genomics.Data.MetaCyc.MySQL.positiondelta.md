---
title: positiondelta
---

# positiondelta
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `positiondelta`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `positiondelta` (
 `WID` bigint(20) NOT NULL,
 `DataSetWID` bigint(20) NOT NULL,
 `DeltaX` float DEFAULT NULL,
 `DeltaY` float DEFAULT NULL,
 `PositionDelta_DistanceUnit` bigint(20) DEFAULT NULL,
 PRIMARY KEY (`WID`),
 KEY `FK_PositionDelta1` (`DataSetWID`),
 KEY `FK_PositionDelta2` (`PositionDelta_DistanceUnit`),
 CONSTRAINT `FK_PositionDelta1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_PositionDelta2` FOREIGN KEY (`PositionDelta_DistanceUnit`) REFERENCES `unit` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




