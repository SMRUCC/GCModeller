---
title: cv_synonym
---

# cv_synonym
_namespace: [SMRUCC.genomics.Analysis.Annotations.Interpro.Tables](N-SMRUCC.genomics.Analysis.Annotations.Interpro.Tables.html)_

--
 
 DROP TABLE IF EXISTS `cv_synonym`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `cv_synonym` (
 `code` char(4) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `description` varchar(80) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 PRIMARY KEY (`code`)
 ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




