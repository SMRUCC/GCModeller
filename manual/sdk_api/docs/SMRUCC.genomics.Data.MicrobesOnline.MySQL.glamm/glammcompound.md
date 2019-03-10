﻿# glammcompound
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.glamm](./index.md)_

--
 
 DROP TABLE IF EXISTS `glammcompound`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `glammcompound` (
 `guid` bigint(10) unsigned NOT NULL,
 `version` int(2) unsigned NOT NULL DEFAULT '1',
 `priority` tinyint(3) unsigned DEFAULT NULL,
 `created` date DEFAULT NULL,
 `commonName` text NOT NULL,
 `mass` float DEFAULT NULL,
 `formula` varchar(255) DEFAULT NULL,
 `smiles` text,
 `inchi` text,
 PRIMARY KEY (`guid`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




