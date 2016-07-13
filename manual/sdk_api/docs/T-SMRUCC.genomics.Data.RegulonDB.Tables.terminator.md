---
title: terminator
---

# terminator
_namespace: [SMRUCC.genomics.Data.RegulonDB.Tables](N-SMRUCC.genomics.Data.RegulonDB.Tables.html)_

--
 
 DROP TABLE IF EXISTS `terminator`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `terminator` (
 `terminator_id` char(12) NOT NULL,
 `terminator_dist_gene` decimal(10,0) DEFAULT NULL,
 `terminator_posleft` decimal(10,0) DEFAULT NULL,
 `terminator_posright` decimal(10,0) DEFAULT NULL,
 `terminator_class` varchar(30) DEFAULT NULL,
 `terminator_sequence` varchar(200) DEFAULT NULL,
 `terminator_note` varchar(2000) DEFAULT NULL,
 `terminator_internal_comment` longtext,
 `key_id_org` varchar(5) NOT NULL
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




