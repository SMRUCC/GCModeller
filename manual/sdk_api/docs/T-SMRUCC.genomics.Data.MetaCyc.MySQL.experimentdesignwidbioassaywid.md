---
title: experimentdesignwidbioassaywid
---

# experimentdesignwidbioassaywid
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `experimentdesignwidbioassaywid`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `experimentdesignwidbioassaywid` (
 `ExperimentDesignWID` bigint(20) NOT NULL,
 `BioAssayWID` bigint(20) NOT NULL,
 KEY `FK_ExperimentDesignWIDBioAss1` (`ExperimentDesignWID`),
 KEY `FK_ExperimentDesignWIDBioAss2` (`BioAssayWID`),
 CONSTRAINT `FK_ExperimentDesignWIDBioAss1` FOREIGN KEY (`ExperimentDesignWID`) REFERENCES `experimentdesign` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_ExperimentDesignWIDBioAss2` FOREIGN KEY (`BioAssayWID`) REFERENCES `bioassay` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




