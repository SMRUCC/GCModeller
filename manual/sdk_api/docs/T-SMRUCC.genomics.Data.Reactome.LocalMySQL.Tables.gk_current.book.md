---
title: book
---

# book
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `book`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `book` (
 `DB_ID` int(10) unsigned NOT NULL,
 `ISBN` text,
 `chapterTitle` text,
 `pages` text,
 `publisher` int(10) unsigned DEFAULT NULL,
 `publisher_class` varchar(64) DEFAULT NULL,
 `year` int(10) DEFAULT NULL,
 PRIMARY KEY (`DB_ID`),
 KEY `publisher` (`publisher`),
 KEY `year` (`year`),
 FULLTEXT KEY `ISBN` (`ISBN`),
 FULLTEXT KEY `chapterTitle` (`chapterTitle`),
 FULLTEXT KEY `pages` (`pages`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




