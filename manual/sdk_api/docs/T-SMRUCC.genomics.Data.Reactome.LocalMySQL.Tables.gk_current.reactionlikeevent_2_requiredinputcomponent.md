---
title: reactionlikeevent_2_requiredinputcomponent
---

# reactionlikeevent_2_requiredinputcomponent
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `reactionlikeevent_2_requiredinputcomponent`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `reactionlikeevent_2_requiredinputcomponent` (
 `DB_ID` int(10) unsigned DEFAULT NULL,
 `requiredInputComponent_rank` int(10) unsigned DEFAULT NULL,
 `requiredInputComponent` int(10) unsigned DEFAULT NULL,
 `requiredInputComponent_class` varchar(64) DEFAULT NULL,
 KEY `DB_ID` (`DB_ID`),
 KEY `requiredInputComponent` (`requiredInputComponent`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




