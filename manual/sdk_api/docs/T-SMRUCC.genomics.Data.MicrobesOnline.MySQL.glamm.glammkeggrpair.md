---
title: glammkeggrpair
---

# glammkeggrpair
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.glamm](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.glamm.html)_

--
 
 DROP TABLE IF EXISTS `glammkeggrpair`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `glammkeggrpair` (
 `guid` bigint(10) unsigned NOT NULL,
 `version` int(2) unsigned NOT NULL DEFAULT '1',
 `priority` tinyint(3) unsigned DEFAULT NULL,
 `created` date DEFAULT NULL,
 `reactionGuid` bigint(10) unsigned NOT NULL,
 `compound0Guid` bigint(10) unsigned NOT NULL,
 `compound1Guid` bigint(10) unsigned NOT NULL,
 `compound0KeggId` varchar(8) NOT NULL DEFAULT '',
 `compound1KeggId` varchar(8) NOT NULL DEFAULT '',
 `rpairRole` varchar(32) NOT NULL DEFAULT '',
 PRIMARY KEY (`guid`),
 KEY `reactionGuid` (`reactionGuid`),
 KEY `compound0Guid` (`compound0Guid`),
 KEY `compound1Guid` (`compound1Guid`),
 KEY `compound0KeggId` (`compound0KeggId`),
 KEY `compound1KeggId` (`compound1KeggId`),
 KEY `rpairRole` (`rpairRole`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




