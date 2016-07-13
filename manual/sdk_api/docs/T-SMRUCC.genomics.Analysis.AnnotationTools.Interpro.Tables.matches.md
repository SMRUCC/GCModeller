---
title: matches
---

# matches
_namespace: [SMRUCC.genomics.Analysis.AnnotationTools.Interpro.Tables](N-SMRUCC.genomics.Analysis.AnnotationTools.Interpro.Tables.html)_

--
 
 DROP TABLE IF EXISTS `matches`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `matches` (
 `protein_ac` varchar(12) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `method_ac` varchar(25) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `pos_from` int(5) DEFAULT NULL,
 `pos_to` int(5) DEFAULT NULL,
 `status` char(1) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `evidence` char(3) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
 `match_date` datetime NOT NULL,
 `seq_date` datetime NOT NULL,
 `score` double DEFAULT NULL,
 KEY `fk_matches$evidence` (`evidence`),
 KEY `fk_matches$method` (`method_ac`),
 CONSTRAINT `fk_matches$evidence` FOREIGN KEY (`evidence`) REFERENCES `cv_evidence` (`code`) ON DELETE NO ACTION ON UPDATE NO ACTION,
 CONSTRAINT `fk_matches$method` FOREIGN KEY (`method_ac`) REFERENCES `method` (`method_ac`) ON DELETE CASCADE ON UPDATE NO ACTION
 ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




