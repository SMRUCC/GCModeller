---
title: promoter
---

# promoter
_namespace: [SMRUCC.genomics.Data.RegulonDB.Tables](N-SMRUCC.genomics.Data.RegulonDB.Tables.html)_

--
 
 DROP TABLE IF EXISTS `promoter`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `promoter` (
 `promoter_id` char(12) NOT NULL,
 `promoter_name` varchar(255) DEFAULT NULL,
 `promoter_strand` varchar(10) DEFAULT NULL,
 `pos_1` decimal(10,0) DEFAULT NULL,
 `sigma_factor` varchar(80) DEFAULT NULL,
 `basal_trans_val` decimal(20,5) DEFAULT NULL,
 `equilibrium_const` decimal(20,5) DEFAULT NULL,
 `kinetic_const` decimal(20,5) DEFAULT NULL,
 `strength_seq` decimal(20,5) DEFAULT NULL,
 `promoter_sequence` varchar(200) DEFAULT NULL,
 `promoter_note` varchar(4000) DEFAULT NULL,
 `promoter_internal_comment` longtext,
 `key_id_org` varchar(5) NOT NULL
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




