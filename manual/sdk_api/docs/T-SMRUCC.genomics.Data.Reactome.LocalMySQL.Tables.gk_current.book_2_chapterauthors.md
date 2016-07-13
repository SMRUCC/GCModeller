---
title: book_2_chapterauthors
---

# book_2_chapterauthors
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current](N-SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_current.html)_

--
 
 DROP TABLE IF EXISTS `book_2_chapterauthors`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `book_2_chapterauthors` (
 `DB_ID` int(10) unsigned DEFAULT NULL,
 `chapterAuthors_rank` int(10) unsigned DEFAULT NULL,
 `chapterAuthors` int(10) unsigned DEFAULT NULL,
 `chapterAuthors_class` varchar(64) DEFAULT NULL,
 KEY `DB_ID` (`DB_ID`),
 KEY `chapterAuthors` (`chapterAuthors`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




