---
title: experimentdesign
---

# experimentdesign
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `experimentdesign`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `experimentdesign` (
 `WID` bigint(20) NOT NULL,
 `DataSetWID` bigint(20) NOT NULL,
 `Experiment_ExperimentDesigns` bigint(20) DEFAULT NULL,
 `QualityControlDescription` bigint(20) DEFAULT NULL,
 `NormalizationDescription` bigint(20) DEFAULT NULL,
 `ReplicateDescription` bigint(20) DEFAULT NULL,
 PRIMARY KEY (`WID`),
 KEY `FK_ExperimentDesign1` (`DataSetWID`),
 KEY `FK_ExperimentDesign3` (`Experiment_ExperimentDesigns`),
 CONSTRAINT `FK_ExperimentDesign1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_ExperimentDesign3` FOREIGN KEY (`Experiment_ExperimentDesigns`) REFERENCES `experiment` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




