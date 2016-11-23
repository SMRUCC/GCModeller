# catalystactivity_2_activeunit
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](./index.md)_

--
 
 DROP TABLE IF EXISTS `catalystactivity_2_activeunit`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `catalystactivity_2_activeunit` (
 `DB_ID` int(10) unsigned DEFAULT NULL,
 `activeUnit_rank` int(10) unsigned DEFAULT NULL,
 `activeUnit` int(10) unsigned DEFAULT NULL,
 `activeUnit_class` varchar(64) DEFAULT NULL,
 KEY `DB_ID` (`DB_ID`),
 KEY `activeUnit` (`activeUnit`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




