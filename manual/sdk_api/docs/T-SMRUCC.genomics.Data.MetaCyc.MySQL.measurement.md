---
title: measurement
---

# measurement
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `measurement`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `measurement` (
 `WID` bigint(20) NOT NULL,
 `DataSetWID` bigint(20) NOT NULL,
 `Type_` varchar(25) DEFAULT NULL,
 `Value` varchar(255) DEFAULT NULL,
 `KindCV` varchar(25) DEFAULT NULL,
 `OtherKind` varchar(255) DEFAULT NULL,
 `Measurement_Unit` bigint(20) DEFAULT NULL,
 PRIMARY KEY (`WID`),
 KEY `FK_Measurement1` (`DataSetWID`),
 KEY `FK_Measurement2` (`Measurement_Unit`),
 CONSTRAINT `FK_Measurement1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_Measurement2` FOREIGN KEY (`Measurement_Unit`) REFERENCES `unit` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




