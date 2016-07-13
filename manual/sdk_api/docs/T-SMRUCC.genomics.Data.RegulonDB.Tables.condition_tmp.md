---
title: condition_tmp
---

# condition_tmp
_namespace: [SMRUCC.genomics.Data.RegulonDB.Tables](N-SMRUCC.genomics.Data.RegulonDB.Tables.html)_

--
 
 DROP TABLE IF EXISTS `condition_tmp`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `condition_tmp` (
 `condition_id` char(12) DEFAULT NULL,
 `cond_effect_link_id` char(12) DEFAULT NULL,
 `condition_gene_name` varchar(200) DEFAULT NULL,
 `condition_gene_id` varchar(12) DEFAULT NULL,
 `condition_effect` varchar(10) DEFAULT NULL,
 `condition_promoter_name` varchar(200) DEFAULT NULL,
 `condition_promoter_id` varchar(12) DEFAULT NULL,
 `condition_final_state` varchar(200) DEFAULT NULL,
 `condition_conformation_id` varchar(12) DEFAULT NULL,
 `condition_site` varchar(200) DEFAULT NULL,
 `condition_evidence` varchar(200) DEFAULT NULL
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




