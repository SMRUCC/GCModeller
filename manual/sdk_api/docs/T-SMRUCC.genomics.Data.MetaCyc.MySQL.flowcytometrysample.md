---
title: flowcytometrysample
---

# flowcytometrysample
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `flowcytometrysample`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `flowcytometrysample` (
 `WID` bigint(20) NOT NULL,
 `BioSourceWID` bigint(20) DEFAULT NULL,
 `FlowCytometryProbeWID` bigint(20) DEFAULT NULL,
 `MeasurementWID` bigint(20) DEFAULT NULL,
 `ManufacturerWID` bigint(20) DEFAULT NULL,
 `DataSetWID` bigint(20) NOT NULL,
 PRIMARY KEY (`WID`),
 KEY `FlowCytometrySample_DWID` (`DataSetWID`),
 KEY `FK_FlowCytometrySample1` (`BioSourceWID`),
 KEY `FK_FlowCytometrySample2` (`FlowCytometryProbeWID`),
 KEY `FK_FlowCytometrySample3` (`MeasurementWID`),
 KEY `FK_FlowCytometrySample4` (`ManufacturerWID`),
 CONSTRAINT `FK_FlowCytometrySample1` FOREIGN KEY (`BioSourceWID`) REFERENCES `biosource` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_FlowCytometrySample2` FOREIGN KEY (`FlowCytometryProbeWID`) REFERENCES `flowcytometryprobe` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_FlowCytometrySample3` FOREIGN KEY (`MeasurementWID`) REFERENCES `measurement` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_FlowCytometrySample4` FOREIGN KEY (`ManufacturerWID`) REFERENCES `contact` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_FlowCytometrySampleDS` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




