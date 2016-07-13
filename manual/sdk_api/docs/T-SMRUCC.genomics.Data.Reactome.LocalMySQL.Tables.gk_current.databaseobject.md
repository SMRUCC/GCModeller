---
title: databaseobject
---

# databaseobject
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `databaseobject`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `databaseobject` (
 `DB_ID` int(10) NOT NULL AUTO_INCREMENT,
 `_Protege_id` varchar(255) DEFAULT NULL,
 `__is_ghost` enum('TRUE') DEFAULT NULL,
 `_class` varchar(64) DEFAULT NULL,
 `_displayName` text,
 `_timestamp` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
 `created` int(10) unsigned DEFAULT NULL,
 `created_class` varchar(64) DEFAULT NULL,
 `stableIdentifier` int(10) unsigned DEFAULT NULL,
 `stableIdentifier_class` varchar(64) DEFAULT NULL,
 PRIMARY KEY (`DB_ID`),
 KEY `_Protege_id` (`_Protege_id`),
 KEY `__is_ghost` (`__is_ghost`),
 KEY `_class` (`_class`),
 KEY `_timestamp` (`_timestamp`),
 KEY `created` (`created`),
 KEY `stableIdentifier` (`stableIdentifier`),
 FULLTEXT KEY `_Protege_id_fulltext` (`_Protege_id`),
 FULLTEXT KEY `_class_fulltext` (`_class`),
 FULLTEXT KEY `_displayName` (`_displayName`)
 ) ENGINE=MyISAM AUTO_INCREMENT=8835475 DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




