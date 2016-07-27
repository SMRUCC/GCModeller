---
title: mv_entry2protein_true
---

# mv_entry2protein_true
_namespace: [SMRUCC.genomics.Analysis.Annotations.Interpro.Tables](N-SMRUCC.genomics.Analysis.Annotations.Interpro.Tables.html)_

--
 
 DROP TABLE IF EXISTS `mv_entry2protein_true`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `mv_entry2protein_true` (
 `entry_ac` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `protein_ac` varchar(6) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `match_count` int(7) NOT NULL,
 PRIMARY KEY (`entry_ac`,`protein_ac`),
 KEY `fk_mv_entry2protein_true$p` (`protein_ac`),
 CONSTRAINT `fk_mv_entry2protein_true$e` FOREIGN KEY (`entry_ac`) REFERENCES `entry` (`entry_ac`) ON DELETE CASCADE ON UPDATE NO ACTION,
 CONSTRAINT `fk_mv_entry2protein_true$p` FOREIGN KEY (`protein_ac`) REFERENCES `protein` (`protein_ac`) ON DELETE CASCADE ON UPDATE NO ACTION
 ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




