---
title: superpathway
---

# superpathway
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `superpathway`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `superpathway` (
 `SubPathwayWID` bigint(20) NOT NULL,
 `SuperPathwayWID` bigint(20) NOT NULL,
 KEY `FK_SuperPathway1` (`SubPathwayWID`),
 KEY `FK_SuperPathway2` (`SuperPathwayWID`),
 CONSTRAINT `FK_SuperPathway1` FOREIGN KEY (`SubPathwayWID`) REFERENCES `pathway` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_SuperPathway2` FOREIGN KEY (`SuperPathwayWID`) REFERENCES `pathway` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




