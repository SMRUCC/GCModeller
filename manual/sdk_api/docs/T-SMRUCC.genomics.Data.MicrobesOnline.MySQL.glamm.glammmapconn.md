---
title: glammmapconn
---

# glammmapconn
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.glamm](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.glamm.html)_

--
 
 DROP TABLE IF EXISTS `glammmapconn`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `glammmapconn` (
 `guid` bigint(10) unsigned NOT NULL,
 `version` int(2) unsigned NOT NULL DEFAULT '1',
 `priority` tinyint(3) unsigned DEFAULT NULL,
 `created` date DEFAULT NULL,
 `mapTitle` text NOT NULL,
 `cpd0ExtId` varchar(50) NOT NULL DEFAULT '',
 `cpd0SvgId` varchar(50) NOT NULL DEFAULT '',
 `cpd1ExtId` varchar(50) NOT NULL DEFAULT '',
 `cpd1SvgId` varchar(50) NOT NULL DEFAULT '',
 `rxnExtId` varchar(50) NOT NULL DEFAULT '',
 `rxnSvgId` varchar(50) NOT NULL DEFAULT '',
 PRIMARY KEY (`guid`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




