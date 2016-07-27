---
title: proteome_rank
---

# proteome_rank
_namespace: [SMRUCC.genomics.Analysis.Annotations.Interpro.Tables](N-SMRUCC.genomics.Analysis.Annotations.Interpro.Tables.html)_

--
 
 DROP TABLE IF EXISTS `proteome_rank`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `proteome_rank` (
 `entry_ac` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `oscode` varchar(5) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `rank` int(7) NOT NULL,
 PRIMARY KEY (`entry_ac`,`oscode`),
 KEY `fk_proteome_rank$oscode` (`oscode`),
 CONSTRAINT `fk_proteome_rank$entry` FOREIGN KEY (`entry_ac`) REFERENCES `entry` (`entry_ac`) ON DELETE CASCADE ON UPDATE NO ACTION,
 CONSTRAINT `fk_proteome_rank$oscode` FOREIGN KEY (`oscode`) REFERENCES `organism` (`oscode`) ON DELETE CASCADE ON UPDATE NO ACTION
 ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




