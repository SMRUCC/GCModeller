---
title: groupusers
---

# groupusers
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `groupusers`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `groupusers` (
 `groupId` int(10) unsigned NOT NULL DEFAULT '0',
 `userId` int(10) unsigned NOT NULL DEFAULT '0',
 `active` tinyint(1) unsigned NOT NULL DEFAULT '0',
 `time` int(10) unsigned NOT NULL DEFAULT '0',
 KEY `groupId` (`groupId`),
 KEY `userId` (`userId`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




