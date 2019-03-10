﻿# regulation_2_revised
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](./index.md)_

--
 
 DROP TABLE IF EXISTS `regulation_2_revised`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `regulation_2_revised` (
 `DB_ID` int(10) unsigned DEFAULT NULL,
 `revised_rank` int(10) unsigned DEFAULT NULL,
 `revised` int(10) unsigned DEFAULT NULL,
 `revised_class` varchar(64) DEFAULT NULL,
 KEY `DB_ID` (`DB_ID`),
 KEY `revised` (`revised`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




