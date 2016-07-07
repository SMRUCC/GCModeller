---
title: taxonomy2tree
---

# taxonomy2tree
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `taxonomy2tree`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `taxonomy2tree` (
 `treeId` int(10) unsigned NOT NULL DEFAULT '0',
 `taxonomyId` int(10) NOT NULL DEFAULT '0',
 KEY `treeId` (`treeId`),
 KEY `taxonomyId` (`taxonomyId`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




