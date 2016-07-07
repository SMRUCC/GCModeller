---
title: derivbioawidderivbioadatawid
---

# derivbioawidderivbioadatawid
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `derivbioawidderivbioadatawid`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `derivbioawidderivbioadatawid` (
 `DerivedBioAssayWID` bigint(20) NOT NULL,
 `DerivedBioAssayDataWID` bigint(20) NOT NULL,
 KEY `FK_DerivBioAWIDDerivBioAData1` (`DerivedBioAssayWID`),
 KEY `FK_DerivBioAWIDDerivBioAData2` (`DerivedBioAssayDataWID`),
 CONSTRAINT `FK_DerivBioAWIDDerivBioAData1` FOREIGN KEY (`DerivedBioAssayWID`) REFERENCES `bioassay` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_DerivBioAWIDDerivBioAData2` FOREIGN KEY (`DerivedBioAssayDataWID`) REFERENCES `bioassaydata` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




