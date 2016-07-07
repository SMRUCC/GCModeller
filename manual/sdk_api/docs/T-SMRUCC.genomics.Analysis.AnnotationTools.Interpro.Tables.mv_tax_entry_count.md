---
title: mv_tax_entry_count
---

# mv_tax_entry_count
_namespace: [SMRUCC.genomics.Analysis.AnnotationTools.Interpro.Tables](N-SMRUCC.genomics.Analysis.AnnotationTools.Interpro.Tables.html)_

--
 
 DROP TABLE IF EXISTS `mv_tax_entry_count`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `mv_tax_entry_count` (
 `entry_ac` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `tax_id` decimal(22,0) NOT NULL,
 `count` decimal(22,0) NOT NULL,
 PRIMARY KEY (`entry_ac`,`tax_id`,`count`)
 ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




