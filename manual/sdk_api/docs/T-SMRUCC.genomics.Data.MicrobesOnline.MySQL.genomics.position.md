---
title: position
---

# position
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `position`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `position` (
 `posId` int(10) unsigned NOT NULL AUTO_INCREMENT,
 `strand` enum('+','-') NOT NULL DEFAULT '+',
 `begin` int(10) unsigned DEFAULT NULL,
 `end` int(10) unsigned DEFAULT NULL,
 `scaffoldId` int(10) unsigned DEFAULT NULL,
 `objectId` int(2) unsigned DEFAULT '1',
 PRIMARY KEY (`posId`),
 KEY `Indx_Position_scaffoldId` (`scaffoldId`),
 KEY `objectId` (`objectId`),
 KEY `start` (`begin`),
 KEY `end` (`end`)
 ) ENGINE=MyISAM AUTO_INCREMENT=44218375 DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




