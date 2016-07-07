---
title: array_
---

# array_
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `array_`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `array_` (
 `WID` bigint(20) NOT NULL,
 `DataSetWID` bigint(20) NOT NULL,
 `Identifier` varchar(255) DEFAULT NULL,
 `Name` varchar(255) DEFAULT NULL,
 `ArrayIdentifier` varchar(255) DEFAULT NULL,
 `ArrayXOrigin` float DEFAULT NULL,
 `ArrayYOrigin` float DEFAULT NULL,
 `OriginRelativeTo` varchar(255) DEFAULT NULL,
 `ArrayDesign` bigint(20) DEFAULT NULL,
 `Information` bigint(20) DEFAULT NULL,
 `ArrayGroup` bigint(20) DEFAULT NULL,
 PRIMARY KEY (`WID`),
 KEY `FK_Array_1` (`DataSetWID`),
 KEY `FK_Array_3` (`ArrayDesign`),
 KEY `FK_Array_4` (`Information`),
 KEY `FK_Array_5` (`ArrayGroup`),
 CONSTRAINT `FK_Array_1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_Array_3` FOREIGN KEY (`ArrayDesign`) REFERENCES `arraydesign` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_Array_4` FOREIGN KEY (`Information`) REFERENCES `arraymanufacture` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_Array_5` FOREIGN KEY (`ArrayGroup`) REFERENCES `arraygroup` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




