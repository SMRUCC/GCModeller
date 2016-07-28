---
title: mv_method2protein
---

# mv_method2protein
_namespace: [SMRUCC.genomics.Analysis.Annotations.Interpro.Tables](N-SMRUCC.genomics.Analysis.Annotations.Interpro.Tables.html)_

--
 
 DROP TABLE IF EXISTS `mv_method2protein`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `mv_method2protein` (
 `method_ac` varchar(25) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `protein_ac` varchar(6) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `match_count` int(7) NOT NULL,
 PRIMARY KEY (`method_ac`,`protein_ac`),
 KEY `fk_mv_method2protein$protein` (`protein_ac`),
 CONSTRAINT `fk_mv_method2protein$protein` FOREIGN KEY (`protein_ac`) REFERENCES `protein` (`protein_ac`) ON DELETE CASCADE ON UPDATE NO ACTION,
 CONSTRAINT `fk_mv_method2prot$method` FOREIGN KEY (`method_ac`) REFERENCES `method` (`method_ac`) ON DELETE CASCADE ON UPDATE NO ACTION
 ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




