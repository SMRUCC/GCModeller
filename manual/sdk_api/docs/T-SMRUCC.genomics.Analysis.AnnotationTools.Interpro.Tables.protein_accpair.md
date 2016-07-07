---
title: protein_accpair
---

# protein_accpair
_namespace: [SMRUCC.genomics.Analysis.AnnotationTools.Interpro.Tables](N-SMRUCC.genomics.Analysis.AnnotationTools.Interpro.Tables.html)_

--
 
 DROP TABLE IF EXISTS `protein_accpair`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `protein_accpair` (
 `protein_ac` varchar(6) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `secondary_ac` varchar(6) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 PRIMARY KEY (`protein_ac`,`secondary_ac`),
 CONSTRAINT `fk_accpair$primary` FOREIGN KEY (`protein_ac`) REFERENCES `protein` (`protein_ac`) ON DELETE CASCADE ON UPDATE NO ACTION
 ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




