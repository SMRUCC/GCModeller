---
title: scaffoldisactiveflag
---

# scaffoldisactiveflag
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `scaffoldisactiveflag`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `scaffoldisactiveflag` (
 `isActive` int(10) unsigned NOT NULL DEFAULT '0',
 `description` varchar(255) NOT NULL DEFAULT '',
 PRIMARY KEY (`isActive`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




