---
title: biomaterialmeasurement
---

# biomaterialmeasurement
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `biomaterialmeasurement`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `biomaterialmeasurement` (
 `WID` bigint(20) NOT NULL,
 `DataSetWID` bigint(20) NOT NULL,
 `BioMaterial` bigint(20) DEFAULT NULL,
 `Measurement` bigint(20) DEFAULT NULL,
 `Treatment` bigint(20) DEFAULT NULL,
 `BioAssayCreation` bigint(20) DEFAULT NULL,
 PRIMARY KEY (`WID`),
 KEY `FK_BioMaterialMeasurement1` (`DataSetWID`),
 KEY `FK_BioMaterialMeasurement2` (`BioMaterial`),
 KEY `FK_BioMaterialMeasurement3` (`Measurement`),
 KEY `FK_BioMaterialMeasurement4` (`Treatment`),
 KEY `FK_BioMaterialMeasurement5` (`BioAssayCreation`),
 CONSTRAINT `FK_BioMaterialMeasurement1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_BioMaterialMeasurement2` FOREIGN KEY (`BioMaterial`) REFERENCES `biosource` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_BioMaterialMeasurement3` FOREIGN KEY (`Measurement`) REFERENCES `measurement` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_BioMaterialMeasurement4` FOREIGN KEY (`Treatment`) REFERENCES `bioevent` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_BioMaterialMeasurement5` FOREIGN KEY (`BioAssayCreation`) REFERENCES `bioevent` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




