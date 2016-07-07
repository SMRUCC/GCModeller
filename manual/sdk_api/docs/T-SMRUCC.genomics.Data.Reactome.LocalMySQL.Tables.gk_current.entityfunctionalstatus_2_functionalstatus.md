---
title: entityfunctionalstatus_2_functionalstatus
---

# entityfunctionalstatus_2_functionalstatus
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `entityfunctionalstatus_2_functionalstatus`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `entityfunctionalstatus_2_functionalstatus` (
 `DB_ID` int(10) unsigned DEFAULT NULL,
 `functionalStatus_rank` int(10) unsigned DEFAULT NULL,
 `functionalStatus` int(10) unsigned DEFAULT NULL,
 `functionalStatus_class` varchar(64) DEFAULT NULL,
 KEY `DB_ID` (`DB_ID`),
 KEY `functionalStatus` (`functionalStatus`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




