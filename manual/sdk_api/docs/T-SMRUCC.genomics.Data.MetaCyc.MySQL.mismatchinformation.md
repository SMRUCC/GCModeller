---
title: mismatchinformation
---

# mismatchinformation
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `mismatchinformation`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `mismatchinformation` (
 `WID` bigint(20) NOT NULL,
 `DataSetWID` bigint(20) NOT NULL,
 `CompositePosition` bigint(20) DEFAULT NULL,
 `FeatureInformation` bigint(20) DEFAULT NULL,
 `StartCoord` smallint(6) DEFAULT NULL,
 `NewSequence` varchar(255) DEFAULT NULL,
 `ReplacedLength` smallint(6) DEFAULT NULL,
 `ReporterPosition` bigint(20) DEFAULT NULL,
 PRIMARY KEY (`WID`),
 KEY `FK_MismatchInformation1` (`DataSetWID`),
 KEY `FK_MismatchInformation2` (`CompositePosition`),
 KEY `FK_MismatchInformation3` (`FeatureInformation`),
 KEY `FK_MismatchInformation4` (`ReporterPosition`),
 CONSTRAINT `FK_MismatchInformation1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_MismatchInformation2` FOREIGN KEY (`CompositePosition`) REFERENCES `sequenceposition` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_MismatchInformation3` FOREIGN KEY (`FeatureInformation`) REFERENCES `featureinformation` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_MismatchInformation4` FOREIGN KEY (`ReporterPosition`) REFERENCES `sequenceposition` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




