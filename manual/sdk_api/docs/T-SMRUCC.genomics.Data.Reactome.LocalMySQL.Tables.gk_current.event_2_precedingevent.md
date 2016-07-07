---
title: event_2_precedingevent
---

# event_2_precedingevent
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `event_2_precedingevent`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `event_2_precedingevent` (
 `DB_ID` int(10) unsigned DEFAULT NULL,
 `precedingEvent_rank` int(10) unsigned DEFAULT NULL,
 `precedingEvent` int(10) unsigned DEFAULT NULL,
 `precedingEvent_class` varchar(64) DEFAULT NULL,
 KEY `DB_ID` (`DB_ID`),
 KEY `precedingEvent` (`precedingEvent`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




