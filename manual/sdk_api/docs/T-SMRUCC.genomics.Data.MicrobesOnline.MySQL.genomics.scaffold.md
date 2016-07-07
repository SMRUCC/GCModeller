---
title: scaffold
---

# scaffold
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `scaffold`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `scaffold` (
 `scaffoldId` int(10) unsigned NOT NULL AUTO_INCREMENT,
 `chr_num` int(10) unsigned DEFAULT '1',
 `isCircular` int(1) DEFAULT NULL,
 `length` int(10) unsigned DEFAULT NULL,
 `file` varchar(32) DEFAULT NULL,
 `isGenomic` int(1) DEFAULT NULL,
 `gi` int(10) unsigned DEFAULT NULL,
 `taxonomyId` int(10) DEFAULT '0',
 `comment` varchar(255) DEFAULT NULL,
 `isActive` int(1) DEFAULT '1',
 `isPartial` int(1) DEFAULT '0',
 `created` date DEFAULT '0000-00-00',
 `allowUpdates` tinyint(3) unsigned NOT NULL DEFAULT '1',
 `ncbiProjectId` int(10) DEFAULT NULL,
 `md5` char(32) DEFAULT NULL,
 PRIMARY KEY (`scaffoldId`),
 KEY `Indx_Scaffold_file` (`file`),
 KEY `comment` (`comment`),
 KEY `taxonomyId` (`taxonomyId`)
 ) ENGINE=MyISAM AUTO_INCREMENT=952581 DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




