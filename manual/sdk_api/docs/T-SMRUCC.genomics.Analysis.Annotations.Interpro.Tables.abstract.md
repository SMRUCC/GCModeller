---
title: abstract
---

# abstract
_namespace: [SMRUCC.genomics.Analysis.Annotations.Interpro.Tables](N-SMRUCC.genomics.Analysis.Annotations.Interpro.Tables.html)_

--
 
 DROP TABLE IF EXISTS `abstract`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `abstract` (
 `entry_ac` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `abstract` mediumtext CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `comments` varchar(100) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
 PRIMARY KEY (`entry_ac`),
 CONSTRAINT `fk_abstract$entry_ac` FOREIGN KEY (`entry_ac`) REFERENCES `entry` (`entry_ac`) ON DELETE CASCADE ON UPDATE NO ACTION
 ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




