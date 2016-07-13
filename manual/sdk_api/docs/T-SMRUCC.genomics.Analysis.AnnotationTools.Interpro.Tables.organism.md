---
title: organism
---

# organism
_namespace: [SMRUCC.genomics.Analysis.AnnotationTools.Interpro.Tables](N-SMRUCC.genomics.Analysis.AnnotationTools.Interpro.Tables.html)_

--
 
 DROP TABLE IF EXISTS `organism`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `organism` (
 `oscode` varchar(5) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `name` varchar(100) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `italics_name` varchar(100) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `full_name` varchar(100) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
 `tax_code` decimal(38,0) DEFAULT NULL,
 PRIMARY KEY (`oscode`)
 ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




