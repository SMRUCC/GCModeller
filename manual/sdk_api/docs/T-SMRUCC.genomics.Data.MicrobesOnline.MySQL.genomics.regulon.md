---
title: regulon
---

# regulon
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `regulon`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `regulon` (
 `taxId` int(20) unsigned NOT NULL,
 `proteinId` varchar(255) NOT NULL,
 `name` varchar(255) DEFAULT NULL,
 PRIMARY KEY (`proteinId`),
 KEY `taxId` (`taxId`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




