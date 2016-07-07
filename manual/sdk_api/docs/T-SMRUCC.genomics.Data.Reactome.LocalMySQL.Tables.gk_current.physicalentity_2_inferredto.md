---
title: physicalentity_2_inferredto
---

# physicalentity_2_inferredto
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `physicalentity_2_inferredto`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `physicalentity_2_inferredto` (
 `DB_ID` int(10) unsigned DEFAULT NULL,
 `inferredTo_rank` int(10) unsigned DEFAULT NULL,
 `inferredTo` int(10) unsigned DEFAULT NULL,
 `inferredTo_class` varchar(64) DEFAULT NULL,
 KEY `DB_ID` (`DB_ID`),
 KEY `inferredTo` (`inferredTo`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




