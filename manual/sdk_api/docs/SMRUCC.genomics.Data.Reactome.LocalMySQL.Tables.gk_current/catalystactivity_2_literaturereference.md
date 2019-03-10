﻿# catalystactivity_2_literaturereference
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](./index.md)_

--
 
 DROP TABLE IF EXISTS `catalystactivity_2_literaturereference`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `catalystactivity_2_literaturereference` (
 `DB_ID` int(10) unsigned DEFAULT NULL,
 `literatureReference_rank` int(10) unsigned DEFAULT NULL,
 `literatureReference` int(10) unsigned DEFAULT NULL,
 `literatureReference_class` varchar(64) DEFAULT NULL,
 KEY `DB_ID` (`DB_ID`),
 KEY `literatureReference` (`literatureReference`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




