---
title: polymer_2_repeatedunit
---

# polymer_2_repeatedunit
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `polymer_2_repeatedunit`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `polymer_2_repeatedunit` (
 `DB_ID` int(10) unsigned DEFAULT NULL,
 `repeatedUnit_rank` int(10) unsigned DEFAULT NULL,
 `repeatedUnit` int(10) unsigned DEFAULT NULL,
 `repeatedUnit_class` varchar(64) DEFAULT NULL,
 KEY `DB_ID` (`DB_ID`),
 KEY `repeatedUnit` (`repeatedUnit`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




