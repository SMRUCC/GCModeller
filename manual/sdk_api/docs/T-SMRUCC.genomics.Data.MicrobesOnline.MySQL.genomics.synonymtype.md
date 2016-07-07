---
title: synonymtype
---

# synonymtype
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `synonymtype`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `synonymtype` (
 `type` int(10) unsigned NOT NULL DEFAULT '0',
 `description` varchar(255) NOT NULL DEFAULT '',
 PRIMARY KEY (`type`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




