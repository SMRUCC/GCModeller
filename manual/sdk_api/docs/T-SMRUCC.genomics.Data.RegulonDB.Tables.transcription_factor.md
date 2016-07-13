---
title: transcription_factor
---

# transcription_factor
_namespace: [SMRUCC.genomics.Data.RegulonDB.Tables](N-SMRUCC.genomics.Data.RegulonDB.Tables.html)_

--
 
 DROP TABLE IF EXISTS `transcription_factor`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `transcription_factor` (
 `transcription_factor_id` char(12) NOT NULL,
 `transcription_factor_name` varchar(255) NOT NULL,
 `site_length` decimal(10,0) DEFAULT NULL,
 `symmetry` varchar(50) DEFAULT NULL,
 `transcription_factor_family` varchar(250) DEFAULT NULL,
 `tf_internal_comment` longtext,
 `key_id_org` varchar(5) NOT NULL,
 `transcription_factor_note` longtext,
 `connectivity_class` varchar(100) DEFAULT NULL,
 `sensing_class` varchar(100) DEFAULT NULL
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




