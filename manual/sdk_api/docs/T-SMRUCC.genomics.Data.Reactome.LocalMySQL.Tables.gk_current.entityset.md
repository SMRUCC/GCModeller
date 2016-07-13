---
title: entityset
---

# entityset
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `entityset`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `entityset` (
 `DB_ID` int(10) unsigned NOT NULL,
 `isOrdered` enum('TRUE','FALSE') DEFAULT NULL,
 `totalProt` text,
 `inferredProt` text,
 `maxHomologues` text,
 PRIMARY KEY (`DB_ID`),
 KEY `isOrdered` (`isOrdered`),
 FULLTEXT KEY `totalProt` (`totalProt`),
 FULLTEXT KEY `inferredProt` (`inferredProt`),
 FULLTEXT KEY `maxHomologues` (`maxHomologues`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




