---
title: quantitationtype
---

# quantitationtype
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `quantitationtype`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `quantitationtype` (
 `WID` bigint(20) NOT NULL,
 `DataSetWID` bigint(20) NOT NULL,
 `MAGEClass` varchar(100) NOT NULL,
 `Identifier` varchar(255) DEFAULT NULL,
 `Name` varchar(255) DEFAULT NULL,
 `IsBackground` char(1) DEFAULT NULL,
 `Channel` bigint(20) DEFAULT NULL,
 `QuantitationType_Scale` bigint(20) DEFAULT NULL,
 `QuantitationType_DataType` bigint(20) DEFAULT NULL,
 `TargetQuantitationType` bigint(20) DEFAULT NULL,
 PRIMARY KEY (`WID`),
 KEY `FK_QuantitationType1` (`DataSetWID`),
 KEY `FK_QuantitationType3` (`Channel`),
 KEY `FK_QuantitationType4` (`QuantitationType_Scale`),
 KEY `FK_QuantitationType5` (`QuantitationType_DataType`),
 KEY `FK_QuantitationType6` (`TargetQuantitationType`),
 CONSTRAINT `FK_QuantitationType1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_QuantitationType3` FOREIGN KEY (`Channel`) REFERENCES `channel` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_QuantitationType4` FOREIGN KEY (`QuantitationType_Scale`) REFERENCES `term` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_QuantitationType5` FOREIGN KEY (`QuantitationType_DataType`) REFERENCES `term` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_QuantitationType6` FOREIGN KEY (`TargetQuantitationType`) REFERENCES `quantitationtype` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




