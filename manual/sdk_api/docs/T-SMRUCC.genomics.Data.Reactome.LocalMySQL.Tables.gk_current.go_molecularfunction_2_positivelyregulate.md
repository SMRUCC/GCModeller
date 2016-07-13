---
title: go_molecularfunction_2_positivelyregulate
---

# go_molecularfunction_2_positivelyregulate
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `go_molecularfunction_2_positivelyregulate`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `go_molecularfunction_2_positivelyregulate` (
 `DB_ID` int(10) unsigned DEFAULT NULL,
 `positivelyRegulate_rank` int(10) unsigned DEFAULT NULL,
 `positivelyRegulate` int(10) unsigned DEFAULT NULL,
 `positivelyRegulate_class` varchar(64) DEFAULT NULL,
 KEY `DB_ID` (`DB_ID`),
 KEY `positivelyRegulate` (`positivelyRegulate`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




