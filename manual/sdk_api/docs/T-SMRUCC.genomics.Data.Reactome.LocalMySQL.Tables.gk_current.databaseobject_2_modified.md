---
title: databaseobject_2_modified
---

# databaseobject_2_modified
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `databaseobject_2_modified`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `databaseobject_2_modified` (
 `DB_ID` int(10) unsigned DEFAULT NULL,
 `modified_rank` int(10) unsigned DEFAULT NULL,
 `modified` int(10) unsigned DEFAULT NULL,
 `modified_class` varchar(64) DEFAULT NULL,
 KEY `DB_ID` (`DB_ID`),
 KEY `modified` (`modified`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




