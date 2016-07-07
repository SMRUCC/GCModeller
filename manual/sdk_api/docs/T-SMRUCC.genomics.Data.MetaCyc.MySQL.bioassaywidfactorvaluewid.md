---
title: bioassaywidfactorvaluewid
---

# bioassaywidfactorvaluewid
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `bioassaywidfactorvaluewid`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `bioassaywidfactorvaluewid` (
 `BioAssayWID` bigint(20) NOT NULL,
 `FactorValueWID` bigint(20) NOT NULL,
 KEY `FK_BioAssayWIDFactorValueWID1` (`BioAssayWID`),
 KEY `FK_BioAssayWIDFactorValueWID2` (`FactorValueWID`),
 CONSTRAINT `FK_BioAssayWIDFactorValueWID1` FOREIGN KEY (`BioAssayWID`) REFERENCES `bioassay` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_BioAssayWIDFactorValueWID2` FOREIGN KEY (`FactorValueWID`) REFERENCES `factorvalue` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




