---
title: supermatch
---

# supermatch
_namespace: [SMRUCC.genomics.Analysis.Annotations.Interpro.Tables](N-SMRUCC.genomics.Analysis.Annotations.Interpro.Tables.html)_

--
 
 DROP TABLE IF EXISTS `supermatch`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `supermatch` (
 `protein_ac` varchar(6) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `entry_ac` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `pos_from` int(5) NOT NULL,
 `pos_to` int(5) NOT NULL,
 PRIMARY KEY (`protein_ac`,`entry_ac`,`pos_to`,`pos_from`),
 KEY `fkv_supermatch$entry_ac` (`entry_ac`),
 CONSTRAINT `fkv_supermatch$entry_ac` FOREIGN KEY (`entry_ac`) REFERENCES `entry` (`entry_ac`) ON DELETE CASCADE ON UPDATE NO ACTION,
 CONSTRAINT `fkv_supermatch$protein_ac` FOREIGN KEY (`protein_ac`) REFERENCES `protein` (`protein_ac`) ON DELETE CASCADE ON UPDATE NO ACTION
 ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




