---
title: etaxi
---

# etaxi
_namespace: [SMRUCC.genomics.Analysis.Annotations.Interpro.Tables](N-SMRUCC.genomics.Analysis.Annotations.Interpro.Tables.html)_

--
 
 DROP TABLE IF EXISTS `etaxi`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `etaxi` (
 `tax_id` bigint(15) NOT NULL,
 `parent_id` bigint(15) DEFAULT NULL,
 `scientific_name` varchar(100) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `complete_genome_flag` char(1) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `rank` varchar(20) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
 `hidden` int(3) NOT NULL,
 `left_number` bigint(15) DEFAULT NULL,
 `right_number` bigint(15) DEFAULT NULL,
 `annotation_source` varchar(30) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `full_name` mediumtext CHARACTER SET latin1 COLLATE latin1_bin
 ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




