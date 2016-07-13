---
title: parametervalue
---

# parametervalue
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `parametervalue`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `parametervalue` (
 `WID` bigint(20) NOT NULL,
 `DataSetWID` bigint(20) NOT NULL,
 `Value` varchar(255) DEFAULT NULL,
 `ParameterType` bigint(20) DEFAULT NULL,
 `ParameterizableApplication` bigint(20) DEFAULT NULL,
 PRIMARY KEY (`WID`),
 KEY `FK_ParameterValue1` (`DataSetWID`),
 KEY `FK_ParameterValue2` (`ParameterType`),
 KEY `FK_ParameterValue3` (`ParameterizableApplication`),
 CONSTRAINT `FK_ParameterValue1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_ParameterValue2` FOREIGN KEY (`ParameterType`) REFERENCES `parameter` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_ParameterValue3` FOREIGN KEY (`ParameterizableApplication`) REFERENCES `parameterizableapplication` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




