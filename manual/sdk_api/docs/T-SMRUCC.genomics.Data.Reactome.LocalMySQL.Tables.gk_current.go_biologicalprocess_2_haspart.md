---
title: go_biologicalprocess_2_haspart
---

# go_biologicalprocess_2_haspart
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `go_biologicalprocess_2_haspart`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `go_biologicalprocess_2_haspart` (
 `DB_ID` int(10) unsigned DEFAULT NULL,
 `hasPart_rank` int(10) unsigned DEFAULT NULL,
 `hasPart` int(10) unsigned DEFAULT NULL,
 `hasPart_class` varchar(64) DEFAULT NULL,
 KEY `DB_ID` (`DB_ID`),
 KEY `hasPart` (`hasPart`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




