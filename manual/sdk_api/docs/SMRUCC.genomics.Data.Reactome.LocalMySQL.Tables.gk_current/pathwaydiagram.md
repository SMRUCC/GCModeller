﻿# pathwaydiagram
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](./index.md)_

--
 
 DROP TABLE IF EXISTS `pathwaydiagram`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `pathwaydiagram` (
 `DB_ID` int(10) unsigned NOT NULL,
 `height` int(10) DEFAULT NULL,
 `storedATXML` longblob,
 `width` int(10) DEFAULT NULL,
 PRIMARY KEY (`DB_ID`),
 KEY `height` (`height`),
 KEY `width` (`width`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




