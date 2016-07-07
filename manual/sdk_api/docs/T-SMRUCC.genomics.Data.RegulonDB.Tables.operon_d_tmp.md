---
title: operon_d_tmp
---

# operon_d_tmp
_namespace: [SMRUCC.genomics.Data.RegulonDB.Tables](N-SMRUCC.genomics.Data.RegulonDB.Tables.html)_

--
 
 DROP TABLE IF EXISTS `operon_d_tmp`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `operon_d_tmp` (
 `op_id` decimal(10,0) NOT NULL,
 `operon_id` char(12) DEFAULT NULL,
 `operon_name` varchar(255) DEFAULT NULL,
 `operon_tu_group` decimal(10,0) DEFAULT NULL,
 `operon_gene_group` decimal(10,0) DEFAULT NULL,
 `operon_sf_group` decimal(10,0) DEFAULT NULL,
 `operon_site_group` decimal(10,0) DEFAULT NULL,
 `operon_promoter_group` decimal(10,0) DEFAULT NULL,
 `operon_tf_group` decimal(10,0) DEFAULT NULL,
 `operon_terminator_group` decimal(10,0) DEFAULT NULL
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




