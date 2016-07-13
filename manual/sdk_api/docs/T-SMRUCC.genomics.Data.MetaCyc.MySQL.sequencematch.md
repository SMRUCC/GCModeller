---
title: sequencematch
---

# sequencematch
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `sequencematch`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `sequencematch` (
 `QueryWID` bigint(20) NOT NULL,
 `MatchWID` bigint(20) NOT NULL,
 `ComputationWID` bigint(20) NOT NULL,
 `EValue` double DEFAULT NULL,
 `PValue` double DEFAULT NULL,
 `PercentIdentical` float DEFAULT NULL,
 `PercentSimilar` float DEFAULT NULL,
 `Rank` smallint(6) DEFAULT NULL,
 `Length` int(11) DEFAULT NULL,
 `QueryStart` int(11) DEFAULT NULL,
 `QueryEnd` int(11) DEFAULT NULL,
 `MatchStart` int(11) DEFAULT NULL,
 `MatchEnd` int(11) DEFAULT NULL,
 KEY `FK_SequenceMatch` (`ComputationWID`),
 CONSTRAINT `FK_SequenceMatch` FOREIGN KEY (`ComputationWID`) REFERENCES `computation` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




