---
title: matrix
---

# matrix
_namespace: [SMRUCC.genomics.Data.RegulonDB.Tables](N-SMRUCC.genomics.Data.RegulonDB.Tables.html)_

--
 
 DROP TABLE IF EXISTS `matrix`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `matrix` (
 `tf_matrix_id` char(12) NOT NULL,
 `num_col` decimal(10,0) NOT NULL,
 `freq_a` decimal(10,0) NOT NULL,
 `freq_c` decimal(10,0) NOT NULL,
 `freq_g` decimal(10,0) NOT NULL,
 `freq_t` decimal(10,0) NOT NULL
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




