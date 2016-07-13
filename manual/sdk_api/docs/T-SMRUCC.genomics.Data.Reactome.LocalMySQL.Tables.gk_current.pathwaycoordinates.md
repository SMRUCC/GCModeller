---
title: pathwaycoordinates
---

# pathwaycoordinates
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `pathwaycoordinates`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `pathwaycoordinates` (
 `DB_ID` int(10) unsigned NOT NULL,
 `locatedEvent` int(10) unsigned DEFAULT NULL,
 `locatedEvent_class` varchar(64) DEFAULT NULL,
 `maxX` int(10) DEFAULT NULL,
 `maxY` int(10) DEFAULT NULL,
 `minX` int(10) DEFAULT NULL,
 `minY` int(10) DEFAULT NULL,
 PRIMARY KEY (`DB_ID`),
 KEY `locatedEvent` (`locatedEvent`),
 KEY `maxX` (`maxX`),
 KEY `maxY` (`maxY`),
 KEY `minX` (`minX`),
 KEY `minY` (`minY`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




