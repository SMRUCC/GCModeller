---
title: gene
---

# gene
_namespace: [SMRUCC.genomics.Data.RegulonDB.Tables](N-SMRUCC.genomics.Data.RegulonDB.Tables.html)_

--
 
 DROP TABLE IF EXISTS `gene`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `gene` (
 `gene_id` char(12) NOT NULL,
 `gene_name` varchar(255) DEFAULT NULL,
 `gene_posleft` decimal(10,0) DEFAULT NULL,
 `gene_posright` decimal(10,0) DEFAULT NULL,
 `gene_strand` varchar(10) DEFAULT NULL,
 `gene_sequence` longtext,
 `gc_content` decimal(15,10) DEFAULT NULL,
 `cri_score` decimal(15,10) DEFAULT NULL,
 `gene_note` varchar(2000) DEFAULT NULL,
 `gene_internal_comment` longtext,
 `key_id_org` varchar(5) NOT NULL,
 `gene_type` varchar(100) DEFAULT NULL
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




