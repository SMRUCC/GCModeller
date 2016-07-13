---
title: map_tmp
---

# map_tmp
_namespace: [SMRUCC.genomics.Data.RegulonDB.Tables](N-SMRUCC.genomics.Data.RegulonDB.Tables.html)_

--
 
 DROP TABLE IF EXISTS `map_tmp`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `map_tmp` (
 `map_id` varchar(12) DEFAULT NULL,
 `map_name` varchar(250) DEFAULT NULL,
 `map_description` varchar(4000) DEFAULT NULL,
 `map_type` varchar(1) DEFAULT NULL,
 `map_component` varchar(2000) DEFAULT NULL,
 `map_reaction_name` varchar(2000) DEFAULT NULL,
 `map_source` varchar(255) DEFAULT NULL,
 `map_status` varchar(255) DEFAULT NULL,
 `map_file_name` varchar(250) DEFAULT NULL
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




