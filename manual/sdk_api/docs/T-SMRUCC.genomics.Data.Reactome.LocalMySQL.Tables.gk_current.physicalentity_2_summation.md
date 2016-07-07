---
title: physicalentity_2_summation
---

# physicalentity_2_summation
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `physicalentity_2_summation`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `physicalentity_2_summation` (
 `DB_ID` int(10) unsigned DEFAULT NULL,
 `summation_rank` int(10) unsigned DEFAULT NULL,
 `summation` int(10) unsigned DEFAULT NULL,
 `summation_class` varchar(64) DEFAULT NULL,
 KEY `DB_ID` (`DB_ID`),
 KEY `summation` (`summation`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




