---
title: regulation_2_containedinpathway
---

# regulation_2_containedinpathway
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `regulation_2_containedinpathway`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `regulation_2_containedinpathway` (
 `DB_ID` int(10) unsigned DEFAULT NULL,
 `containedInPathway_rank` int(10) unsigned DEFAULT NULL,
 `containedInPathway` int(10) unsigned DEFAULT NULL,
 `containedInPathway_class` varchar(64) DEFAULT NULL,
 KEY `DB_ID` (`DB_ID`),
 KEY `containedInPathway` (`containedInPathway`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




