# referencesequence_2_description
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](./index.md)_

--
 
 DROP TABLE IF EXISTS `referencesequence_2_description`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `referencesequence_2_description` (
 `DB_ID` int(10) unsigned DEFAULT NULL,
 `description_rank` int(10) unsigned DEFAULT NULL,
 `description` text,
 KEY `DB_ID` (`DB_ID`),
 FULLTEXT KEY `description` (`description`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




