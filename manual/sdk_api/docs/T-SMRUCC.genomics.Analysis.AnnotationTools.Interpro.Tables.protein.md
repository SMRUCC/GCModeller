---
title: protein
---

# protein
_namespace: [SMRUCC.genomics.Analysis.AnnotationTools.Interpro.Tables](N-SMRUCC.genomics.Analysis.AnnotationTools.Interpro.Tables.html)_

--
 
 DROP TABLE IF EXISTS `protein`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `protein` (
 `protein_ac` varchar(6) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `name` varchar(12) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `dbcode` char(1) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `crc64` char(16) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `len` int(5) NOT NULL,
 `fragment` char(1) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `struct_flag` char(1) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `tax_id` bigint(15) DEFAULT NULL,
 PRIMARY KEY (`protein_ac`),
 KEY `fk_protein$dbcode` (`dbcode`),
 CONSTRAINT `fk_protein$dbcode` FOREIGN KEY (`dbcode`) REFERENCES `cv_database` (`dbcode`) ON DELETE NO ACTION ON UPDATE NO ACTION
 ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




