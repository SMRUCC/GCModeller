---
title: arraygroup
---

# arraygroup
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `arraygroup`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `arraygroup` (
 `WID` bigint(20) NOT NULL,
 `DataSetWID` bigint(20) NOT NULL,
 `Identifier` varchar(255) DEFAULT NULL,
 `Name` varchar(255) DEFAULT NULL,
 `Barcode` varchar(255) DEFAULT NULL,
 `ArraySpacingX` float DEFAULT NULL,
 `ArraySpacingY` float DEFAULT NULL,
 `NumArrays` smallint(6) DEFAULT NULL,
 `OrientationMark` varchar(255) DEFAULT NULL,
 `OrientationMarkPosition` varchar(25) DEFAULT NULL,
 `Width` float DEFAULT NULL,
 `Length` float DEFAULT NULL,
 `ArrayGroup_SubstrateType` bigint(20) DEFAULT NULL,
 `ArrayGroup_DistanceUnit` bigint(20) DEFAULT NULL,
 PRIMARY KEY (`WID`),
 KEY `FK_ArrayGroup1` (`DataSetWID`),
 KEY `FK_ArrayGroup3` (`ArrayGroup_SubstrateType`),
 KEY `FK_ArrayGroup4` (`ArrayGroup_DistanceUnit`),
 CONSTRAINT `FK_ArrayGroup1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_ArrayGroup3` FOREIGN KEY (`ArrayGroup_SubstrateType`) REFERENCES `term` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_ArrayGroup4` FOREIGN KEY (`ArrayGroup_DistanceUnit`) REFERENCES `unit` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




