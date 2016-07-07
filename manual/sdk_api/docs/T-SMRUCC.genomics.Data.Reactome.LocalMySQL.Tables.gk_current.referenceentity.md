---
title: referenceentity
---

# referenceentity
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `referenceentity`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `referenceentity` (
 `DB_ID` int(10) unsigned NOT NULL,
 `identifier` text,
 `referenceDatabase` int(10) unsigned DEFAULT NULL,
 `referenceDatabase_class` varchar(64) DEFAULT NULL,
 PRIMARY KEY (`DB_ID`),
 KEY `referenceDatabase` (`referenceDatabase`),
 FULLTEXT KEY `identifier` (`identifier`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




