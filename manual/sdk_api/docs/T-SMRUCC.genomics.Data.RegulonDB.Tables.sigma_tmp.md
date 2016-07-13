---
title: sigma_tmp
---

# sigma_tmp
_namespace: [SMRUCC.genomics.Data.RegulonDB.Tables](N-SMRUCC.genomics.Data.RegulonDB.Tables.html)_

--
 
 DROP TABLE IF EXISTS `sigma_tmp`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `sigma_tmp` (
 `sigma_id` varchar(12) NOT NULL,
 `sigma_name` varchar(50) NOT NULL,
 `sigma_synonyms` varchar(50) DEFAULT NULL,
 `sigma_gene_id` varchar(12) DEFAULT NULL,
 `sigma_gene_name` varchar(250) DEFAULT NULL,
 `sigma_coregulators` varchar(2000) DEFAULT NULL,
 `sigma_notes` varchar(4000) DEFAULT NULL,
 `sigma_sigmulon_genes` varchar(4000) DEFAULT NULL,
 `key_id_org` varchar(5) DEFAULT NULL
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




