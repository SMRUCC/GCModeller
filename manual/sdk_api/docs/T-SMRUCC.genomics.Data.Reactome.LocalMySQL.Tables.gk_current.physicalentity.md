---
title: physicalentity
---

# physicalentity
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `physicalentity`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `physicalentity` (
 `DB_ID` int(10) unsigned NOT NULL,
 `authored` int(10) unsigned DEFAULT NULL,
 `authored_class` varchar(64) DEFAULT NULL,
 `cellType` int(10) unsigned DEFAULT NULL,
 `cellType_class` varchar(64) DEFAULT NULL,
 `definition` text,
 `goCellularComponent` int(10) unsigned DEFAULT NULL,
 `goCellularComponent_class` varchar(64) DEFAULT NULL,
 `systematicName` text,
 PRIMARY KEY (`DB_ID`),
 KEY `authored` (`authored`),
 KEY `cellType` (`cellType`),
 KEY `goCellularComponent` (`goCellularComponent`),
 FULLTEXT KEY `definition` (`definition`),
 FULLTEXT KEY `systematicName` (`systematicName`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




