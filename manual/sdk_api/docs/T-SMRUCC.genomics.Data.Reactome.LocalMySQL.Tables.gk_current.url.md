---
title: url
---

# url
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `url`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `url` (
 `DB_ID` int(10) unsigned NOT NULL,
 `uniformResourceLocator` text,
 PRIMARY KEY (`DB_ID`),
 FULLTEXT KEY `uniformResourceLocator` (`uniformResourceLocator`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




