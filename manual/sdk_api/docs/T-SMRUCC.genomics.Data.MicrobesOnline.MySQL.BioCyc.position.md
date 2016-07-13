---
title: position
---

# position
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.BioCyc](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.BioCyc.html)_

--
 
 DROP TABLE IF EXISTS `position`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `position` (
 `taxId` int(20) unsigned NOT NULL,
 `objectId` varchar(100) NOT NULL,
 `posLeft` int(50) unsigned DEFAULT NULL,
 `posRight` int(50) unsigned DEFAULT NULL,
 `strand` varchar(4) DEFAULT NULL,
 PRIMARY KEY (`objectId`),
 KEY `taxId` (`taxId`),
 KEY `posLeft` (`posLeft`),
 KEY `posRight` (`posRight`),
 KEY `strand` (`strand`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




