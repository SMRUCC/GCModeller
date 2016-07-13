---
title: promoter
---

# promoter
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.BioCyc](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.BioCyc.html)_

--
 
 DROP TABLE IF EXISTS `promoter`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `promoter` (
 `taxId` int(20) unsigned NOT NULL,
 `promoterId` varchar(100) NOT NULL,
 `name` varchar(20) DEFAULT NULL,
 `evidence` varchar(100) DEFAULT NULL,
 PRIMARY KEY (`promoterId`),
 KEY `taxId` (`taxId`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




