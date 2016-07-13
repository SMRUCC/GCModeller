---
title: complex_2_includedlocation
---

# complex_2_includedlocation
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `complex_2_includedlocation`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `complex_2_includedlocation` (
 `DB_ID` int(10) unsigned DEFAULT NULL,
 `includedLocation_rank` int(10) unsigned DEFAULT NULL,
 `includedLocation` int(10) unsigned DEFAULT NULL,
 `includedLocation_class` varchar(64) DEFAULT NULL,
 KEY `DB_ID` (`DB_ID`),
 KEY `includedLocation` (`includedLocation`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




