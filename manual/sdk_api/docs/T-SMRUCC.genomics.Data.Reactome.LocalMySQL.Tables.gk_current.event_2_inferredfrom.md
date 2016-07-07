---
title: event_2_inferredfrom
---

# event_2_inferredfrom
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `event_2_inferredfrom`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `event_2_inferredfrom` (
 `DB_ID` int(10) unsigned DEFAULT NULL,
 `inferredFrom_rank` int(10) unsigned DEFAULT NULL,
 `inferredFrom` int(10) unsigned DEFAULT NULL,
 `inferredFrom_class` varchar(64) DEFAULT NULL,
 KEY `DB_ID` (`DB_ID`),
 KEY `inferredFrom` (`inferredFrom`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




