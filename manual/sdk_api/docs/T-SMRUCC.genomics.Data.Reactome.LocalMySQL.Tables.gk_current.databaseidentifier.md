---
title: databaseidentifier
---

# databaseidentifier
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `databaseidentifier`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `databaseidentifier` (
 `DB_ID` int(10) unsigned NOT NULL,
 `identifier` varchar(20) DEFAULT NULL,
 `referenceDatabase` int(10) unsigned DEFAULT NULL,
 `referenceDatabase_class` varchar(64) DEFAULT NULL,
 PRIMARY KEY (`DB_ID`),
 KEY `identifier` (`identifier`),
 KEY `referenceDatabase` (`referenceDatabase`),
 FULLTEXT KEY `identifier_fulltext` (`identifier`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




