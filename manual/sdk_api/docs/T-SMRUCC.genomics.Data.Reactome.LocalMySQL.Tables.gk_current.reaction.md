---
title: reaction
---

# reaction
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `reaction`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `reaction` (
 `DB_ID` int(10) unsigned NOT NULL,
 `reverseReaction` int(10) unsigned DEFAULT NULL,
 `reverseReaction_class` varchar(64) DEFAULT NULL,
 `totalProt` text,
 `maxHomologues` text,
 `inferredProt` text,
 PRIMARY KEY (`DB_ID`),
 KEY `reverseReaction` (`reverseReaction`),
 FULLTEXT KEY `totalProt` (`totalProt`),
 FULLTEXT KEY `maxHomologues` (`maxHomologues`),
 FULLTEXT KEY `inferredProt` (`inferredProt`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




