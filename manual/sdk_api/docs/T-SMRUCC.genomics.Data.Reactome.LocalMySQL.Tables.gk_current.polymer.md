---
title: polymer
---

# polymer
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `polymer`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `polymer` (
 `DB_ID` int(10) unsigned NOT NULL,
 `maxUnitCount` int(10) DEFAULT NULL,
 `minUnitCount` int(10) DEFAULT NULL,
 `totalProt` text,
 `maxHomologues` text,
 `inferredProt` text,
 PRIMARY KEY (`DB_ID`),
 KEY `maxUnitCount` (`maxUnitCount`),
 KEY `minUnitCount` (`minUnitCount`),
 FULLTEXT KEY `totalProt` (`totalProt`),
 FULLTEXT KEY `maxHomologues` (`maxHomologues`),
 FULLTEXT KEY `inferredProt` (`inferredProt`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




