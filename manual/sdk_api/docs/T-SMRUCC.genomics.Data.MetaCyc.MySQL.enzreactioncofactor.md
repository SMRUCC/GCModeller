---
title: enzreactioncofactor
---

# enzreactioncofactor
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `enzreactioncofactor`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `enzreactioncofactor` (
 `EnzymaticReactionWID` bigint(20) NOT NULL,
 `ChemicalWID` bigint(20) NOT NULL,
 `Prosthetic` char(1) DEFAULT NULL,
 KEY `FK_EnzReactionCofactor1` (`EnzymaticReactionWID`),
 KEY `FK_EnzReactionCofactor2` (`ChemicalWID`),
 CONSTRAINT `FK_EnzReactionCofactor1` FOREIGN KEY (`EnzymaticReactionWID`) REFERENCES `enzymaticreaction` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_EnzReactionCofactor2` FOREIGN KEY (`ChemicalWID`) REFERENCES `chemical` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




