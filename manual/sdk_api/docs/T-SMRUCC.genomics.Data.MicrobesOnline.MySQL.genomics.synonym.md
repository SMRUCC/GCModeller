---
title: synonym
---

# synonym
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `synonym`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `synonym` (
 `name` varchar(100) DEFAULT NULL,
 `locusId` int(10) unsigned DEFAULT NULL,
 `version` int(2) unsigned DEFAULT '1',
 `type` int(2) unsigned DEFAULT NULL,
 `created` date DEFAULT NULL,
 `source` varchar(50) DEFAULT NULL,
 KEY `orfId` (`locusId`),
 KEY `orfId_version` (`locusId`,`version`),
 KEY `name` (`name`),
 KEY `type` (`type`),
 KEY `locusId_type_version` (`locusId`,`type`,`version`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




