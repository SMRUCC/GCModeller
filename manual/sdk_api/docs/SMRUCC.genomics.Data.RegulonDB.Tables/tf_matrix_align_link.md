﻿# tf_matrix_align_link
_namespace: [SMRUCC.genomics.Data.RegulonDB.Tables](./index.md)_

--
 
 DROP TABLE IF EXISTS `tf_matrix_align_link`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `tf_matrix_align_link` (
 `tf_matrix_id` char(12) NOT NULL,
 `tf_alignment_id` char(12) DEFAULT NULL,
 `transcription_factor_id` char(12) NOT NULL,
 `tf_matrix_name` varchar(255) DEFAULT NULL,
 `media` decimal(5,2) NOT NULL,
 `standar_desv` decimal(8,3) NOT NULL,
 `score_low` decimal(5,2) NOT NULL,
 `score_high` decimal(5,2) DEFAULT NULL,
 `tf_matrix_note` varchar(2000) DEFAULT NULL,
 `tf_matrix_internal_comment` longtext,
 `key_id_org` varchar(5) NOT NULL
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




