---
title: complex_2_interactionidentifier
---

# complex_2_interactionidentifier
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `complex_2_interactionidentifier`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `complex_2_interactionidentifier` (
 `DB_ID` int(10) unsigned DEFAULT NULL,
 `interactionIdentifier_rank` int(10) unsigned DEFAULT NULL,
 `interactionIdentifier` int(10) unsigned DEFAULT NULL,
 `interactionIdentifier_class` varchar(64) DEFAULT NULL,
 KEY `DB_ID` (`DB_ID`),
 KEY `interactionIdentifier` (`interactionIdentifier`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




