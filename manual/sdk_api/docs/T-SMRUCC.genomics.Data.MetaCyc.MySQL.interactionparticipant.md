---
title: interactionparticipant
---

# interactionparticipant
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `interactionparticipant`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `interactionparticipant` (
 `InteractionWID` bigint(20) NOT NULL,
 `OtherWID` bigint(20) NOT NULL,
 `Coefficient` smallint(6) DEFAULT NULL,
 KEY `PR_INTERACTIONWID_OTHERWID` (`InteractionWID`,`OtherWID`),
 CONSTRAINT `FK_InteractionParticipant1` FOREIGN KEY (`InteractionWID`) REFERENCES `interaction` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




