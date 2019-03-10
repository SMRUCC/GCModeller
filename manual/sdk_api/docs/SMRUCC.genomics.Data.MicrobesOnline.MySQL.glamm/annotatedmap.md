﻿# annotatedmap
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.glamm](./index.md)_

--
 
 DROP TABLE IF EXISTS `annotatedmap`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `annotatedmap` (
 `id` bigint(20) NOT NULL AUTO_INCREMENT,
 `icon` varchar(255) NOT NULL,
 `mapId` varchar(255) NOT NULL,
 `svg` varchar(255) NOT NULL,
 `title` varchar(255) NOT NULL,
 PRIMARY KEY (`id`)
 ) ENGINE=MyISAM AUTO_INCREMENT=4 DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




