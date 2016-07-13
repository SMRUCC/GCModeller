---
title: biodatavalues
---

# biodatavalues
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `biodatavalues`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `biodatavalues` (
 `WID` bigint(20) NOT NULL,
 `DataSetWID` bigint(20) NOT NULL,
 `MAGEClass` varchar(100) NOT NULL,
 `Order_` varchar(25) DEFAULT NULL,
 `BioDataCube_DataInternal` bigint(20) DEFAULT NULL,
 `BioDataCube_DataExternal` bigint(20) DEFAULT NULL,
 PRIMARY KEY (`WID`),
 KEY `FK_BioDataValues1` (`DataSetWID`),
 KEY `FK_BioDataValues2` (`BioDataCube_DataInternal`),
 KEY `FK_BioDataValues3` (`BioDataCube_DataExternal`),
 CONSTRAINT `FK_BioDataValues1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_BioDataValues2` FOREIGN KEY (`BioDataCube_DataInternal`) REFERENCES `datainternal` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_BioDataValues3` FOREIGN KEY (`BioDataCube_DataExternal`) REFERENCES `dataexternal` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




