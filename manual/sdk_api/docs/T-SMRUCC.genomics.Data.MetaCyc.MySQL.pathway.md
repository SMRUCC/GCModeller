---
title: pathway
---

# pathway
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `pathway`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `pathway` (
 `WID` bigint(20) NOT NULL,
 `Name` varchar(255) NOT NULL,
 `Type` char(1) NOT NULL,
 `BioSourceWID` bigint(20) DEFAULT NULL,
 `DataSetWID` bigint(20) NOT NULL,
 PRIMARY KEY (`WID`),
 KEY `PATHWAY_BSWID_WID_DWID` (`BioSourceWID`,`WID`,`DataSetWID`),
 KEY `PATHWAY_TYPE_WID_DWID` (`Type`,`WID`,`DataSetWID`),
 KEY `PATHWAY_DWID` (`DataSetWID`),
 CONSTRAINT `FK_Pathway1` FOREIGN KEY (`BioSourceWID`) REFERENCES `biosource` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_Pathway2` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




