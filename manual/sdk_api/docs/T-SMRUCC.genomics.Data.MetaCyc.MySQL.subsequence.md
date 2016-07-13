---
title: subsequence
---

# subsequence
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `subsequence`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `subsequence` (
 `WID` bigint(20) NOT NULL,
 `NucleicAcidWID` bigint(20) NOT NULL,
 `FullSequence` char(1) DEFAULT NULL,
 `Sequence` longtext,
 `Length` int(11) DEFAULT NULL,
 `LengthApproximate` varchar(10) DEFAULT NULL,
 `PercentGC` float DEFAULT NULL,
 `Version` varchar(30) DEFAULT NULL,
 `DataSetWID` bigint(20) NOT NULL,
 PRIMARY KEY (`WID`),
 KEY `FK_Subsequence1` (`NucleicAcidWID`),
 KEY `FK_Subsequence2` (`DataSetWID`),
 CONSTRAINT `FK_Subsequence1` FOREIGN KEY (`NucleicAcidWID`) REFERENCES `nucleicacid` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_Subsequence2` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




