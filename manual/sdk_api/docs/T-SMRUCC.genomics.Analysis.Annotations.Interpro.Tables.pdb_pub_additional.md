---
title: pdb_pub_additional
---

# pdb_pub_additional
_namespace: [SMRUCC.genomics.Analysis.Annotations.Interpro.Tables](N-SMRUCC.genomics.Analysis.Annotations.Interpro.Tables.html)_

--
 
 DROP TABLE IF EXISTS `pdb_pub_additional`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `pdb_pub_additional` (
 `entry_ac` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `pub_id` varchar(11) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 KEY `fk_pdbpubadd$entry_ac` (`entry_ac`),
 KEY `fk_pdbpubadd$pub_id` (`pub_id`),
 CONSTRAINT `fk_pdbpubadd$entry_ac` FOREIGN KEY (`entry_ac`) REFERENCES `entry` (`entry_ac`) ON DELETE CASCADE ON UPDATE NO ACTION,
 CONSTRAINT `fk_pdbpubadd$pub_id` FOREIGN KEY (`pub_id`) REFERENCES `pub` (`pub_id`) ON DELETE CASCADE ON UPDATE NO ACTION
 ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




