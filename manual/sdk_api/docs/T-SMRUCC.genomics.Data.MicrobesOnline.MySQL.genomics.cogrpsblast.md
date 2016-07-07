---
title: cogrpsblast
---

# cogrpsblast
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `cogrpsblast`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `cogrpsblast` (
 `locusId` int(10) unsigned NOT NULL DEFAULT '0',
 `version` int(2) unsigned NOT NULL DEFAULT '1',
 `subject` varchar(20) NOT NULL DEFAULT '',
 `identity` float unsigned NOT NULL DEFAULT '0',
 `alignLength` int(10) unsigned NOT NULL DEFAULT '0',
 `mismatch` int(10) unsigned NOT NULL DEFAULT '0',
 `gap` int(10) unsigned NOT NULL DEFAULT '0',
 `qBegin` int(10) unsigned NOT NULL DEFAULT '0',
 `qEnd` int(10) unsigned NOT NULL DEFAULT '0',
 `sBegin` int(10) unsigned NOT NULL DEFAULT '0',
 `sEnd` int(10) unsigned NOT NULL DEFAULT '0',
 `evalue` double NOT NULL DEFAULT '0',
 `score` float NOT NULL DEFAULT '0',
 PRIMARY KEY (`locusId`,`version`,`subject`,`qBegin`,`qEnd`,`sBegin`,`sEnd`),
 KEY `locusId` (`locusId`,`version`),
 KEY `subject` (`subject`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




