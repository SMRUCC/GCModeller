﻿# physicalentity
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current_dn](./index.md)_

--
 
 DROP TABLE IF EXISTS `physicalentity`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `physicalentity` (
 `id` int(32) NOT NULL,
 `displayName` varchar(255) NOT NULL,
 `species` varchar(255) DEFAULT NULL,
 `class` varchar(255) NOT NULL,
 `stableId` varchar(32) DEFAULT NULL,
 PRIMARY KEY (`id`),
 UNIQUE KEY `stableId` (`stableId`)
 ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




