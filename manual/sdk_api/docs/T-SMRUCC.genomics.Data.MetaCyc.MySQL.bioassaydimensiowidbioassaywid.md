---
title: bioassaydimensiowidbioassaywid
---

# bioassaydimensiowidbioassaywid
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `bioassaydimensiowidbioassaywid`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `bioassaydimensiowidbioassaywid` (
 `BioAssayDimensionWID` bigint(20) NOT NULL,
 `BioAssayWID` bigint(20) NOT NULL,
 KEY `FK_BioAssayDimensioWIDBioAss1` (`BioAssayDimensionWID`),
 KEY `FK_BioAssayDimensioWIDBioAss2` (`BioAssayWID`),
 CONSTRAINT `FK_BioAssayDimensioWIDBioAss1` FOREIGN KEY (`BioAssayDimensionWID`) REFERENCES `bioassaydimension` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_BioAssayDimensioWIDBioAss2` FOREIGN KEY (`BioAssayWID`) REFERENCES `bioassay` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




