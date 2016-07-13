---
title: edge
---

# edge
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `edge`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `edge` (
 `DB_ID` int(10) unsigned NOT NULL,
 `edgeType` int(10) DEFAULT NULL,
 `pathwayDiagram` int(10) unsigned DEFAULT NULL,
 `pathwayDiagram_class` varchar(64) DEFAULT NULL,
 `pointCoordinates` text,
 `sourceVertex` int(10) unsigned DEFAULT NULL,
 `sourceVertex_class` varchar(64) DEFAULT NULL,
 `targetVertex` int(10) unsigned DEFAULT NULL,
 `targetVertex_class` varchar(64) DEFAULT NULL,
 PRIMARY KEY (`DB_ID`),
 KEY `edgeType` (`edgeType`),
 KEY `pathwayDiagram` (`pathwayDiagram`),
 KEY `sourceVertex` (`sourceVertex`),
 KEY `targetVertex` (`targetVertex`),
 FULLTEXT KEY `pointCoordinates` (`pointCoordinates`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




