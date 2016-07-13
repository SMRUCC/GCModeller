---
title: experimentwidbioassaydatawid
---

# experimentwidbioassaydatawid
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `experimentwidbioassaydatawid`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `experimentwidbioassaydatawid` (
 `ExperimentWID` bigint(20) NOT NULL,
 `BioAssayDataWID` bigint(20) NOT NULL,
 KEY `FK_ExperimentWIDBioAssayData1` (`ExperimentWID`),
 KEY `FK_ExperimentWIDBioAssayData2` (`BioAssayDataWID`),
 CONSTRAINT `FK_ExperimentWIDBioAssayData1` FOREIGN KEY (`ExperimentWID`) REFERENCES `experiment` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_ExperimentWIDBioAssayData2` FOREIGN KEY (`BioAssayDataWID`) REFERENCES `bioassaydata` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




