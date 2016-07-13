---
title: mogmember
---

# mogmember
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `mogmember`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `mogmember` (
 `mogId` int(10) unsigned NOT NULL DEFAULT '0',
 `locusId` int(10) unsigned NOT NULL DEFAULT '0',
 `version` int(2) unsigned NOT NULL DEFAULT '0',
 `minBegin` int(10) unsigned NOT NULL DEFAULT '0',
 `maxEnd` int(10) unsigned NOT NULL DEFAULT '0',
 `treeId` int(10) unsigned NOT NULL DEFAULT '0',
 `ogId` int(10) unsigned NOT NULL DEFAULT '0',
 `metric` float NOT NULL DEFAULT '0',
 `nAligned` int(10) unsigned DEFAULT NULL,
 `taxonomyId` int(10) DEFAULT NULL,
 UNIQUE KEY `locusId` (`locusId`,`version`),
 KEY `mogId` (`mogId`),
 KEY `taxonomyId` (`taxonomyId`),
 KEY `treeId` (`treeId`,`ogId`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




