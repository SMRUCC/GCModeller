# complex_2_species
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](./index.md)_

--
 
 DROP TABLE IF EXISTS `complex_2_species`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `complex_2_species` (
 `DB_ID` int(10) unsigned DEFAULT NULL,
 `species_rank` int(10) unsigned DEFAULT NULL,
 `species` int(10) unsigned DEFAULT NULL,
 `species_class` varchar(64) DEFAULT NULL,
 KEY `DB_ID` (`DB_ID`),
 KEY `species` (`species`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




