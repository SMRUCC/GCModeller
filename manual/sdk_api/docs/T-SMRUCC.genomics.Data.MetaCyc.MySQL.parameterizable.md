---
title: parameterizable
---

# parameterizable
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `parameterizable`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `parameterizable` (
 `WID` bigint(20) NOT NULL,
 `DataSetWID` bigint(20) NOT NULL,
 `MAGEClass` varchar(100) NOT NULL,
 `Identifier` varchar(255) DEFAULT NULL,
 `Name` varchar(255) DEFAULT NULL,
 `URI` varchar(255) DEFAULT NULL,
 `Model` varchar(255) DEFAULT NULL,
 `Make` varchar(255) DEFAULT NULL,
 `Hardware_Type` bigint(20) DEFAULT NULL,
 `Text` varchar(1000) DEFAULT NULL,
 `Title` varchar(255) DEFAULT NULL,
 `Protocol_Type` bigint(20) DEFAULT NULL,
 `Software_Type` bigint(20) DEFAULT NULL,
 `Hardware` bigint(20) DEFAULT NULL,
 PRIMARY KEY (`WID`),
 KEY `FK_Parameterizable1` (`DataSetWID`),
 KEY `FK_Parameterizable3` (`Hardware_Type`),
 KEY `FK_Parameterizable4` (`Protocol_Type`),
 KEY `FK_Parameterizable5` (`Software_Type`),
 KEY `FK_Parameterizable6` (`Hardware`),
 CONSTRAINT `FK_Parameterizable1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_Parameterizable3` FOREIGN KEY (`Hardware_Type`) REFERENCES `term` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_Parameterizable4` FOREIGN KEY (`Protocol_Type`) REFERENCES `term` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_Parameterizable5` FOREIGN KEY (`Software_Type`) REFERENCES `term` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_Parameterizable6` FOREIGN KEY (`Hardware`) REFERENCES `parameterizable` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




