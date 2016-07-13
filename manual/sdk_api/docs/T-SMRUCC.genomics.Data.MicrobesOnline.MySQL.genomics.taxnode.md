---
title: taxnode
---

# taxnode
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `taxnode`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `taxnode` (
 `taxonomyId` int(10) unsigned NOT NULL DEFAULT '0',
 `parentId` int(10) unsigned DEFAULT NULL,
 `rank` varchar(50) DEFAULT NULL,
 `embl` varchar(10) DEFAULT NULL,
 `divId` int(3) unsigned NOT NULL DEFAULT '0',
 `flag1` int(1) unsigned NOT NULL DEFAULT '0',
 `flag2` int(1) unsigned NOT NULL DEFAULT '0',
 `flag3` int(1) unsigned NOT NULL DEFAULT '0',
 `flag4` int(1) unsigned NOT NULL DEFAULT '0',
 `flag5` int(1) unsigned NOT NULL DEFAULT '0',
 `flag6` int(1) unsigned NOT NULL DEFAULT '0',
 `flag7` int(1) unsigned NOT NULL DEFAULT '0',
 `comments` varchar(255) DEFAULT NULL,
 PRIMARY KEY (`taxonomyId`),
 KEY `parentId` (`parentId`),
 KEY `rank` (`rank`),
 KEY `divisionId` (`divId`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




