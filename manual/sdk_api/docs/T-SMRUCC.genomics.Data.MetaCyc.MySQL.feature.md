---
title: feature
---

# feature
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `feature`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `feature` (
 `WID` bigint(20) NOT NULL,
 `Description` varchar(1300) DEFAULT NULL,
 `Type` varchar(50) DEFAULT NULL,
 `Class` varchar(50) DEFAULT NULL,
 `SequenceType` char(1) NOT NULL,
 `SequenceWID` bigint(20) DEFAULT NULL,
 `Variant` longtext,
 `RegionOrPoint` varchar(10) DEFAULT NULL,
 `PointType` varchar(10) DEFAULT NULL,
 `StartPosition` int(11) DEFAULT NULL,
 `EndPosition` int(11) DEFAULT NULL,
 `StartPositionApproximate` varchar(10) DEFAULT NULL,
 `EndPositionApproximate` varchar(10) DEFAULT NULL,
 `ExperimentalSupport` char(1) DEFAULT NULL,
 `ComputationalSupport` char(1) DEFAULT NULL,
 `DataSetWID` bigint(20) NOT NULL,
 PRIMARY KEY (`WID`),
 KEY `FK_Feature` (`DataSetWID`),
 CONSTRAINT `FK_Feature` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




