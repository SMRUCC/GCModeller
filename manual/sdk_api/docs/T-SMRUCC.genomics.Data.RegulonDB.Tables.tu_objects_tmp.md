---
title: tu_objects_tmp
---

# tu_objects_tmp
_namespace: [SMRUCC.genomics.Data.RegulonDB.Tables](N-SMRUCC.genomics.Data.RegulonDB.Tables.html)_

--
 
 DROP TABLE IF EXISTS `tu_objects_tmp`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `tu_objects_tmp` (
 `transcription_unit_id` char(12) DEFAULT NULL,
 `numtu` decimal(10,0) DEFAULT NULL,
 `tu_posleft` decimal(10,0) DEFAULT NULL,
 `tu_posright` decimal(10,0) DEFAULT NULL,
 `tu_type` varchar(10) DEFAULT NULL,
 `tu_object_class` char(2) DEFAULT NULL,
 `tu_object_id` char(12) DEFAULT NULL,
 `tu_object_name` varchar(200) DEFAULT NULL,
 `tu_object_posleft` decimal(10,0) DEFAULT NULL,
 `tu_object_posright` decimal(10,0) DEFAULT NULL,
 `tu_object_strand` char(1) DEFAULT NULL,
 `tu_object_colorclass` varchar(12) DEFAULT NULL,
 `tu_object_description` varchar(2000) DEFAULT NULL,
 `tu_object_sigma` varchar(100) DEFAULT NULL,
 `tu_object_evidence` varchar(2000) DEFAULT NULL,
 `tu_object_ri_type` varchar(100) DEFAULT NULL,
 `tu_object_type` varchar(10) DEFAULT NULL,
 `evidence` char(1) DEFAULT NULL
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




