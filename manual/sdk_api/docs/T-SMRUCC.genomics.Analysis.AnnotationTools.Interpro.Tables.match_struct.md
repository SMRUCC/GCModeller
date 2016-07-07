---
title: match_struct
---

# match_struct
_namespace: [SMRUCC.genomics.Analysis.AnnotationTools.Interpro.Tables](N-SMRUCC.genomics.Analysis.AnnotationTools.Interpro.Tables.html)_

--
 
 DROP TABLE IF EXISTS `match_struct`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `match_struct` (
 `protein_ac` varchar(10) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `domain_id` varchar(14) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `pos_from` int(5) NOT NULL,
 `pos_to` int(5) DEFAULT NULL,
 `dbcode` varchar(1) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
 PRIMARY KEY (`protein_ac`,`domain_id`,`pos_from`),
 KEY `fk_match_struct` (`domain_id`),
 CONSTRAINT `fk_match_struct` FOREIGN KEY (`domain_id`) REFERENCES `struct_class` (`domain_id`) ON DELETE NO ACTION ON UPDATE NO ACTION
 ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




