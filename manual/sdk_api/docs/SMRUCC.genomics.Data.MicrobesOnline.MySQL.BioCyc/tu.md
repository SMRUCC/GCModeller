﻿# tu
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.BioCyc](./index.md)_

--
 
 DROP TABLE IF EXISTS `tu`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `tu` (
 `taxId` int(10) unsigned NOT NULL,
 `tuId` varchar(50) NOT NULL,
 `name` varchar(100) DEFAULT NULL,
 `evidence` varchar(255) DEFAULT NULL,
 PRIMARY KEY (`tuId`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




