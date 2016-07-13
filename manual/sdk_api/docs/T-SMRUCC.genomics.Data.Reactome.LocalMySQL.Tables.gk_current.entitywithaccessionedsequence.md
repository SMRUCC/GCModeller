---
title: entitywithaccessionedsequence
---

# entitywithaccessionedsequence
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `entitywithaccessionedsequence`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `entitywithaccessionedsequence` (
 `DB_ID` int(10) unsigned NOT NULL,
 `endCoordinate` int(10) DEFAULT NULL,
 `referenceEntity` int(10) unsigned DEFAULT NULL,
 `referenceEntity_class` varchar(64) DEFAULT NULL,
 `startCoordinate` int(10) DEFAULT NULL,
 PRIMARY KEY (`DB_ID`),
 KEY `endCoordinate` (`endCoordinate`),
 KEY `referenceEntity` (`referenceEntity`),
 KEY `startCoordinate` (`startCoordinate`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




