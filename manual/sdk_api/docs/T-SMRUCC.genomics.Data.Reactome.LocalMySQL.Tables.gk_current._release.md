---
title: _release
---

# _release
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `_release`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `_release` (
 `DB_ID` int(10) unsigned NOT NULL,
 `releaseDate` text,
 `releaseNumber` int(10) DEFAULT NULL,
 PRIMARY KEY (`DB_ID`),
 KEY `releaseNumber` (`releaseNumber`),
 FULLTEXT KEY `releaseDate` (`releaseDate`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




