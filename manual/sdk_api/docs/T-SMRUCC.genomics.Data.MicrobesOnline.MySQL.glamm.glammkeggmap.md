---
title: glammkeggmap
---

# glammkeggmap
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.glamm](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.glamm.html)_

--
 
 DROP TABLE IF EXISTS `glammkeggmap`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `glammkeggmap` (
 `guid` bigint(10) unsigned NOT NULL,
 `version` int(2) unsigned NOT NULL DEFAULT '1',
 `priority` tinyint(3) unsigned DEFAULT NULL,
 `created` date DEFAULT NULL,
 `fromGuid` bigint(10) DEFAULT NULL,
 `mapId` varchar(20) DEFAULT NULL,
 `title` varchar(255) NOT NULL DEFAULT '',
 PRIMARY KEY (`guid`),
 KEY `mapId` (`mapId`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




