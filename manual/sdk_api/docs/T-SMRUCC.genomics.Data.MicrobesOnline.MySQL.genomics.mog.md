---
title: mog
---

# mog
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `mog`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `mog` (
 `mogId` int(10) unsigned NOT NULL DEFAULT '0',
 `nComponents` int(10) unsigned NOT NULL DEFAULT '0',
 `nLoci` int(10) unsigned NOT NULL DEFAULT '0',
 `metric` float NOT NULL DEFAULT '0',
 PRIMARY KEY (`mogId`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




