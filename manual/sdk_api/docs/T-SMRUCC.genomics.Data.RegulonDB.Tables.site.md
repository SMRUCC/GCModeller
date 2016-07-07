---
title: site
---

# site
_namespace: [SMRUCC.genomics.Data.RegulonDB.Tables](N-SMRUCC.genomics.Data.RegulonDB.Tables.html)_

--
 
 DROP TABLE IF EXISTS `site`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `site` (
 `site_id` char(12) NOT NULL,
 `site_posleft` decimal(10,0) NOT NULL,
 `site_posright` decimal(10,0) NOT NULL,
 `site_sequence` varchar(100) DEFAULT NULL,
 `site_note` varchar(2000) DEFAULT NULL,
 `site_internal_comment` longtext,
 `key_id_org` varchar(5) NOT NULL,
 `site_length` decimal(10,0) DEFAULT NULL
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




