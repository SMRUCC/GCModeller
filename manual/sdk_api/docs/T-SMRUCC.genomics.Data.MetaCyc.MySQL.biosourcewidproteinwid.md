---
title: biosourcewidproteinwid
---

# biosourcewidproteinwid
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `biosourcewidproteinwid`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `biosourcewidproteinwid` (
 `BioSourceWID` bigint(20) NOT NULL,
 `ProteinWID` bigint(20) NOT NULL,
 KEY `FK_BioSourceWIDProteinWID1` (`BioSourceWID`),
 KEY `FK_BioSourceWIDProteinWID2` (`ProteinWID`),
 CONSTRAINT `FK_BioSourceWIDProteinWID1` FOREIGN KEY (`BioSourceWID`) REFERENCES `biosource` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_BioSourceWIDProteinWID2` FOREIGN KEY (`ProteinWID`) REFERENCES `protein` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




