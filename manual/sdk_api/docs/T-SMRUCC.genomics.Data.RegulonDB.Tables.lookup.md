---
title: lookup
---

# lookup
_namespace: [SMRUCC.genomics.Data.RegulonDB.Tables](N-SMRUCC.genomics.Data.RegulonDB.Tables.html)_

--
 
 DROP TABLE IF EXISTS `lookup`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `lookup` (
 `lookup_id` varchar(16) DEFAULT NULL,
 `lookup_name` varchar(2000) DEFAULT NULL,
 `lookup_category` varchar(100) NOT NULL,
 `lookup_source` varchar(20) NOT NULL,
 `lookup_reference` varchar(500) DEFAULT NULL,
 `lookup_object_id` varchar(255) DEFAULT NULL,
 `lookup_accesion_id` varchar(100) NOT NULL,
 `lookup_context` varchar(100) DEFAULT NULL,
 `lookup_description` varchar(1000) NOT NULL,
 `lookup_lastupdate` varchar(100) DEFAULT NULL,
 `lookup_url` varchar(2000) NOT NULL
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




