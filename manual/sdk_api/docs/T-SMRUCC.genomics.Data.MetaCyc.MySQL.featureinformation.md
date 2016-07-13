---
title: featureinformation
---

# featureinformation
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `featureinformation`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `featureinformation` (
 `WID` bigint(20) NOT NULL,
 `DataSetWID` bigint(20) NOT NULL,
 `Feature` bigint(20) DEFAULT NULL,
 `FeatureReporterMap` bigint(20) DEFAULT NULL,
 PRIMARY KEY (`WID`),
 KEY `FK_FeatureInformation1` (`DataSetWID`),
 KEY `FK_FeatureInformation2` (`Feature`),
 KEY `FK_FeatureInformation3` (`FeatureReporterMap`),
 CONSTRAINT `FK_FeatureInformation1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_FeatureInformation2` FOREIGN KEY (`Feature`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_FeatureInformation3` FOREIGN KEY (`FeatureReporterMap`) REFERENCES `bioevent` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




