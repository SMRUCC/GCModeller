---
title: pfamclan
---

# pfamclan
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `pfamclan`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `pfamclan` (
 `clanId` varchar(10) NOT NULL DEFAULT '',
 `domainId` varchar(10) NOT NULL DEFAULT '',
 KEY `clanId` (`clanId`),
 KEY `domainId` (`domainId`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




