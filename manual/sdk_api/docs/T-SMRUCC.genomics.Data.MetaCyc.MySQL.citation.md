---
title: citation
---

# citation
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `citation`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `citation` (
 `WID` bigint(20) NOT NULL,
 `Citation` text,
 `PMID` decimal(10,0) DEFAULT NULL,
 `Title` varchar(255) DEFAULT NULL,
 `Authors` varchar(255) DEFAULT NULL,
 `Publication` varchar(255) DEFAULT NULL,
 `Publisher` varchar(255) DEFAULT NULL,
 `Editor` varchar(255) DEFAULT NULL,
 `Year` varchar(255) DEFAULT NULL,
 `Volume` varchar(255) DEFAULT NULL,
 `Issue` varchar(255) DEFAULT NULL,
 `Pages` varchar(255) DEFAULT NULL,
 `URI` varchar(255) DEFAULT NULL,
 `DataSetWID` bigint(20) NOT NULL,
 PRIMARY KEY (`WID`),
 KEY `CITATION_PMID` (`PMID`),
 KEY `CITATION_CITATION` (`Citation`(20)),
 KEY `FK_Citation` (`DataSetWID`),
 CONSTRAINT `FK_Citation` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




