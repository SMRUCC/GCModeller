---
title: bioassaydata
---

# bioassaydata
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `bioassaydata`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `bioassaydata` (
 `WID` bigint(20) NOT NULL,
 `DataSetWID` bigint(20) NOT NULL,
 `MAGEClass` varchar(100) NOT NULL,
 `Identifier` varchar(255) DEFAULT NULL,
 `Name` varchar(255) DEFAULT NULL,
 `BioAssayDimension` bigint(20) DEFAULT NULL,
 `DesignElementDimension` bigint(20) DEFAULT NULL,
 `QuantitationTypeDimension` bigint(20) DEFAULT NULL,
 `BioAssayData_BioDataValues` bigint(20) DEFAULT NULL,
 `ProducerTransformation` bigint(20) DEFAULT NULL,
 PRIMARY KEY (`WID`),
 KEY `FK_BioAssayData1` (`DataSetWID`),
 KEY `FK_BioAssayData3` (`BioAssayDimension`),
 KEY `FK_BioAssayData4` (`DesignElementDimension`),
 KEY `FK_BioAssayData5` (`QuantitationTypeDimension`),
 KEY `FK_BioAssayData6` (`BioAssayData_BioDataValues`),
 KEY `FK_BioAssayData7` (`ProducerTransformation`),
 CONSTRAINT `FK_BioAssayData1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_BioAssayData3` FOREIGN KEY (`BioAssayDimension`) REFERENCES `bioassaydimension` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_BioAssayData4` FOREIGN KEY (`DesignElementDimension`) REFERENCES `designelementdimension` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_BioAssayData5` FOREIGN KEY (`QuantitationTypeDimension`) REFERENCES `quantitationtypedimension` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_BioAssayData6` FOREIGN KEY (`BioAssayData_BioDataValues`) REFERENCES `biodatavalues` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_BioAssayData7` FOREIGN KEY (`ProducerTransformation`) REFERENCES `bioevent` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




