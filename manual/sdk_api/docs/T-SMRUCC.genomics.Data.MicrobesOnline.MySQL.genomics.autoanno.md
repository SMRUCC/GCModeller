---
title: autoanno
---

# autoanno
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `autoanno`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `autoanno` (
 `db` varchar(20) DEFAULT NULL,
 `locusId` int(10) unsigned NOT NULL DEFAULT '0',
 `version` int(2) unsigned DEFAULT '1',
 `subject` longtext NOT NULL,
 `identity` float unsigned NOT NULL DEFAULT '0',
 `alignLength` int(10) unsigned NOT NULL DEFAULT '0',
 `mismatch` int(10) unsigned NOT NULL DEFAULT '0',
 `gap` int(10) unsigned NOT NULL DEFAULT '0',
 `qBegin` int(10) unsigned NOT NULL DEFAULT '0',
 `qEnd` int(10) unsigned NOT NULL DEFAULT '0',
 `sBegin` int(10) unsigned NOT NULL DEFAULT '0',
 `sEnd` int(10) unsigned NOT NULL DEFAULT '0',
 `evalue` float NOT NULL DEFAULT '0',
 `score` float NOT NULL DEFAULT '0',
 PRIMARY KEY (`locusId`,`subject`(200),`identity`,`qBegin`,`score`),
 KEY `db` (`db`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




