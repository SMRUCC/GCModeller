---
title: varsplic_match
---

# varsplic_match
_namespace: [SMRUCC.genomics.Analysis.AnnotationTools.Interpro.Tables](N-SMRUCC.genomics.Analysis.AnnotationTools.Interpro.Tables.html)_

--
 
 DROP TABLE IF EXISTS `varsplic_match`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `varsplic_match` (
 `protein_ac` varchar(12) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `method_ac` varchar(25) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `pos_from` int(5) DEFAULT NULL,
 `pos_to` int(5) DEFAULT NULL,
 `status` char(1) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `dbcode` char(1) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `evidence` char(3) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
 `seq_date` datetime NOT NULL,
 `match_date` datetime NOT NULL,
 `score` double DEFAULT NULL,
 KEY `fk_varsplic_match$dbcode` (`dbcode`),
 KEY `fk_varsplic_match$evidence` (`evidence`),
 KEY `fk_varsplic_match$method` (`method_ac`),
 CONSTRAINT `fk_varsplic_match$dbcode` FOREIGN KEY (`dbcode`) REFERENCES `cv_database` (`dbcode`) ON DELETE NO ACTION ON UPDATE NO ACTION,
 CONSTRAINT `fk_varsplic_match$evidence` FOREIGN KEY (`evidence`) REFERENCES `cv_evidence` (`code`) ON DELETE NO ACTION ON UPDATE NO ACTION,
 CONSTRAINT `fk_varsplic_match$method` FOREIGN KEY (`method_ac`) REFERENCES `method` (`method_ac`) ON DELETE CASCADE ON UPDATE NO ACTION
 ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




