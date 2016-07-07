---
title: glammreaction
---

# glammreaction
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.glamm](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.glamm.html)_

--
 
 DROP TABLE IF EXISTS `glammreaction`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `glammreaction` (
 `guid` bigint(10) unsigned NOT NULL,
 `version` int(2) unsigned NOT NULL DEFAULT '1',
 `priority` tinyint(3) unsigned DEFAULT NULL,
 `created` date DEFAULT NULL,
 `deltaG` float DEFAULT NULL,
 `equation` text NOT NULL,
 `normalizedEquation` text NOT NULL,
 `definition` text NOT NULL,
 PRIMARY KEY (`guid`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




