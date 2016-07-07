---
title: experimwidbioassaydataclustwid
---

# experimwidbioassaydataclustwid
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `experimwidbioassaydataclustwid`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `experimwidbioassaydataclustwid` (
 `ExperimentWID` bigint(20) NOT NULL,
 `BioAssayDataClusterWID` bigint(20) NOT NULL,
 KEY `FK_ExperimWIDBioAssayDataClu1` (`ExperimentWID`),
 KEY `FK_ExperimWIDBioAssayDataClu2` (`BioAssayDataClusterWID`),
 CONSTRAINT `FK_ExperimWIDBioAssayDataClu1` FOREIGN KEY (`ExperimentWID`) REFERENCES `experiment` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_ExperimWIDBioAssayDataClu2` FOREIGN KEY (`BioAssayDataClusterWID`) REFERENCES `bioassaydatacluster` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




