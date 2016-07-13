---
title: go_molecularfunction_2_regulate
---

# go_molecularfunction_2_regulate
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `go_molecularfunction_2_regulate`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `go_molecularfunction_2_regulate` (
 `DB_ID` int(10) unsigned DEFAULT NULL,
 `regulate_rank` int(10) unsigned DEFAULT NULL,
 `regulate` int(10) unsigned DEFAULT NULL,
 `regulate_class` varchar(64) DEFAULT NULL,
 KEY `DB_ID` (`DB_ID`),
 KEY `regulate` (`regulate`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




