---
title: taxname
---

# taxname
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `taxname`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `taxname` (
 `taxonomyId` int(20) DEFAULT NULL,
 `name` text NOT NULL,
 `uniqueName` text,
 `class` varchar(255) DEFAULT NULL,
 UNIQUE KEY `combined` (`taxonomyId`,`name`(200),`uniqueName`(100),`class`(100)),
 KEY `taxId` (`taxonomyId`),
 KEY `class` (`class`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




