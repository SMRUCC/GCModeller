---
title: groups
---

# groups
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `groups`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `groups` (
 `groupId` int(10) unsigned NOT NULL AUTO_INCREMENT,
 `name` varchar(32) NOT NULL DEFAULT '',
 `description` varchar(255) NOT NULL DEFAULT '',
 `adminUserId` int(10) unsigned NOT NULL DEFAULT '0',
 PRIMARY KEY (`groupId`),
 UNIQUE KEY `name` (`name`),
 KEY `adminUserId` (`adminUserId`)
 ) ENGINE=MyISAM AUTO_INCREMENT=10296746 DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




