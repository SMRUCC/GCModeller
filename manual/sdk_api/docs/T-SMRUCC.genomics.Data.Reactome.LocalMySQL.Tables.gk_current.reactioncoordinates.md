---
title: reactioncoordinates
---

# reactioncoordinates
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `reactioncoordinates`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `reactioncoordinates` (
 `DB_ID` int(10) unsigned NOT NULL,
 `locatedEvent` int(10) unsigned DEFAULT NULL,
 `locatedEvent_class` varchar(64) DEFAULT NULL,
 `locationContext` int(10) unsigned DEFAULT NULL,
 `locationContext_class` varchar(64) DEFAULT NULL,
 `sourceX` int(10) DEFAULT NULL,
 `sourceY` int(10) DEFAULT NULL,
 `targetX` int(10) DEFAULT NULL,
 `targetY` int(10) DEFAULT NULL,
 PRIMARY KEY (`DB_ID`),
 KEY `locatedEvent` (`locatedEvent`),
 KEY `locationContext` (`locationContext`),
 KEY `sourceX` (`sourceX`),
 KEY `sourceY` (`sourceY`),
 KEY `targetX` (`targetX`),
 KEY `targetY` (`targetY`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




