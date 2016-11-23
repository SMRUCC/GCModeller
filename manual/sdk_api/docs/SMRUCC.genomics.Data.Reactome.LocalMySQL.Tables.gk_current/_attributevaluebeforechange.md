# _attributevaluebeforechange
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](./index.md)_

--
 
 DROP TABLE IF EXISTS `_attributevaluebeforechange`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `_attributevaluebeforechange` (
 `DB_ID` int(10) unsigned NOT NULL,
 `changedAttribute` text,
 PRIMARY KEY (`DB_ID`),
 FULLTEXT KEY `changedAttribute` (`changedAttribute`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




