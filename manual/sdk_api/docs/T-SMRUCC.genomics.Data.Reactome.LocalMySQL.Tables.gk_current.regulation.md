---
title: regulation
---

# regulation
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `regulation`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `regulation` (
 `DB_ID` int(10) unsigned NOT NULL,
 `authored` int(10) unsigned DEFAULT NULL,
 `authored_class` varchar(64) DEFAULT NULL,
 `regulatedEntity` int(10) unsigned DEFAULT NULL,
 `regulatedEntity_class` varchar(64) DEFAULT NULL,
 `regulationType` int(10) unsigned DEFAULT NULL,
 `regulationType_class` varchar(64) DEFAULT NULL,
 `regulator` int(10) unsigned DEFAULT NULL,
 `regulator_class` varchar(64) DEFAULT NULL,
 `releaseDate` date DEFAULT NULL,
 PRIMARY KEY (`DB_ID`),
 KEY `authored` (`authored`),
 KEY `regulatedEntity` (`regulatedEntity`),
 KEY `regulationType` (`regulationType`),
 KEY `regulator` (`regulator`),
 KEY `releaseDate` (`releaseDate`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




