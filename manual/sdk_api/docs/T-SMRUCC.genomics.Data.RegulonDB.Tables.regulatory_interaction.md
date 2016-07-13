---
title: regulatory_interaction
---

# regulatory_interaction
_namespace: [SMRUCC.genomics.Data.RegulonDB.Tables](N-SMRUCC.genomics.Data.RegulonDB.Tables.html)_

--
 
 DROP TABLE IF EXISTS `regulatory_interaction`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `regulatory_interaction` (
 `regulatory_interaction_id` char(12) NOT NULL,
 `conformation_id` char(12) NOT NULL,
 `promoter_id` char(12) DEFAULT NULL,
 `site_id` char(12) NOT NULL,
 `ri_function` varchar(9) DEFAULT NULL,
 `center_position` decimal(20,2) DEFAULT NULL,
 `ri_dist_first_gene` decimal(20,2) DEFAULT NULL,
 `ri_first_gene_id` char(12) DEFAULT NULL,
 `affinity_exp` decimal(20,5) DEFAULT NULL,
 `regulatory_interaction_note` varchar(2000) DEFAULT NULL,
 `ri_internal_comment` longtext,
 `key_id_org` varchar(5) NOT NULL,
 `ri_sequence` varchar(100) DEFAULT NULL
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




