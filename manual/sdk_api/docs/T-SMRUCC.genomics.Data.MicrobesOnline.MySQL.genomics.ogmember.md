---
title: ogmember
---

# ogmember
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `ogmember`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `ogmember` (
 `treeId` int(10) unsigned NOT NULL DEFAULT '0',
 `ogId` int(10) unsigned NOT NULL DEFAULT '0',
 `locusId` int(10) unsigned NOT NULL DEFAULT '0',
 `version` int(2) unsigned NOT NULL DEFAULT '0',
 `begin` int(5) unsigned NOT NULL DEFAULT '0',
 `end` int(5) unsigned NOT NULL DEFAULT '0',
 `taxonomyId` int(10) NOT NULL DEFAULT '0',
 `nMemberThisGenome` int(10) unsigned NOT NULL DEFAULT '0',
 `aaLength` int(10) unsigned NOT NULL DEFAULT '0',
 `nAligned` int(10) unsigned DEFAULT NULL,
 `score` decimal(5,1) unsigned DEFAULT NULL,
 KEY `treeId` (`treeId`,`ogId`),
 KEY `locusId_2` (`locusId`,`version`),
 KEY `treelocus` (`treeId`,`locusId`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




