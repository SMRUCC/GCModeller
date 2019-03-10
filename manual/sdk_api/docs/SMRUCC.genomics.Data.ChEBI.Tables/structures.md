﻿# structures
_namespace: [SMRUCC.genomics.Data.ChEBI.Tables](./index.md)_

--
 
 DROP TABLE IF EXISTS `structures`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `structures` (
 `id` int(11) NOT NULL,
 `compound_id` int(11) NOT NULL,
 `structure` text NOT NULL,
 `type` text NOT NULL,
 `dimension` text NOT NULL,
 PRIMARY KEY (`id`),
 KEY `FK_STRUCTURES_TO_COMPOUND` (`compound_id`),
 CONSTRAINT `FK_STRUCTURES_TO_COMPOUND` FOREIGN KEY (`compound_id`) REFERENCES `compounds` (`id`)
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




