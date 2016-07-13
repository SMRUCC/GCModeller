---
title: gocount
---

# gocount
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `gocount`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `gocount` (
 `goId` int(10) unsigned NOT NULL DEFAULT '0',
 `goCount` int(10) unsigned NOT NULL DEFAULT '0',
 `taxId` int(10) DEFAULT NULL,
 UNIQUE KEY `tax2go` (`taxId`,`goId`),
 KEY `taxId` (`taxId`),
 KEY `goId` (`goId`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




