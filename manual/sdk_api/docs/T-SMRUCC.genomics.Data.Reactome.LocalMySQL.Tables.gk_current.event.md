---
title: event
---

# event
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `event`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `event` (
 `DB_ID` int(10) unsigned NOT NULL,
 `_doRelease` enum('TRUE','FALSE') DEFAULT NULL,
 `definition` text,
 `evidenceType` int(10) unsigned DEFAULT NULL,
 `evidenceType_class` varchar(64) DEFAULT NULL,
 `goBiologicalProcess` int(10) unsigned DEFAULT NULL,
 `goBiologicalProcess_class` varchar(64) DEFAULT NULL,
 `releaseDate` date DEFAULT NULL,
 `releaseStatus` text,
 PRIMARY KEY (`DB_ID`),
 KEY `_doRelease` (`_doRelease`),
 KEY `evidenceType` (`evidenceType`),
 KEY `goBiologicalProcess` (`goBiologicalProcess`),
 KEY `releaseDate` (`releaseDate`),
 FULLTEXT KEY `definition` (`definition`),
 FULLTEXT KEY `releaseStatus` (`releaseStatus`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




