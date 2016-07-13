---
title: enzreactioninhibitoractivator
---

# enzreactioninhibitoractivator
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `enzreactioninhibitoractivator`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `enzreactioninhibitoractivator` (
 `EnzymaticReactionWID` bigint(20) NOT NULL,
 `CompoundWID` bigint(20) NOT NULL,
 `InhibitOrActivate` char(1) DEFAULT NULL,
 `Mechanism` char(1) DEFAULT NULL,
 `PhysioRelevant` char(1) DEFAULT NULL,
 KEY `FK_EnzReactionIA1` (`EnzymaticReactionWID`),
 CONSTRAINT `FK_EnzReactionIA1` FOREIGN KEY (`EnzymaticReactionWID`) REFERENCES `enzymaticreaction` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




