---
title: person
---

# person
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `person`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `person` (
 `DB_ID` int(10) unsigned NOT NULL,
 `eMailAddress` varchar(255) DEFAULT NULL,
 `firstname` text,
 `initial` varchar(10) DEFAULT NULL,
 `project` text,
 `surname` varchar(255) DEFAULT NULL,
 `url` text,
 PRIMARY KEY (`DB_ID`),
 KEY `eMailAddress` (`eMailAddress`),
 KEY `initial` (`initial`),
 KEY `surname` (`surname`),
 FULLTEXT KEY `eMailAddress_fulltext` (`eMailAddress`),
 FULLTEXT KEY `firstname` (`firstname`),
 FULLTEXT KEY `initial_fulltext` (`initial`),
 FULLTEXT KEY `project` (`project`),
 FULLTEXT KEY `surname_fulltext` (`surname`),
 FULLTEXT KEY `url` (`url`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




