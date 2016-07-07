---
title: dataexternal
---

# dataexternal
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `dataexternal`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `dataexternal` (
 `WID` bigint(20) NOT NULL,
 `DataSetWID` bigint(20) NOT NULL,
 `DataFormat` varchar(255) DEFAULT NULL,
 `DataFormatInfoURI` varchar(255) DEFAULT NULL,
 `FilenameURI` varchar(255) DEFAULT NULL,
 PRIMARY KEY (`WID`),
 KEY `FK_DataExternal1` (`DataSetWID`),
 CONSTRAINT `FK_DataExternal1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




