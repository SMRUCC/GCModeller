---
title: orthologgroup
---

# orthologgroup
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `orthologgroup`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `orthologgroup` (
 `treeId` int(10) unsigned NOT NULL DEFAULT '0',
 `ogId` int(10) unsigned NOT NULL DEFAULT '0',
 `parentOG` int(10) unsigned DEFAULT NULL,
 `isDuplication` tinyint(1) DEFAULT NULL,
 `nGenes` int(10) unsigned NOT NULL DEFAULT '0',
 `nGenomes` int(10) unsigned NOT NULL DEFAULT '0',
 `nNonUniqueGenomes` int(10) unsigned NOT NULL DEFAULT '0',
 `splitTaxId` int(10) DEFAULT NULL,
 PRIMARY KEY (`treeId`,`ogId`),
 KEY `treeId` (`treeId`,`ogId`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




