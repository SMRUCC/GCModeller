---
title: seqfeaturelocation
---

# seqfeaturelocation
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `seqfeaturelocation`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `seqfeaturelocation` (
 `WID` bigint(20) NOT NULL,
 `DataSetWID` bigint(20) NOT NULL,
 `SeqFeature_Regions` bigint(20) DEFAULT NULL,
 `StrandType` varchar(255) DEFAULT NULL,
 `SeqFeatureLocation_Subregions` bigint(20) DEFAULT NULL,
 `SeqFeatureLocation_Coordinate` bigint(20) DEFAULT NULL,
 PRIMARY KEY (`WID`),
 KEY `FK_SeqFeatureLocation1` (`DataSetWID`),
 KEY `FK_SeqFeatureLocation2` (`SeqFeature_Regions`),
 KEY `FK_SeqFeatureLocation3` (`SeqFeatureLocation_Subregions`),
 KEY `FK_SeqFeatureLocation4` (`SeqFeatureLocation_Coordinate`),
 CONSTRAINT `FK_SeqFeatureLocation1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_SeqFeatureLocation2` FOREIGN KEY (`SeqFeature_Regions`) REFERENCES `feature` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_SeqFeatureLocation3` FOREIGN KEY (`SeqFeatureLocation_Subregions`) REFERENCES `seqfeaturelocation` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_SeqFeatureLocation4` FOREIGN KEY (`SeqFeatureLocation_Coordinate`) REFERENCES `sequenceposition` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




