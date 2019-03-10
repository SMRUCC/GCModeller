﻿# object_external_db_link
_namespace: [SMRUCC.genomics.Data.RegulonDB.Tables](./index.md)_

--
 
 DROP TABLE IF EXISTS `object_external_db_link`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `object_external_db_link` (
 `object_id` char(12) NOT NULL,
 `external_db_id` char(12) NOT NULL,
 `ext_reference_id` varchar(255) NOT NULL
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




