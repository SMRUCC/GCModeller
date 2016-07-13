---
title: external_db
---

# external_db
_namespace: [SMRUCC.genomics.Data.RegulonDB.Tables](N-SMRUCC.genomics.Data.RegulonDB.Tables.html)_

--
 
 DROP TABLE IF EXISTS `external_db`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `external_db` (
 `external_db_id` char(12) NOT NULL,
 `external_db_name` varchar(255) NOT NULL,
 `external_db_description` varchar(255) DEFAULT NULL,
 `url` varchar(255) NOT NULL,
 `external_db_note` varchar(2000) DEFAULT NULL,
 `ext_db_internal_comment` longtext
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




