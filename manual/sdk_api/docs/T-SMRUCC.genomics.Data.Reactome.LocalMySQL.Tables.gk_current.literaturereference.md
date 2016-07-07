---
title: literaturereference
---

# literaturereference
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `literaturereference`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `literaturereference` (
 `DB_ID` int(10) unsigned NOT NULL,
 `journal` varchar(255) DEFAULT NULL,
 `pages` text,
 `pubMedIdentifier` int(10) DEFAULT NULL,
 `volume` int(10) DEFAULT NULL,
 `year` int(10) DEFAULT NULL,
 PRIMARY KEY (`DB_ID`),
 KEY `journal` (`journal`),
 KEY `pubMedIdentifier` (`pubMedIdentifier`),
 KEY `volume` (`volume`),
 KEY `year` (`year`),
 FULLTEXT KEY `journal_fulltext` (`journal`),
 FULLTEXT KEY `pages` (`pages`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




