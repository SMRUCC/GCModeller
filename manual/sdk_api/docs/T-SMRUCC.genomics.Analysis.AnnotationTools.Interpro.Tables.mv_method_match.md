---
title: mv_method_match
---

# mv_method_match
_namespace: [SMRUCC.genomics.Analysis.AnnotationTools.Interpro.Tables](N-SMRUCC.genomics.Analysis.AnnotationTools.Interpro.Tables.html)_

--
 
 DROP TABLE IF EXISTS `mv_method_match`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `mv_method_match` (
 `method_ac` varchar(25) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `protein_count` int(7) NOT NULL,
 `match_count` int(7) NOT NULL,
 PRIMARY KEY (`method_ac`),
 CONSTRAINT `fk_mv_method_match$method` FOREIGN KEY (`method_ac`) REFERENCES `method` (`method_ac`) ON DELETE CASCADE ON UPDATE NO ACTION
 ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




