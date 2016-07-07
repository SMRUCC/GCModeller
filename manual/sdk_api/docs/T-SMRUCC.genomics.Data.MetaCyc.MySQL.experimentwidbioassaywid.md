---
title: experimentwidbioassaywid
---

# experimentwidbioassaywid
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `experimentwidbioassaywid`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `experimentwidbioassaywid` (
 `ExperimentWID` bigint(20) NOT NULL,
 `BioAssayWID` bigint(20) NOT NULL,
 KEY `FK_ExperimentWIDBioAssayWID1` (`ExperimentWID`),
 KEY `FK_ExperimentWIDBioAssayWID2` (`BioAssayWID`),
 CONSTRAINT `FK_ExperimentWIDBioAssayWID1` FOREIGN KEY (`ExperimentWID`) REFERENCES `experiment` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_ExperimentWIDBioAssayWID2` FOREIGN KEY (`BioAssayWID`) REFERENCES `bioassay` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




