---
title: enzreactionaltcompound
---

# enzreactionaltcompound
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `enzreactionaltcompound`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `enzreactionaltcompound` (
 `EnzymaticReactionWID` bigint(20) NOT NULL,
 `PrimaryWID` bigint(20) NOT NULL,
 `AlternativeWID` bigint(20) NOT NULL,
 `Cofactor` char(1) DEFAULT NULL,
 KEY `FK_ERAC1` (`EnzymaticReactionWID`),
 KEY `FK_ERAC2` (`PrimaryWID`),
 KEY `FK_ERAC3` (`AlternativeWID`),
 CONSTRAINT `FK_ERAC1` FOREIGN KEY (`EnzymaticReactionWID`) REFERENCES `enzymaticreaction` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_ERAC2` FOREIGN KEY (`PrimaryWID`) REFERENCES `chemical` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_ERAC3` FOREIGN KEY (`AlternativeWID`) REFERENCES `chemical` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




