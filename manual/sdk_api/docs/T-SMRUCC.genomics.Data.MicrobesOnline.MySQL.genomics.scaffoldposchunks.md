---
title: scaffoldposchunks
---

# scaffoldposchunks
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `scaffoldposchunks`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `scaffoldposchunks` (
 `locusId` int(10) unsigned NOT NULL DEFAULT '0',
 `version` int(2) unsigned NOT NULL DEFAULT '0',
 `posId` int(10) unsigned NOT NULL DEFAULT '0',
 `scaffoldId` int(10) unsigned NOT NULL DEFAULT '0',
 `kbt` int(10) unsigned NOT NULL DEFAULT '0',
 PRIMARY KEY (`locusId`,`version`,`posId`,`scaffoldId`,`kbt`),
 KEY `locusIdVersion` (`locusId`,`version`),
 KEY `locusId` (`locusId`),
 KEY `scaffoldId` (`scaffoldId`),
 KEY `kbt` (`kbt`),
 KEY `scaffoldIdKbt` (`scaffoldId`,`kbt`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




