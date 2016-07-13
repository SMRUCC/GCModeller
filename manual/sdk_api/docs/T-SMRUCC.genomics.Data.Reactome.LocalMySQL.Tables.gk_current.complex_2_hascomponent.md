---
title: complex_2_hascomponent
---

# complex_2_hascomponent
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `complex_2_hascomponent`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `complex_2_hascomponent` (
 `DB_ID` int(10) unsigned DEFAULT NULL,
 `hasComponent_rank` int(10) unsigned DEFAULT NULL,
 `hasComponent` int(10) unsigned DEFAULT NULL,
 `hasComponent_class` varchar(64) DEFAULT NULL,
 KEY `DB_ID` (`DB_ID`),
 KEY `hasComponent` (`hasComponent`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




