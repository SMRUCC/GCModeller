---
title: species_2_figure
---

# species_2_figure
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `species_2_figure`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `species_2_figure` (
 `DB_ID` int(10) unsigned DEFAULT NULL,
 `figure_rank` int(10) unsigned DEFAULT NULL,
 `figure` int(10) unsigned DEFAULT NULL,
 `figure_class` varchar(64) DEFAULT NULL,
 KEY `DB_ID` (`DB_ID`),
 KEY `figure` (`figure`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




