---
title: arraydesign
---

# arraydesign
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `arraydesign`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `arraydesign` (
 `WID` bigint(20) NOT NULL,
 `DataSetWID` bigint(20) NOT NULL,
 `MAGEClass` varchar(100) NOT NULL,
 `Identifier` varchar(255) DEFAULT NULL,
 `Name` varchar(255) DEFAULT NULL,
 `Version` varchar(255) DEFAULT NULL,
 `NumberOfFeatures` smallint(6) DEFAULT NULL,
 `SurfaceType` bigint(20) DEFAULT NULL,
 PRIMARY KEY (`WID`),
 KEY `FK_ArrayDesign1` (`DataSetWID`),
 KEY `FK_ArrayDesign3` (`SurfaceType`),
 CONSTRAINT `FK_ArrayDesign1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_ArrayDesign3` FOREIGN KEY (`SurfaceType`) REFERENCES `term` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




