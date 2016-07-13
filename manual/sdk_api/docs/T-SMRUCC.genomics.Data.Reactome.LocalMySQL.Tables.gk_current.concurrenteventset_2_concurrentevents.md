---
title: concurrenteventset_2_concurrentevents
---

# concurrenteventset_2_concurrentevents
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `concurrenteventset_2_concurrentevents`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `concurrenteventset_2_concurrentevents` (
 `DB_ID` int(10) unsigned DEFAULT NULL,
 `concurrentEvents_rank` int(10) unsigned DEFAULT NULL,
 `concurrentEvents` int(10) unsigned DEFAULT NULL,
 `concurrentEvents_class` varchar(64) DEFAULT NULL,
 KEY `DB_ID` (`DB_ID`),
 KEY `concurrentEvents` (`concurrentEvents`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




