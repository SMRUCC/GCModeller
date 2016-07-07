---
title: mogcomponent
---

# mogcomponent
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `mogcomponent`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `mogcomponent` (
 `mogId` int(10) unsigned NOT NULL DEFAULT '0',
 `treeId` int(10) unsigned NOT NULL DEFAULT '0',
 `ogId` int(10) unsigned NOT NULL DEFAULT '0',
 `metric` float NOT NULL DEFAULT '0',
 `nMembers` int(10) unsigned NOT NULL DEFAULT '0',
 `nMembersBest` int(10) unsigned NOT NULL DEFAULT '0',
 KEY `mogId` (`mogId`),
 KEY `treeId` (`treeId`,`ogId`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




