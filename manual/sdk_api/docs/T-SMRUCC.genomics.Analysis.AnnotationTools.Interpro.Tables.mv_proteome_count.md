---
title: mv_proteome_count
---

# mv_proteome_count
_namespace: [SMRUCC.genomics.Analysis.AnnotationTools.Interpro.Tables](N-SMRUCC.genomics.Analysis.AnnotationTools.Interpro.Tables.html)_

--
 
 DROP TABLE IF EXISTS `mv_proteome_count`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `mv_proteome_count` (
 `entry_ac` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `name` varchar(100) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `oscode` varchar(5) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `protein_count` int(7) NOT NULL,
 `method_count` int(7) NOT NULL,
 PRIMARY KEY (`entry_ac`,`oscode`),
 KEY `fk_mv_proteome_count$oscode` (`oscode`),
 CONSTRAINT `fk_mv_proteome_count$entry` FOREIGN KEY (`entry_ac`) REFERENCES `entry` (`entry_ac`) ON DELETE CASCADE ON UPDATE NO ACTION,
 CONSTRAINT `fk_mv_proteome_count$oscode` FOREIGN KEY (`oscode`) REFERENCES `organism` (`oscode`) ON DELETE CASCADE ON UPDATE NO ACTION
 ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




