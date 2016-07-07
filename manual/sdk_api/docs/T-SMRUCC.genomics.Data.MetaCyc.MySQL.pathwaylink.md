---
title: pathwaylink
---

# pathwaylink
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `pathwaylink`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `pathwaylink` (
 `Pathway1WID` bigint(20) NOT NULL,
 `Pathway2WID` bigint(20) NOT NULL,
 `ChemicalWID` bigint(20) NOT NULL,
 KEY `FK_PathwayLink1` (`Pathway1WID`),
 KEY `FK_PathwayLink2` (`Pathway2WID`),
 KEY `FK_PathwayLink3` (`ChemicalWID`),
 CONSTRAINT `FK_PathwayLink1` FOREIGN KEY (`Pathway1WID`) REFERENCES `pathway` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_PathwayLink2` FOREIGN KEY (`Pathway2WID`) REFERENCES `pathway` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_PathwayLink3` FOREIGN KEY (`ChemicalWID`) REFERENCES `chemical` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




