---
title: operon
---

# operon
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `operon`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `operon` (
 `tuId` int(10) unsigned NOT NULL AUTO_INCREMENT,
 `posId` int(10) unsigned NOT NULL DEFAULT '0',
 `evidence` varchar(255) NOT NULL DEFAULT '',
 PRIMARY KEY (`tuId`),
 UNIQUE KEY `posId` (`posId`)
 ) ENGINE=MyISAM AUTO_INCREMENT=32456483 DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




