﻿# go_molecularfunction
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](./index.md)_

--
 
 DROP TABLE IF EXISTS `go_molecularfunction`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `go_molecularfunction` (
 `DB_ID` int(10) unsigned NOT NULL,
 `accession` text,
 `definition` text,
 `referenceDatabase` int(10) unsigned DEFAULT NULL,
 `referenceDatabase_class` varchar(64) DEFAULT NULL,
 PRIMARY KEY (`DB_ID`),
 KEY `referenceDatabase` (`referenceDatabase`),
 FULLTEXT KEY `accession` (`accession`),
 FULLTEXT KEY `definition` (`definition`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




