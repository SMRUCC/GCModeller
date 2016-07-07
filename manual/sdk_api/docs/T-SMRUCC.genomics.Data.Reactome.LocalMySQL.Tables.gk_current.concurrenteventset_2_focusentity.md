---
title: concurrenteventset_2_focusentity
---

# concurrenteventset_2_focusentity
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `concurrenteventset_2_focusentity`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `concurrenteventset_2_focusentity` (
 `DB_ID` int(10) unsigned DEFAULT NULL,
 `focusEntity_rank` int(10) unsigned DEFAULT NULL,
 `focusEntity` int(10) unsigned DEFAULT NULL,
 `focusEntity_class` varchar(64) DEFAULT NULL,
 KEY `DB_ID` (`DB_ID`),
 KEY `focusEntity` (`focusEntity`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




