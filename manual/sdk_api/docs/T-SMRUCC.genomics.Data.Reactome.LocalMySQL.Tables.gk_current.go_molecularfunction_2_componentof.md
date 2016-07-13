---
title: go_molecularfunction_2_componentof
---

# go_molecularfunction_2_componentof
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `go_molecularfunction_2_componentof`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `go_molecularfunction_2_componentof` (
 `DB_ID` int(10) unsigned DEFAULT NULL,
 `componentOf_rank` int(10) unsigned DEFAULT NULL,
 `componentOf` int(10) unsigned DEFAULT NULL,
 `componentOf_class` varchar(64) DEFAULT NULL,
 KEY `DB_ID` (`DB_ID`),
 KEY `componentOf` (`componentOf`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




