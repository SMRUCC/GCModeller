---
title: description
---

# description
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `description`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `description` (
 `descriptionId` int(10) unsigned NOT NULL AUTO_INCREMENT,
 `description` text,
 `source` varchar(255) DEFAULT NULL,
 `created` date DEFAULT NULL,
 `locusId` int(10) unsigned DEFAULT NULL,
 `version` int(2) unsigned NOT NULL DEFAULT '1',
 PRIMARY KEY (`descriptionId`),
 KEY `OrfId_version` (`locusId`,`version`),
 KEY `locusId` (`locusId`),
 KEY `description` (`description`(150)),
 FULLTEXT KEY `description2` (`description`)
 ) ENGINE=MyISAM AUTO_INCREMENT=8014620 DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




