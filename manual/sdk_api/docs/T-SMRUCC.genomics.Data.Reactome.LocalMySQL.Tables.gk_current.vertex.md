---
title: vertex
---

# vertex
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `vertex`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `vertex` (
 `DB_ID` int(10) unsigned NOT NULL,
 `height` int(10) DEFAULT NULL,
 `pathwayDiagram` int(10) unsigned DEFAULT NULL,
 `pathwayDiagram_class` varchar(64) DEFAULT NULL,
 `representedInstance` int(10) unsigned DEFAULT NULL,
 `representedInstance_class` varchar(64) DEFAULT NULL,
 `width` int(10) DEFAULT NULL,
 `x` int(10) DEFAULT NULL,
 `y` int(10) DEFAULT NULL,
 PRIMARY KEY (`DB_ID`),
 KEY `height` (`height`),
 KEY `pathwayDiagram` (`pathwayDiagram`),
 KEY `representedInstance` (`representedInstance`),
 KEY `width` (`width`),
 KEY `x` (`x`),
 KEY `y` (`y`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




