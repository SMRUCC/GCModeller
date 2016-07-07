---
title: entry
---

# entry
_namespace: [SMRUCC.genomics.Analysis.AnnotationTools.Interpro.Tables](N-SMRUCC.genomics.Analysis.AnnotationTools.Interpro.Tables.html)_

--
 
 DROP TABLE IF EXISTS `entry`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `entry` (
 `entry_ac` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `entry_type` char(1) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `name` varchar(100) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
 `created` datetime NOT NULL,
 `short_name` varchar(30) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
 PRIMARY KEY (`entry_ac`),
 KEY `i_fk_entry$entry_type` (`entry_type`),
 KEY `fk_entry$entry_type` (`entry_type`),
 CONSTRAINT `fk_entry$entry_type` FOREIGN KEY (`entry_type`) REFERENCES `cv_entry_type` (`code`) ON DELETE NO ACTION ON UPDATE NO ACTION
 ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




