---
title: interaction
---

# interaction
_namespace: [SMRUCC.genomics.Data.RegulonDB.Tables](N-SMRUCC.genomics.Data.RegulonDB.Tables.html)_

--
 
 DROP TABLE IF EXISTS `interaction`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `interaction` (
 `interaction_id` varchar(12) NOT NULL,
 `regulator_id` varchar(12) DEFAULT NULL,
 `promoter_id` char(12) DEFAULT NULL,
 `site_id` char(12) DEFAULT NULL,
 `interaction_function` varchar(12) DEFAULT NULL,
 `center_position` decimal(20,2) DEFAULT NULL,
 `interaction_first_gene_id` varchar(12) DEFAULT NULL,
 `affinity_exp` decimal(20,5) DEFAULT NULL,
 `interaction_note` varchar(2000) DEFAULT NULL,
 `interaction_internal_comment` longtext,
 `interaction_sequence` varchar(100) DEFAULT NULL,
 `key_id_org` varchar(5) NOT NULL
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




