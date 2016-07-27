---
title: book
---

# book
_namespace: [SMRUCC.genomics.Analysis.Annotations.Interpro.Tables](N-SMRUCC.genomics.Analysis.Annotations.Interpro.Tables.html)_

--
 
 DROP TABLE IF EXISTS `book`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `book` (
 `isbn` varchar(10) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `title` mediumtext CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `edition` int(3) DEFAULT NULL,
 `publisher` varchar(200) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
 `pubplace` varchar(50) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
 PRIMARY KEY (`isbn`)
 ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




