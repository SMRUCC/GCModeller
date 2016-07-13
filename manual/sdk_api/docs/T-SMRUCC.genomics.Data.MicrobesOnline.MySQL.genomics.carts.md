---
title: carts
---

# carts
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `carts`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `carts` (
 `cartId` int(10) unsigned NOT NULL AUTO_INCREMENT,
 `userId` int(10) unsigned NOT NULL DEFAULT '0',
 `name` varchar(32) NOT NULL DEFAULT '',
 `seqData` longtext NOT NULL,
 `seqCount` int(10) unsigned NOT NULL DEFAULT '0',
 `time` int(10) unsigned NOT NULL DEFAULT '0',
 `active` int(1) unsigned NOT NULL DEFAULT '0',
 PRIMARY KEY (`cartId`),
 KEY `userId` (`userId`)
 ) ENGINE=MyISAM AUTO_INCREMENT=9354 DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




