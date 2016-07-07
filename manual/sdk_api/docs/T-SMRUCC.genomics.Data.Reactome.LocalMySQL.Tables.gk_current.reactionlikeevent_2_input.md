---
title: reactionlikeevent_2_input
---

# reactionlikeevent_2_input
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `reactionlikeevent_2_input`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `reactionlikeevent_2_input` (
 `DB_ID` int(10) unsigned DEFAULT NULL,
 `input_rank` int(10) unsigned DEFAULT NULL,
 `input` int(10) unsigned DEFAULT NULL,
 `input_class` varchar(64) DEFAULT NULL,
 KEY `DB_ID` (`DB_ID`),
 KEY `input` (`input`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




