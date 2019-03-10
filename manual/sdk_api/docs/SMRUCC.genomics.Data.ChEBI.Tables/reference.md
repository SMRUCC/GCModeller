﻿# reference
_namespace: [SMRUCC.genomics.Data.ChEBI.Tables](./index.md)_

--
 
 DROP TABLE IF EXISTS `reference`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `reference` (
 `id` int(11) NOT NULL,
 `compound_id` int(11) NOT NULL,
 `reference_id` varchar(60) NOT NULL,
 `reference_db_name` varchar(60) NOT NULL,
 `location_in_ref` varchar(90) DEFAULT NULL,
 `reference_name` varchar(512) DEFAULT NULL,
 PRIMARY KEY (`id`),
 KEY `compound_id` (`compound_id`),
 CONSTRAINT `FK_REFERENCE_TO_COMPOUND` FOREIGN KEY (`compound_id`) REFERENCES `compounds` (`id`)
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




