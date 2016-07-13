---
title: class
---

# class
_namespace: [SMRUCC.genomics.Data.ExplorEnz.MySQL](N-SMRUCC.genomics.Data.ExplorEnz.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `class`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `class` (
 `id` int(11) NOT NULL AUTO_INCREMENT,
 `class` int(11) NOT NULL DEFAULT '0',
 `subclass` int(11) DEFAULT NULL,
 `subsubclass` int(11) DEFAULT NULL,
 `heading` varchar(255) DEFAULT NULL,
 `note` text,
 `last_change` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
 PRIMARY KEY (`id`)
 ) ENGINE=MyISAM AUTO_INCREMENT=631 DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




