---
title: rfam
---

# rfam
_namespace: [SMRUCC.genomics.Data.RegulonDB.Tables](N-SMRUCC.genomics.Data.RegulonDB.Tables.html)_

--
 
 DROP TABLE IF EXISTS `rfam`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `rfam` (
 `rfam_id` varchar(12) NOT NULL,
 `gene_id` char(12) DEFAULT NULL,
 `rfam_type` varchar(100) DEFAULT NULL,
 `rfam_description` varchar(2000) DEFAULT NULL,
 `rfam_score` decimal(10,5) DEFAULT NULL,
 `rfam_strand` varchar(12) DEFAULT NULL,
 `rfam_posleft` decimal(10,0) DEFAULT NULL,
 `rfam_posright` decimal(10,0) DEFAULT NULL,
 `rfam_sequence` varchar(1000) DEFAULT NULL
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




