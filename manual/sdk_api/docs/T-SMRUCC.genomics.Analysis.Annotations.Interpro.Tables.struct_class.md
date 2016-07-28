---
title: struct_class
---

# struct_class
_namespace: [SMRUCC.genomics.Analysis.Annotations.Interpro.Tables](N-SMRUCC.genomics.Analysis.Annotations.Interpro.Tables.html)_

--
 
 DROP TABLE IF EXISTS `struct_class`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `struct_class` (
 `domain_id` varchar(14) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `fam_id` varchar(20) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
 `dbcode` varchar(1) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
 PRIMARY KEY (`domain_id`)
 ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




