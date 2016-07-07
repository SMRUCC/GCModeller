---
title: glammenzyme
---

# glammenzyme
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.glamm](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.glamm.html)_

--
 
 DROP TABLE IF EXISTS `glammenzyme`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `glammenzyme` (
 `guid` bigint(10) unsigned NOT NULL,
 `version` int(2) unsigned NOT NULL DEFAULT '1',
 `priority` tinyint(3) unsigned DEFAULT NULL,
 `created` date DEFAULT NULL,
 `reactionGuid` bigint(10) unsigned NOT NULL,
 `ecNum` varchar(20) NOT NULL DEFAULT '',
 `name` text NOT NULL,
 PRIMARY KEY (`guid`),
 KEY `reactionGuid` (`reactionGuid`),
 KEY `ecNum` (`ecNum`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




