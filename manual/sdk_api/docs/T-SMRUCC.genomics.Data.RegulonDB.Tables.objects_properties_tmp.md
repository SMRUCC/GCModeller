---
title: objects_properties_tmp
---

# objects_properties_tmp
_namespace: [SMRUCC.genomics.Data.RegulonDB.Tables](N-SMRUCC.genomics.Data.RegulonDB.Tables.html)_

--
 
 DROP TABLE IF EXISTS `objects_properties_tmp`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `objects_properties_tmp` (
 `object_id` varchar(12) DEFAULT NULL,
 `object_name` varchar(255) DEFAULT NULL,
 `object_type` varchar(50) NOT NULL,
 `object_strand` varchar(7) DEFAULT NULL,
 `object_posleft` decimal(10,0) DEFAULT NULL,
 `object_posright` decimal(10,0) DEFAULT NULL,
 `object_color` varchar(11) DEFAULT NULL,
 `tool_tip` varchar(4000) DEFAULT NULL,
 `line_type` decimal(10,0) DEFAULT NULL,
 `label_size` decimal(10,0) DEFAULT NULL
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




