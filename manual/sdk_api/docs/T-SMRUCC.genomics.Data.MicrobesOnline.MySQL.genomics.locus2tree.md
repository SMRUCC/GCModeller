---
title: locus2tree
---

# locus2tree
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `locus2tree`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `locus2tree` (
 `treeId` int(10) unsigned NOT NULL DEFAULT '0',
 `locusId` bigint(10) unsigned NOT NULL DEFAULT '0',
 `version` int(2) unsigned NOT NULL DEFAULT '0',
 `aaTree` tinyint(1) NOT NULL DEFAULT '0',
 `begin` int(10) unsigned NOT NULL DEFAULT '0',
 `end` int(10) unsigned NOT NULL DEFAULT '0',
 `nAligned` int(10) unsigned NOT NULL DEFAULT '0',
 `score` decimal(5,1) unsigned DEFAULT NULL,
 `scaffoldId` bigint(10) unsigned DEFAULT NULL,
 `taxonomyId` bigint(20) DEFAULT NULL,
 KEY `locusId` (`locusId`),
 KEY `treeId` (`treeId`),
 KEY `treelocus` (`treeId`,`locusId`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




