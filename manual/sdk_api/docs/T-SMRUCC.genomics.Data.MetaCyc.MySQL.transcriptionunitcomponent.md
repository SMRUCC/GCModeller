---
title: transcriptionunitcomponent
---

# transcriptionunitcomponent
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `transcriptionunitcomponent`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `transcriptionunitcomponent` (
 `Type` varchar(100) NOT NULL,
 `TranscriptionUnitWID` bigint(20) NOT NULL,
 `OtherWID` bigint(20) NOT NULL,
 KEY `FK_TranscriptionUnitComponent1` (`TranscriptionUnitWID`),
 CONSTRAINT `FK_TranscriptionUnitComponent1` FOREIGN KEY (`TranscriptionUnitWID`) REFERENCES `transcriptionunit` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




