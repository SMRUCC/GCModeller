---
title: experimentalfactor
---

# experimentalfactor
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `experimentalfactor`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `experimentalfactor` (
 `WID` bigint(20) NOT NULL,
 `DataSetWID` bigint(20) NOT NULL,
 `Identifier` varchar(255) DEFAULT NULL,
 `Name` varchar(255) DEFAULT NULL,
 `ExperimentDesign` bigint(20) DEFAULT NULL,
 `ExperimentalFactor_Category` bigint(20) DEFAULT NULL,
 PRIMARY KEY (`WID`),
 KEY `FK_ExperimentalFactor1` (`DataSetWID`),
 KEY `FK_ExperimentalFactor3` (`ExperimentDesign`),
 KEY `FK_ExperimentalFactor4` (`ExperimentalFactor_Category`),
 CONSTRAINT `FK_ExperimentalFactor1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_ExperimentalFactor3` FOREIGN KEY (`ExperimentDesign`) REFERENCES `experimentdesign` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_ExperimentalFactor4` FOREIGN KEY (`ExperimentalFactor_Category`) REFERENCES `term` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




