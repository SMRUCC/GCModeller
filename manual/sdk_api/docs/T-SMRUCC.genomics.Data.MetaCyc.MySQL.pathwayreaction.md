---
title: pathwayreaction
---

# pathwayreaction
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `pathwayreaction`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `pathwayreaction` (
 `PathwayWID` bigint(20) NOT NULL,
 `ReactionWID` bigint(20) NOT NULL,
 `PriorReactionWID` bigint(20) DEFAULT NULL,
 `Hypothetical` char(1) NOT NULL,
 KEY `PR_PATHWID_REACTIONWID` (`PathwayWID`,`ReactionWID`),
 KEY `FK_PathwayReaction3` (`PriorReactionWID`),
 CONSTRAINT `FK_PathwayReaction1` FOREIGN KEY (`PathwayWID`) REFERENCES `pathway` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_PathwayReaction3` FOREIGN KEY (`PriorReactionWID`) REFERENCES `reaction` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




