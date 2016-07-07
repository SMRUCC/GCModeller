---
title: image
---

# image
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `image`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `image` (
 `WID` bigint(20) NOT NULL,
 `DataSetWID` bigint(20) NOT NULL,
 `Identifier` varchar(255) DEFAULT NULL,
 `Name` varchar(255) DEFAULT NULL,
 `URI` varchar(255) DEFAULT NULL,
 `Image_Format` bigint(20) DEFAULT NULL,
 `PhysicalBioAssay` bigint(20) DEFAULT NULL,
 PRIMARY KEY (`WID`),
 KEY `FK_Image1` (`DataSetWID`),
 KEY `FK_Image3` (`Image_Format`),
 KEY `FK_Image4` (`PhysicalBioAssay`),
 CONSTRAINT `FK_Image1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_Image3` FOREIGN KEY (`Image_Format`) REFERENCES `term` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_Image4` FOREIGN KEY (`PhysicalBioAssay`) REFERENCES `bioassay` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




