---
title: transcription_unit
---

# transcription_unit
_namespace: [SMRUCC.genomics.Data.RegulonDB.Tables](N-SMRUCC.genomics.Data.RegulonDB.Tables.html)_

--
 
 DROP TABLE IF EXISTS `transcription_unit`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `transcription_unit` (
 `transcription_unit_id` char(12) NOT NULL,
 `promoter_id` char(12) DEFAULT NULL,
 `transcription_unit_name` varchar(255) DEFAULT NULL,
 `operon_id` char(12) DEFAULT NULL,
 `transcription_unit_note` varchar(4000) DEFAULT NULL,
 `tu_internal_comment` longtext,
 `key_id_org` varchar(5) NOT NULL
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




