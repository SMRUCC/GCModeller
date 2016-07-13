---
title: go_molecularfunction_2_ecnumber
---

# go_molecularfunction_2_ecnumber
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `go_molecularfunction_2_ecnumber`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `go_molecularfunction_2_ecnumber` (
 `DB_ID` int(10) unsigned DEFAULT NULL,
 `ecNumber_rank` int(10) unsigned DEFAULT NULL,
 `ecNumber` text,
 KEY `DB_ID` (`DB_ID`),
 FULLTEXT KEY `ecNumber` (`ecNumber`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




