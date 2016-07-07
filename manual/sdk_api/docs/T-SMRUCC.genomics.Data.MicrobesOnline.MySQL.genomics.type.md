---
title: type
---

# type
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `type`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `type` (
 `objectId` varchar(255) NOT NULL,
 `objectDesc` varchar(255) NOT NULL,
 `objectType` varchar(255) NOT NULL,
 UNIQUE KEY `combined` (`objectId`(250),`objectType`(250)),
 KEY `objectId` (`objectId`),
 KEY `objectDesc` (`objectDesc`),
 KEY `objectType` (`objectType`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




