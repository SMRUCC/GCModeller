---
title: catalystactivity
---

# catalystactivity
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `catalystactivity`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `catalystactivity` (
 `DB_ID` int(10) unsigned NOT NULL,
 `activity` int(10) unsigned DEFAULT NULL,
 `activity_class` varchar(64) DEFAULT NULL,
 `physicalEntity` int(10) unsigned DEFAULT NULL,
 `physicalEntity_class` varchar(64) DEFAULT NULL,
 PRIMARY KEY (`DB_ID`),
 KEY `activity` (`activity`),
 KEY `physicalEntity` (`physicalEntity`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




