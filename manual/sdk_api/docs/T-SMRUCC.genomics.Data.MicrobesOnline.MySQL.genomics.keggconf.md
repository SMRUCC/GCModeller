---
title: keggconf
---

# keggconf
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `keggconf`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `keggconf` (
 `mapId` varchar(20) DEFAULT NULL,
 `object` varchar(20) NOT NULL DEFAULT '',
 `type` int(1) NOT NULL DEFAULT '0',
 `url` varchar(255) NOT NULL DEFAULT '',
 `coord` varchar(100) DEFAULT NULL,
 UNIQUE KEY `combo` (`mapId`,`object`,`coord`),
 KEY `mapId` (`mapId`),
 KEY `object` (`object`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




