---
title: intact_data
---

# intact_data
_namespace: [SMRUCC.genomics.Analysis.AnnotationTools.Interpro.Tables](N-SMRUCC.genomics.Analysis.AnnotationTools.Interpro.Tables.html)_

--
 
 DROP TABLE IF EXISTS `intact_data`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `intact_data` (
 `uniprot_id` varchar(20) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `protein_ac` varchar(30) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `undetermined` char(1) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
 `intact_id` varchar(30) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `interacts_with` varchar(30) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `type` varchar(20) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
 `entry_ac` varchar(30) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `pubmed_id` varchar(30) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
 PRIMARY KEY (`entry_ac`,`intact_id`,`interacts_with`,`protein_ac`),
 CONSTRAINT `fk_intact_data$entry_ac` FOREIGN KEY (`entry_ac`) REFERENCES `entry` (`entry_ac`) ON DELETE CASCADE ON UPDATE NO ACTION
 ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




