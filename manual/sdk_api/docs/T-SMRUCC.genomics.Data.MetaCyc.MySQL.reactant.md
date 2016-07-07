---
title: reactant
---

# reactant
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `reactant`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `reactant` (
 `ReactionWID` bigint(20) NOT NULL,
 `OtherWID` bigint(20) NOT NULL,
 `Coefficient` smallint(6) NOT NULL,
 KEY `FK_Reactant` (`ReactionWID`),
 CONSTRAINT `FK_Reactant` FOREIGN KEY (`ReactionWID`) REFERENCES `reaction` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




