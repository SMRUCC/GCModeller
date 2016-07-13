---
title: synonym
---

# synonym
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.BioCyc](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.BioCyc.html)_

--
 
 DROP TABLE IF EXISTS `synonym`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `synonym` (
 `objectId` varchar(255) NOT NULL,
 `synonym` varchar(255) NOT NULL,
 UNIQUE KEY `combined` (`objectId`(250),`synonym`(250)),
 KEY `objectId` (`objectId`),
 KEY `synonym` (`synonym`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




