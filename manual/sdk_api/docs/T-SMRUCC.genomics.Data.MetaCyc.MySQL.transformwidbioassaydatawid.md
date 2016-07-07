---
title: transformwidbioassaydatawid
---

# transformwidbioassaydatawid
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `transformwidbioassaydatawid`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `transformwidbioassaydatawid` (
 `TransformationWID` bigint(20) NOT NULL,
 `BioAssayDataWID` bigint(20) NOT NULL,
 KEY `FK_TransformWIDBioAssayDataW1` (`TransformationWID`),
 KEY `FK_TransformWIDBioAssayDataW2` (`BioAssayDataWID`),
 CONSTRAINT `FK_TransformWIDBioAssayDataW1` FOREIGN KEY (`TransformationWID`) REFERENCES `bioevent` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_TransformWIDBioAssayDataW2` FOREIGN KEY (`BioAssayDataWID`) REFERENCES `bioassaydata` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




