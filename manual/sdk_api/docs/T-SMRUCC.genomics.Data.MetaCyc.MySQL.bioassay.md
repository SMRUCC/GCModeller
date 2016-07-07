---
title: bioassay
---

# bioassay
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `bioassay`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `bioassay` (
 `WID` bigint(20) NOT NULL,
 `DataSetWID` bigint(20) NOT NULL,
 `MAGEClass` varchar(100) NOT NULL,
 `Identifier` varchar(255) DEFAULT NULL,
 `Name` varchar(255) DEFAULT NULL,
 `DerivedBioAssay_Type` bigint(20) DEFAULT NULL,
 `FeatureExtraction` bigint(20) DEFAULT NULL,
 `BioAssayCreation` bigint(20) DEFAULT NULL,
 PRIMARY KEY (`WID`),
 KEY `FK_BioAssay1` (`DataSetWID`),
 KEY `FK_BioAssay3` (`DerivedBioAssay_Type`),
 KEY `FK_BioAssay4` (`FeatureExtraction`),
 KEY `FK_BioAssay5` (`BioAssayCreation`),
 CONSTRAINT `FK_BioAssay1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_BioAssay3` FOREIGN KEY (`DerivedBioAssay_Type`) REFERENCES `term` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_BioAssay4` FOREIGN KEY (`FeatureExtraction`) REFERENCES `bioevent` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_BioAssay5` FOREIGN KEY (`BioAssayCreation`) REFERENCES `bioevent` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




