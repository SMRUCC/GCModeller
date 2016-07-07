---
title: alignment
---

# alignment
_namespace: [SMRUCC.genomics.Data.RegulonDB.Tables](N-SMRUCC.genomics.Data.RegulonDB.Tables.html)_

--
 
 DROP TABLE IF EXISTS `alignment`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `alignment` (
 `tf_alignment_id` char(12) NOT NULL,
 `site_id` char(12) NOT NULL,
 `alignment_sequence` varchar(255) NOT NULL,
 `alignment_score_sequence` decimal(10,0) NOT NULL
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




