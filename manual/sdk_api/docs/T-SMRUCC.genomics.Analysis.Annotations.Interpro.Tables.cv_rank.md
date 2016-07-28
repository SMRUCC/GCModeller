---
title: cv_rank
---

# cv_rank
_namespace: [SMRUCC.genomics.Analysis.Annotations.Interpro.Tables](N-SMRUCC.genomics.Analysis.Annotations.Interpro.Tables.html)_

--
 
 DROP TABLE IF EXISTS `cv_rank`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `cv_rank` (
 `rank` varchar(20) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
 `seq` bigint(15) NOT NULL
 ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




