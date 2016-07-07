---
title: regulonlinks
---

# regulonlinks
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `regulonlinks`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `regulonlinks` (
 `cluster1` int(10) unsigned NOT NULL DEFAULT '0',
 `cluster2` int(10) unsigned NOT NULL DEFAULT '0',
 `link` varchar(50) NOT NULL DEFAULT '',
 `score` decimal(10,3) unsigned DEFAULT NULL,
 `updated` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
 PRIMARY KEY (`cluster1`,`cluster2`,`link`),
 KEY `cluster1` (`cluster1`),
 KEY `cluster2` (`cluster2`),
 KEY `link` (`link`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




