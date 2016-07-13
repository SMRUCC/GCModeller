---
title: glammdatasource
---

# glammdatasource
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.glamm](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.glamm.html)_

--
 
 DROP TABLE IF EXISTS `glammdatasource`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `glammdatasource` (
 `guid` bigint(10) unsigned NOT NULL,
 `version` int(2) unsigned NOT NULL DEFAULT '1',
 `priority` tinyint(3) unsigned DEFAULT NULL,
 `created` date DEFAULT NULL,
 `description` text,
 `organization` varchar(255) DEFAULT NULL,
 `dbName` varchar(255) DEFAULT NULL,
 `dbVersion` varchar(255) DEFAULT NULL,
 PRIMARY KEY (`guid`),
 KEY `dbName` (`dbName`),
 KEY `organization` (`organization`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




