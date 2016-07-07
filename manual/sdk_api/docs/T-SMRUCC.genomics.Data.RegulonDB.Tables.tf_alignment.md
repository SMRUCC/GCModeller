---
title: tf_alignment
---

# tf_alignment
_namespace: [SMRUCC.genomics.Data.RegulonDB.Tables](N-SMRUCC.genomics.Data.RegulonDB.Tables.html)_

--
 
 DROP TABLE IF EXISTS `tf_alignment`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `tf_alignment` (
 `tf_alignment_id` char(12) NOT NULL,
 `transcription_factor_id` char(12) NOT NULL,
 `tf_matrix_id` char(12) DEFAULT NULL,
 `tf_alignment_name` varchar(255) NOT NULL,
 `tf_alignment_note` varchar(2000) DEFAULT NULL,
 `tf_alignment_internal_comment` longtext,
 `key_id_org` varchar(5) NOT NULL
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




