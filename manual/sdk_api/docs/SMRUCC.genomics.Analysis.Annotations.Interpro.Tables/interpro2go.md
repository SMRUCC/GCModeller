﻿# interpro2go
_namespace: [SMRUCC.genomics.Analysis.Annotations.Interpro.Tables](./index.md)_

--
 
 DROP TABLE IF EXISTS `interpro2go`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `interpro2go` (
 `entry_ac` varchar(9) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `go_id` varchar(10) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL
 ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




