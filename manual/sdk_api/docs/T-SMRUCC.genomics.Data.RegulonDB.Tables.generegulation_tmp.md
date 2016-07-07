---
title: generegulation_tmp
---

# generegulation_tmp
_namespace: [SMRUCC.genomics.Data.RegulonDB.Tables](N-SMRUCC.genomics.Data.RegulonDB.Tables.html)_

--
 
 DROP TABLE IF EXISTS `generegulation_tmp`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `generegulation_tmp` (
 `gene_id_regulator` char(12) DEFAULT NULL,
 `gene_name_regulator` varchar(255) DEFAULT NULL,
 `tf_id_regulator` char(12) DEFAULT NULL,
 `transcription_factor_name` varchar(255) DEFAULT NULL,
 `tf_conformation` varchar(2000) DEFAULT NULL,
 `conformation_status` varchar(5) DEFAULT NULL,
 `gene_id_regulated` char(12) DEFAULT NULL,
 `gene_name_regulated` varchar(255) DEFAULT NULL,
 `generegulation_function` varchar(9) DEFAULT NULL
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




