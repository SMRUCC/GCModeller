---
title: varsplic_master
---

# varsplic_master
_namespace: [SMRUCC.genomics.Analysis.Annotations.Interpro.Tables](N-SMRUCC.genomics.Analysis.Annotations.Interpro.Tables.html)_

--
 
 DROP TABLE IF EXISTS `varsplic_master`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `varsplic_master` (
 `protein_ac` varchar(6) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
 `variant` int(3) DEFAULT NULL,
 `crc64` varchar(16) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
 `length` int(5) DEFAULT NULL
 ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




