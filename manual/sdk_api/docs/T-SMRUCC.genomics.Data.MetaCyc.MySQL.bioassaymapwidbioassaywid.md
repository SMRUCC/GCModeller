---
title: bioassaymapwidbioassaywid
---

# bioassaymapwidbioassaywid
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `bioassaymapwidbioassaywid`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `bioassaymapwidbioassaywid` (
 `BioAssayMapWID` bigint(20) NOT NULL,
 `BioAssayWID` bigint(20) NOT NULL,
 KEY `FK_BioAssayMapWIDBioAssayWID1` (`BioAssayMapWID`),
 KEY `FK_BioAssayMapWIDBioAssayWID2` (`BioAssayWID`),
 CONSTRAINT `FK_BioAssayMapWIDBioAssayWID1` FOREIGN KEY (`BioAssayMapWID`) REFERENCES `bioevent` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_BioAssayMapWIDBioAssayWID2` FOREIGN KEY (`BioAssayWID`) REFERENCES `bioassay` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




