---
title: biosourcewidgenewid
---

# biosourcewidgenewid
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `biosourcewidgenewid`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `biosourcewidgenewid` (
 `BioSourceWID` bigint(20) NOT NULL,
 `GeneWID` bigint(20) NOT NULL,
 KEY `FK_BioSourceWIDGeneWID1` (`BioSourceWID`),
 KEY `FK_BioSourceWIDGeneWID2` (`GeneWID`),
 CONSTRAINT `FK_BioSourceWIDGeneWID1` FOREIGN KEY (`BioSourceWID`) REFERENCES `biosource` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_BioSourceWIDGeneWID2` FOREIGN KEY (`GeneWID`) REFERENCES `gene` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




