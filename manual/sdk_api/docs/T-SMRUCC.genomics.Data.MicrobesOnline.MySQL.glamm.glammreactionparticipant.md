---
title: glammreactionparticipant
---

# glammreactionparticipant
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.glamm](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.glamm.html)_

--
 
 DROP TABLE IF EXISTS `glammreactionparticipant`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `glammreactionparticipant` (
 `guid` bigint(10) unsigned NOT NULL,
 `version` int(2) unsigned NOT NULL DEFAULT '1',
 `priority` tinyint(3) unsigned DEFAULT NULL,
 `created` date DEFAULT NULL,
 `reactionGuid` bigint(10) unsigned NOT NULL,
 `compoundGuid` bigint(10) unsigned NOT NULL,
 `coefficient` tinyint(3) unsigned NOT NULL DEFAULT '1',
 `pType` enum('REACTANT','PRODUCT') NOT NULL,
 PRIMARY KEY (`guid`),
 KEY `reactionGuid` (`reactionGuid`),
 KEY `compoundGuid` (`compoundGuid`),
 KEY `pType` (`pType`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




