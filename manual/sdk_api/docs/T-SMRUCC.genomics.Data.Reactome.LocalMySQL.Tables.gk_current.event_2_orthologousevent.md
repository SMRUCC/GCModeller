---
title: event_2_orthologousevent
---

# event_2_orthologousevent
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `event_2_orthologousevent`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `event_2_orthologousevent` (
 `DB_ID` int(10) unsigned DEFAULT NULL,
 `orthologousEvent_rank` int(10) unsigned DEFAULT NULL,
 `orthologousEvent` int(10) unsigned DEFAULT NULL,
 `orthologousEvent_class` varchar(64) DEFAULT NULL,
 KEY `DB_ID` (`DB_ID`),
 KEY `orthologousEvent` (`orthologousEvent`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




