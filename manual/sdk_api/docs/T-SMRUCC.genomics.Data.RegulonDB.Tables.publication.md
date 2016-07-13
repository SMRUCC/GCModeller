---
title: publication
---

# publication
_namespace: [SMRUCC.genomics.Data.RegulonDB.Tables](N-SMRUCC.genomics.Data.RegulonDB.Tables.html)_

--
 
 DROP TABLE IF EXISTS `publication`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `publication` (
 `publication_id` char(12) NOT NULL,
 `reference_id` varchar(255) NOT NULL,
 `external_db_id` char(12) NOT NULL,
 `author` varchar(2000) DEFAULT NULL,
 `title` varchar(2000) DEFAULT NULL,
 `source` varchar(2000) DEFAULT NULL,
 `years` varchar(50) DEFAULT NULL,
 `publication_note` varchar(2000) DEFAULT NULL,
 `publication_internal_comment` longtext
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




