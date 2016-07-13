---
title: component
---

# component
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.BioCyc](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.BioCyc.html)_

--
 
 DROP TABLE IF EXISTS `component`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `component` (
 `taxId` int(20) unsigned NOT NULL,
 `objectId` varchar(255) NOT NULL,
 `component` varchar(255) NOT NULL,
 UNIQUE KEY `combined` (`objectId`(250),`component`(250)),
 KEY `taxId` (`taxId`),
 KEY `objectId` (`objectId`),
 KEY `component` (`component`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




