﻿# go_cellularcomponent_2_instanceof
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](./index.md)_

--
 
 DROP TABLE IF EXISTS `go_cellularcomponent_2_instanceof`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `go_cellularcomponent_2_instanceof` (
 `DB_ID` int(10) unsigned DEFAULT NULL,
 `instanceOf_rank` int(10) unsigned DEFAULT NULL,
 `instanceOf` int(10) unsigned DEFAULT NULL,
 `instanceOf_class` varchar(64) DEFAULT NULL,
 KEY `DB_ID` (`DB_ID`),
 KEY `instanceOf` (`instanceOf`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




