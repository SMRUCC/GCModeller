﻿# _deleted_2_replacementinstances
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](./index.md)_

--
 
 DROP TABLE IF EXISTS `_deleted_2_replacementinstances`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `_deleted_2_replacementinstances` (
 `DB_ID` int(10) unsigned DEFAULT NULL,
 `replacementInstances_rank` int(10) unsigned DEFAULT NULL,
 `replacementInstances` int(10) unsigned DEFAULT NULL,
 `replacementInstances_class` varchar(64) DEFAULT NULL,
 KEY `DB_ID` (`DB_ID`),
 KEY `replacementInstances` (`replacementInstances`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




