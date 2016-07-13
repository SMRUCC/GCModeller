---
title: cog
---

# cog
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `cog`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `cog` (
 `cogId` int(10) unsigned NOT NULL AUTO_INCREMENT,
 `locusId` bigint(20) unsigned NOT NULL,
 `version` int(2) unsigned NOT NULL DEFAULT '1',
 `cogInfoId` int(10) unsigned NOT NULL DEFAULT '0',
 PRIMARY KEY (`cogId`),
 UNIQUE KEY `combined` (`locusId`,`cogInfoId`),
 UNIQUE KEY `orfId` (`locusId`,`version`),
 KEY `cogInfoId` (`cogInfoId`)
 ) ENGINE=MyISAM AUTO_INCREMENT=4644952 DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




