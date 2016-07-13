---
title: enzymaticreaction
---

# enzymaticreaction
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `enzymaticreaction`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `enzymaticreaction` (
 `WID` bigint(20) NOT NULL,
 `ReactionWID` bigint(20) NOT NULL,
 `ProteinWID` bigint(20) NOT NULL,
 `ComplexWID` bigint(20) DEFAULT NULL,
 `ReactionDirection` varchar(30) DEFAULT NULL,
 `DataSetWID` bigint(20) NOT NULL,
 PRIMARY KEY (`WID`),
 KEY `ER_DATASETWID` (`DataSetWID`),
 KEY `FK_EnzymaticReaction1` (`ReactionWID`),
 KEY `FK_EnzymaticReaction2` (`ProteinWID`),
 KEY `FK_EnzymaticReaction3` (`ComplexWID`),
 CONSTRAINT `FK_EnzymaticReaction1` FOREIGN KEY (`ReactionWID`) REFERENCES `reaction` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_EnzymaticReaction2` FOREIGN KEY (`ProteinWID`) REFERENCES `protein` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_EnzymaticReaction3` FOREIGN KEY (`ComplexWID`) REFERENCES `protein` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_EnzymaticReaction4` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




