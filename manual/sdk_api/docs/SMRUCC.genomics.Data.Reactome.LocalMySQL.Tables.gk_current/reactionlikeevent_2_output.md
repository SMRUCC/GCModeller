﻿# reactionlikeevent_2_output
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](./index.md)_

--
 
 DROP TABLE IF EXISTS `reactionlikeevent_2_output`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `reactionlikeevent_2_output` (
 `DB_ID` int(10) unsigned DEFAULT NULL,
 `output_rank` int(10) unsigned DEFAULT NULL,
 `output` int(10) unsigned DEFAULT NULL,
 `output_class` varchar(64) DEFAULT NULL,
 KEY `DB_ID` (`DB_ID`),
 KEY `output` (`output`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




