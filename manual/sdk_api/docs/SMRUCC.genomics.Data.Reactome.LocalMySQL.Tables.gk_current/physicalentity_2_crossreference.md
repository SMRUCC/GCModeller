﻿# physicalentity_2_crossreference
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](./index.md)_

--
 
 DROP TABLE IF EXISTS `physicalentity_2_crossreference`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `physicalentity_2_crossreference` (
 `DB_ID` int(10) unsigned DEFAULT NULL,
 `crossReference_rank` int(10) unsigned DEFAULT NULL,
 `crossReference` int(10) unsigned DEFAULT NULL,
 `crossReference_class` varchar(64) DEFAULT NULL,
 KEY `DB_ID` (`DB_ID`),
 KEY `crossReference` (`crossReference`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




