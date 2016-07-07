---
title: controlledvocabulary
---

# controlledvocabulary
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `controlledvocabulary`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `controlledvocabulary` (
 `DB_ID` int(10) unsigned NOT NULL,
 `definition` text,
 PRIMARY KEY (`DB_ID`),
 FULLTEXT KEY `definition` (`definition`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




