---
title: name
---

# name
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_stable_ids](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_stable_ids.html)_

--
 
 DROP TABLE IF EXISTS `name`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `name` (
 `DB_ID` int(12) unsigned NOT NULL AUTO_INCREMENT,
 `ST_ID` int(12) unsigned NOT NULL,
 `name` text NOT NULL,
 PRIMARY KEY (`DB_ID`)
 ) ENGINE=MyISAM AUTO_INCREMENT=1639545 DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




