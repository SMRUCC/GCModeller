---
title: factorvalue
---

# factorvalue
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `factorvalue`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `factorvalue` (
 `WID` bigint(20) NOT NULL,
 `DataSetWID` bigint(20) NOT NULL,
 `Identifier` varchar(255) DEFAULT NULL,
 `Name` varchar(255) DEFAULT NULL,
 `ExperimentalFactor` bigint(20) DEFAULT NULL,
 `ExperimentalFactor2` bigint(20) DEFAULT NULL,
 `FactorValue_Measurement` bigint(20) DEFAULT NULL,
 `FactorValue_Value` bigint(20) DEFAULT NULL,
 PRIMARY KEY (`WID`),
 KEY `FK_FactorValue1` (`DataSetWID`),
 KEY `FK_FactorValue3` (`ExperimentalFactor`),
 KEY `FK_FactorValue4` (`ExperimentalFactor2`),
 KEY `FK_FactorValue5` (`FactorValue_Measurement`),
 KEY `FK_FactorValue6` (`FactorValue_Value`),
 CONSTRAINT `FK_FactorValue1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_FactorValue3` FOREIGN KEY (`ExperimentalFactor`) REFERENCES `experimentalfactor` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_FactorValue4` FOREIGN KEY (`ExperimentalFactor2`) REFERENCES `experimentalfactor` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_FactorValue5` FOREIGN KEY (`FactorValue_Measurement`) REFERENCES `measurement` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_FactorValue6` FOREIGN KEY (`FactorValue_Value`) REFERENCES `term` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




