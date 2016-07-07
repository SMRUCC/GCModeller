---
title: datamodel
---

# datamodel
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `datamodel`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `datamodel` (
 `thing` varchar(255) NOT NULL,
 `thing_class` enum('SchemaClass','SchemaClassAttribute','Schema') DEFAULT NULL,
 `property_name` varchar(255) NOT NULL,
 `property_value` text,
 `property_value_type` enum('INTEGER','SYMBOL','STRING','INSTANCE','SchemaClass','SchemaClassAttribute') DEFAULT NULL,
 `property_value_rank` int(10) unsigned DEFAULT '0'
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




