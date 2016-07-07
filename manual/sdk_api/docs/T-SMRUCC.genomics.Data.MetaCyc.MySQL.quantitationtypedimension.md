---
title: quantitationtypedimension
---

# quantitationtypedimension
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `quantitationtypedimension`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `quantitationtypedimension` (
 `WID` bigint(20) NOT NULL,
 `DataSetWID` bigint(20) NOT NULL,
 `Identifier` varchar(255) DEFAULT NULL,
 `Name` varchar(255) DEFAULT NULL,
 PRIMARY KEY (`WID`),
 KEY `FK_QuantitationTypeDimension1` (`DataSetWID`),
 CONSTRAINT `FK_QuantitationTypeDimension1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




