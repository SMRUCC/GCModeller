---
title: author
---

# author
_namespace: [SMRUCC.genomics.Analysis.Annotations.Interpro.Tables](N-SMRUCC.genomics.Analysis.Annotations.Interpro.Tables.html)_

--
 
 DROP TABLE IF EXISTS `author`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `author` (
 `author_id` int(9) NOT NULL,
 `name` varchar(80) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `uppercase` varchar(80) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 PRIMARY KEY (`author_id`),
 UNIQUE KEY `ui_author$id$name` (`author_id`,`name`),
 UNIQUE KEY `uq_author$name` (`name`)
 ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




