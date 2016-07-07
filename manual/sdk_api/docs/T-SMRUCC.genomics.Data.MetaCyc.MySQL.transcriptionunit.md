---
title: transcriptionunit
---

# transcriptionunit
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `transcriptionunit`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `transcriptionunit` (
 `WID` bigint(20) NOT NULL,
 `Name` varchar(255) NOT NULL,
 `DataSetWID` bigint(20) NOT NULL,
 PRIMARY KEY (`WID`),
 KEY `TranscriptionUnit_DWID` (`DataSetWID`),
 CONSTRAINT `FK_TranscriptionUnit1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




