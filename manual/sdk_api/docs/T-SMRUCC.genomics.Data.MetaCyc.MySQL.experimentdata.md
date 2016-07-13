---
title: experimentdata
---

# experimentdata
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `experimentdata`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `experimentdata` (
 `WID` bigint(20) NOT NULL,
 `ExperimentWID` bigint(20) NOT NULL,
 `Data` longtext,
 `MageData` bigint(20) DEFAULT NULL,
 `Role` varchar(50) NOT NULL,
 `Kind` char(1) NOT NULL,
 `DateProduced` datetime DEFAULT NULL,
 `OtherWID` bigint(20) DEFAULT NULL,
 `DataSetWID` bigint(20) NOT NULL,
 PRIMARY KEY (`WID`),
 KEY `FK_ExpData1` (`ExperimentWID`),
 KEY `FK_ExpDataMD` (`MageData`),
 KEY `FK_ExpData2` (`DataSetWID`),
 CONSTRAINT `FK_ExpData1` FOREIGN KEY (`ExperimentWID`) REFERENCES `experiment` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_ExpData2` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_ExpDataMD` FOREIGN KEY (`MageData`) REFERENCES `parametervalue` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




