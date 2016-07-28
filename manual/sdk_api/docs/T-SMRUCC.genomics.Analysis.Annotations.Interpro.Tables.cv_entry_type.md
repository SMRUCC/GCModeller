---
title: cv_entry_type
---

# cv_entry_type
_namespace: [SMRUCC.genomics.Analysis.Annotations.Interpro.Tables](N-SMRUCC.genomics.Analysis.Annotations.Interpro.Tables.html)_

--
 
 DROP TABLE IF EXISTS `cv_entry_type`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `cv_entry_type` (
 `code` char(1) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `abbrev` varchar(20) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `description` mediumtext CHARACTER SET latin1 COLLATE latin1_bin,
 PRIMARY KEY (`code`),
 UNIQUE KEY `uq_entry_type$abbrev` (`abbrev`)
 ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




