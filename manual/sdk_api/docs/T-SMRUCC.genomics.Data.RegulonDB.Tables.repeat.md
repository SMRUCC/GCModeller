---
title: repeat
---

# repeat
_namespace: [SMRUCC.genomics.Data.RegulonDB.Tables](N-SMRUCC.genomics.Data.RegulonDB.Tables.html)_

--
 
 DROP TABLE IF EXISTS `repeat`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `repeat` (
 `repeat_id` char(12) NOT NULL,
 `repeat_posleft` decimal(10,0) NOT NULL,
 `repeat_posright` decimal(10,0) NOT NULL,
 `repeat_family` varchar(255) DEFAULT NULL,
 `repeat_note` varchar(2000) DEFAULT NULL,
 `repeat_internal_comment` longtext,
 `key_id_org` varchar(5) NOT NULL
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




