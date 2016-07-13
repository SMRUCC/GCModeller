---
title: coginfo
---

# coginfo
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `coginfo`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `coginfo` (
 `cogInfoId` int(10) unsigned NOT NULL DEFAULT '0',
 `funCode` varchar(5) NOT NULL DEFAULT '',
 `description` varchar(255) DEFAULT NULL,
 `geneName` varchar(20) DEFAULT NULL,
 `cddId` varchar(255) DEFAULT NULL,
 `length` int(10) unsigned DEFAULT NULL,
 PRIMARY KEY (`cogInfoId`),
 KEY `description` (`description`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




