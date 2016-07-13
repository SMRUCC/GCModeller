---
title: dnabindsite
---

# dnabindsite
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `dnabindsite`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `dnabindsite` (
 `taxId` int(20) unsigned NOT NULL,
 `bsId` varchar(100) NOT NULL,
 `comment` text,
 `name` varchar(255) DEFAULT NULL,
 `promoterID` varchar(255) DEFAULT NULL,
 `centerDist` float DEFAULT NULL,
 `evidence` varchar(255) DEFAULT NULL,
 PRIMARY KEY (`bsId`),
 KEY `taxId` (`taxId`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




