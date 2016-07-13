---
title: srna_interaction
---

# srna_interaction
_namespace: [SMRUCC.genomics.Data.RegulonDB.Tables](N-SMRUCC.genomics.Data.RegulonDB.Tables.html)_

--
 
 DROP TABLE IF EXISTS `srna_interaction`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `srna_interaction` (
 `srna_id` char(12) NOT NULL,
 `srna_gene_id` char(12) DEFAULT NULL,
 `srna_gene_regulated_id` char(12) DEFAULT NULL,
 `srna_tu_regulated_id` char(12) DEFAULT NULL,
 `srna_function` varchar(2000) DEFAULT NULL,
 `srna_posleft` decimal(10,0) DEFAULT NULL,
 `srna_posright` decimal(10,0) DEFAULT NULL,
 `srna_sequence` varchar(2000) DEFAULT NULL,
 `srna_regulation_type` varchar(2000) DEFAULT NULL,
 `srna_mechanis` varchar(1000) DEFAULT NULL,
 `srna_note` varchar(1000) DEFAULT NULL
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




