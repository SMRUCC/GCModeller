---
title: nodevalue
---

# nodevalue
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `nodevalue`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `nodevalue` (
 `WID` bigint(20) NOT NULL,
 `DataSetWID` bigint(20) NOT NULL,
 `Node_NodeValue` bigint(20) DEFAULT NULL,
 `Name` varchar(255) DEFAULT NULL,
 `Value` varchar(255) DEFAULT NULL,
 `NodeValue_Type` bigint(20) DEFAULT NULL,
 `NodeValue_Scale` bigint(20) DEFAULT NULL,
 `NodeValue_DataType` bigint(20) DEFAULT NULL,
 PRIMARY KEY (`WID`),
 KEY `FK_NodeValue1` (`DataSetWID`),
 KEY `FK_NodeValue2` (`Node_NodeValue`),
 KEY `FK_NodeValue3` (`NodeValue_Type`),
 KEY `FK_NodeValue4` (`NodeValue_Scale`),
 KEY `FK_NodeValue5` (`NodeValue_DataType`),
 CONSTRAINT `FK_NodeValue1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_NodeValue2` FOREIGN KEY (`Node_NodeValue`) REFERENCES `node` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_NodeValue3` FOREIGN KEY (`NodeValue_Type`) REFERENCES `term` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_NodeValue4` FOREIGN KEY (`NodeValue_Scale`) REFERENCES `term` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_NodeValue5` FOREIGN KEY (`NodeValue_DataType`) REFERENCES `term` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




