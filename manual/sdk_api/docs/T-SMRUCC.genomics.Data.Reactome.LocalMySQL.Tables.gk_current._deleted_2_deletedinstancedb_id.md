---
title: _deleted_2_deletedinstancedb_id
---

# _deleted_2_deletedinstancedb_id
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `_deleted_2_deletedinstancedb_id`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `_deleted_2_deletedinstancedb_id` (
 `DB_ID` int(10) unsigned DEFAULT NULL,
 `deletedInstanceDB_ID_rank` int(10) unsigned DEFAULT NULL,
 `deletedInstanceDB_ID` int(10) DEFAULT NULL,
 KEY `DB_ID` (`DB_ID`),
 KEY `deletedInstanceDB_ID` (`deletedInstanceDB_ID`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




