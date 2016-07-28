---
title: entry2entry
---

# entry2entry
_namespace: [SMRUCC.genomics.Analysis.Annotations.Interpro.Tables](N-SMRUCC.genomics.Analysis.Annotations.Interpro.Tables.html)_

--
 
 DROP TABLE IF EXISTS `entry2entry`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `entry2entry` (
 `entry_ac` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `parent_ac` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `relation` char(2) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 PRIMARY KEY (`entry_ac`,`parent_ac`),
 KEY `fk_entry2entry$parent` (`parent_ac`),
 KEY `fk_entry2entry$relation` (`relation`),
 CONSTRAINT `fk_entry2entry$child` FOREIGN KEY (`entry_ac`) REFERENCES `entry` (`entry_ac`) ON DELETE CASCADE ON UPDATE NO ACTION,
 CONSTRAINT `fk_entry2entry$parent` FOREIGN KEY (`parent_ac`) REFERENCES `entry` (`entry_ac`) ON DELETE CASCADE ON UPDATE NO ACTION,
 CONSTRAINT `fk_entry2entry$relation` FOREIGN KEY (`relation`) REFERENCES `cv_relation` (`code`) ON DELETE CASCADE ON UPDATE NO ACTION
 ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




