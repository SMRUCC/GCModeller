---
title: cv_relation
---

# cv_relation
_namespace: [SMRUCC.genomics.Analysis.Annotations.Interpro.Tables](N-SMRUCC.genomics.Analysis.Annotations.Interpro.Tables.html)_

--
 
 DROP TABLE IF EXISTS `cv_relation`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `cv_relation` (
 `code` char(2) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `abbrev` varchar(10) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `description` mediumtext CHARACTER SET latin1 COLLATE latin1_bin,
 `forward` varchar(30) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `reverse` varchar(30) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 PRIMARY KEY (`code`),
 UNIQUE KEY `uq_relation$abbrev` (`abbrev`)
 ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




