---
title: namevaluetype
---

# namevaluetype
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `namevaluetype`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `namevaluetype` (
 `WID` bigint(20) NOT NULL,
 `DataSetWID` bigint(20) NOT NULL,
 `Name` varchar(255) DEFAULT NULL,
 `Value` varchar(255) DEFAULT NULL,
 `Type_` varchar(255) DEFAULT NULL,
 `NameValueType_PropertySets` bigint(20) DEFAULT NULL,
 `OtherWID` bigint(20) DEFAULT NULL,
 PRIMARY KEY (`WID`),
 KEY `FK_NameValueType1` (`DataSetWID`),
 KEY `FK_NameValueType66` (`NameValueType_PropertySets`),
 CONSTRAINT `FK_NameValueType1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_NameValueType66` FOREIGN KEY (`NameValueType_PropertySets`) REFERENCES `namevaluetype` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




