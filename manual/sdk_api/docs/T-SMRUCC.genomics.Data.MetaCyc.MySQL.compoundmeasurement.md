---
title: compoundmeasurement
---

# compoundmeasurement
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `compoundmeasurement`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `compoundmeasurement` (
 `WID` bigint(20) NOT NULL,
 `DataSetWID` bigint(20) NOT NULL,
 `Compound_ComponentCompounds` bigint(20) DEFAULT NULL,
 `Compound` bigint(20) DEFAULT NULL,
 `Measurement` bigint(20) DEFAULT NULL,
 `Treatment_CompoundMeasurements` bigint(20) DEFAULT NULL,
 PRIMARY KEY (`WID`),
 KEY `FK_CompoundMeasurement1` (`DataSetWID`),
 KEY `FK_CompoundMeasurement2` (`Compound_ComponentCompounds`),
 KEY `FK_CompoundMeasurement3` (`Compound`),
 KEY `FK_CompoundMeasurement4` (`Measurement`),
 KEY `FK_CompoundMeasurement5` (`Treatment_CompoundMeasurements`),
 CONSTRAINT `FK_CompoundMeasurement1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_CompoundMeasurement2` FOREIGN KEY (`Compound_ComponentCompounds`) REFERENCES `chemical` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_CompoundMeasurement3` FOREIGN KEY (`Compound`) REFERENCES `chemical` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_CompoundMeasurement4` FOREIGN KEY (`Measurement`) REFERENCES `measurement` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_CompoundMeasurement5` FOREIGN KEY (`Treatment_CompoundMeasurements`) REFERENCES `bioevent` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




