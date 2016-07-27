---
title: uniprot_taxonomy
---

# uniprot_taxonomy
_namespace: [SMRUCC.genomics.Analysis.Annotations.Interpro.Tables](N-SMRUCC.genomics.Analysis.Annotations.Interpro.Tables.html)_

--
 
 DROP TABLE IF EXISTS `uniprot_taxonomy`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `uniprot_taxonomy` (
 `protein_ac` varchar(10) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `tax_id` bigint(15) NOT NULL,
 `left_number` bigint(15) NOT NULL,
 `right_number` bigint(15) NOT NULL
 ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




