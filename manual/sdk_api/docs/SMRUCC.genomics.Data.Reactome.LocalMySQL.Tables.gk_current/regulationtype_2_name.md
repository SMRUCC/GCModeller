# regulationtype_2_name
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](./index.md)_

--
 
 DROP TABLE IF EXISTS `regulationtype_2_name`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `regulationtype_2_name` (
 `DB_ID` int(10) unsigned DEFAULT NULL,
 `name_rank` int(10) unsigned DEFAULT NULL,
 `name` varchar(255) DEFAULT NULL,
 KEY `DB_ID` (`DB_ID`),
 KEY `name` (`name`),
 FULLTEXT KEY `name_fulltext` (`name`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




