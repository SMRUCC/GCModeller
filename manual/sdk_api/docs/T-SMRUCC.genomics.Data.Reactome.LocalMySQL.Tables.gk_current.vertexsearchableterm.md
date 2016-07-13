---
title: vertexsearchableterm
---

# vertexsearchableterm
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `vertexsearchableterm`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `vertexsearchableterm` (
 `DB_ID` int(10) unsigned NOT NULL,
 `providerCount` int(10) DEFAULT NULL,
 `searchableTerm` varchar(255) DEFAULT NULL,
 `species` int(10) unsigned DEFAULT NULL,
 `species_class` varchar(64) DEFAULT NULL,
 `vertexCount` int(10) DEFAULT NULL,
 PRIMARY KEY (`DB_ID`),
 KEY `providerCount` (`providerCount`),
 KEY `searchableTerm` (`searchableTerm`),
 KEY `species` (`species`),
 KEY `vertexCount` (`vertexCount`),
 FULLTEXT KEY `searchableTerm_fulltext` (`searchableTerm`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




