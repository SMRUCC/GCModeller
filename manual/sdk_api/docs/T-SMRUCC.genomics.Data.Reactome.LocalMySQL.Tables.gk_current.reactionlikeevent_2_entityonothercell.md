---
title: reactionlikeevent_2_entityonothercell
---

# reactionlikeevent_2_entityonothercell
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `reactionlikeevent_2_entityonothercell`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `reactionlikeevent_2_entityonothercell` (
 `DB_ID` int(10) unsigned DEFAULT NULL,
 `entityOnOtherCell_rank` int(10) unsigned DEFAULT NULL,
 `entityOnOtherCell` int(10) unsigned DEFAULT NULL,
 `entityOnOtherCell_class` varchar(64) DEFAULT NULL,
 KEY `DB_ID` (`DB_ID`),
 KEY `entityOnOtherCell` (`entityOnOtherCell`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




