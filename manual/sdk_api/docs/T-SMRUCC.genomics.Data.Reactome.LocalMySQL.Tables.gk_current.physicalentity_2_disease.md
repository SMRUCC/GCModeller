---
title: physicalentity_2_disease
---

# physicalentity_2_disease
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `physicalentity_2_disease`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `physicalentity_2_disease` (
 `DB_ID` int(10) unsigned DEFAULT NULL,
 `disease_rank` int(10) unsigned DEFAULT NULL,
 `disease` int(10) unsigned DEFAULT NULL,
 `disease_class` varchar(64) DEFAULT NULL,
 KEY `DB_ID` (`DB_ID`),
 KEY `disease` (`disease`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




