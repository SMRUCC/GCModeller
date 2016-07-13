---
title: geneticcode
---

# geneticcode
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `geneticcode`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `geneticcode` (
 `WID` bigint(20) NOT NULL,
 `NCBIID` varchar(2) DEFAULT NULL,
 `Name` varchar(100) DEFAULT NULL,
 `TranslationTable` varchar(64) DEFAULT NULL,
 `StartCodon` varchar(64) DEFAULT NULL,
 `DataSetWID` bigint(20) NOT NULL,
 PRIMARY KEY (`WID`),
 KEY `FK_GeneticCode` (`DataSetWID`),
 CONSTRAINT `FK_GeneticCode` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




