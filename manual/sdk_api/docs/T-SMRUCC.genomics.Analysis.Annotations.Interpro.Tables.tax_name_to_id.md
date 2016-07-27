---
title: tax_name_to_id
---

# tax_name_to_id
_namespace: [SMRUCC.genomics.Analysis.Annotations.Interpro.Tables](N-SMRUCC.genomics.Analysis.Annotations.Interpro.Tables.html)_

--
 
 DROP TABLE IF EXISTS `tax_name_to_id`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `tax_name_to_id` (
 `tax_name` varchar(30) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
 `tax_id` bigint(15) DEFAULT NULL
 ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




