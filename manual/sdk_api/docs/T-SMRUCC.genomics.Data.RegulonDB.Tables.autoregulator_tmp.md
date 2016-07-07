---
title: autoregulator_tmp
---

# autoregulator_tmp
_namespace: [SMRUCC.genomics.Data.RegulonDB.Tables](N-SMRUCC.genomics.Data.RegulonDB.Tables.html)_

--
 
 DROP TABLE IF EXISTS `autoregulator_tmp`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `autoregulator_tmp` (
 `gene_id` char(12) DEFAULT NULL,
 `transcription_factor_id` char(12) DEFAULT NULL,
 `transcription_factor_name` varchar(200) DEFAULT NULL,
 `autoregulator_function` varchar(9) DEFAULT NULL
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




