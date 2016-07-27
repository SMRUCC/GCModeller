---
title: entry2comp
---

# entry2comp
_namespace: [SMRUCC.genomics.Analysis.Annotations.Interpro.Tables](N-SMRUCC.genomics.Analysis.Annotations.Interpro.Tables.html)_

--
 
 DROP TABLE IF EXISTS `entry2comp`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `entry2comp` (
 `entry1_ac` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `entry2_ac` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `relation` char(2) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 PRIMARY KEY (`entry1_ac`,`entry2_ac`),
 KEY `fk_entry2comp$relation` (`relation`),
 KEY `fk_entry2comp$2` (`entry2_ac`),
 CONSTRAINT `fk_entry2comp$relation` FOREIGN KEY (`relation`) REFERENCES `cv_relation` (`code`) ON DELETE CASCADE ON UPDATE NO ACTION,
 CONSTRAINT `fk_entry2comp$1` FOREIGN KEY (`entry1_ac`) REFERENCES `entry` (`entry_ac`) ON DELETE CASCADE ON UPDATE NO ACTION,
 CONSTRAINT `fk_entry2comp$2` FOREIGN KEY (`entry2_ac`) REFERENCES `entry` (`entry_ac`) ON DELETE CASCADE ON UPDATE NO ACTION
 ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




