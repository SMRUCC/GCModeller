---
title: event_2_relatedspecies
---

# event_2_relatedspecies
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `event_2_relatedspecies`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `event_2_relatedspecies` (
 `DB_ID` int(10) unsigned DEFAULT NULL,
 `relatedSpecies_rank` int(10) unsigned DEFAULT NULL,
 `relatedSpecies` int(10) unsigned DEFAULT NULL,
 `relatedSpecies_class` varchar(64) DEFAULT NULL,
 KEY `DB_ID` (`DB_ID`),
 KEY `relatedSpecies` (`relatedSpecies`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




