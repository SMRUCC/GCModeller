---
title: bioassaytuple
---

# bioassaytuple
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `bioassaytuple`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `bioassaytuple` (
 `WID` bigint(20) NOT NULL,
 `DataSetWID` bigint(20) NOT NULL,
 `BioAssay` bigint(20) DEFAULT NULL,
 `BioDataTuples_BioAssayTuples` bigint(20) DEFAULT NULL,
 PRIMARY KEY (`WID`),
 KEY `FK_BioAssayTuple1` (`DataSetWID`),
 KEY `FK_BioAssayTuple2` (`BioAssay`),
 KEY `FK_BioAssayTuple3` (`BioDataTuples_BioAssayTuples`),
 CONSTRAINT `FK_BioAssayTuple1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_BioAssayTuple2` FOREIGN KEY (`BioAssay`) REFERENCES `bioassay` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_BioAssayTuple3` FOREIGN KEY (`BioDataTuples_BioAssayTuples`) REFERENCES `biodatavalues` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




