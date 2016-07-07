---
title: kegginfo
---

# kegginfo
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `kegginfo`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `kegginfo` (
 `hitId` varchar(100) NOT NULL DEFAULT '',
 `sLength` int(10) unsigned NOT NULL DEFAULT '0',
 `description` longtext NOT NULL,
 PRIMARY KEY (`hitId`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




