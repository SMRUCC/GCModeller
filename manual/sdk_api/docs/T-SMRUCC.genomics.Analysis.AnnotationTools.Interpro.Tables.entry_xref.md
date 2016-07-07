---
title: entry_xref
---

# entry_xref
_namespace: [SMRUCC.genomics.Analysis.AnnotationTools.Interpro.Tables](N-SMRUCC.genomics.Analysis.AnnotationTools.Interpro.Tables.html)_

--
 
 DROP TABLE IF EXISTS `entry_xref`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `entry_xref` (
 `entry_ac` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `dbcode` char(1) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `ac` varchar(30) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `name` varchar(70) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
 PRIMARY KEY (`entry_ac`,`dbcode`,`ac`),
 KEY `fk_entry_xref$dbcode` (`dbcode`),
 CONSTRAINT `fk_entry_xref$dbcode` FOREIGN KEY (`dbcode`) REFERENCES `cv_database` (`dbcode`) ON DELETE NO ACTION ON UPDATE NO ACTION,
 CONSTRAINT `fk_entry_xref$entry_ac` FOREIGN KEY (`entry_ac`) REFERENCES `entry` (`entry_ac`) ON DELETE CASCADE ON UPDATE NO ACTION
 ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




