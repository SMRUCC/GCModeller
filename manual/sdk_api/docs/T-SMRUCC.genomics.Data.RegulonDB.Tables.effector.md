---
title: effector
---

# effector
_namespace: [SMRUCC.genomics.Data.RegulonDB.Tables](N-SMRUCC.genomics.Data.RegulonDB.Tables.html)_

--
 
 DROP TABLE IF EXISTS `effector`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `effector` (
 `effector_id` char(12) NOT NULL,
 `effector_name` varchar(255) NOT NULL,
 `category` varchar(10) DEFAULT NULL,
 `effector_type` varchar(100) DEFAULT NULL,
 `effector_note` varchar(2000) DEFAULT NULL,
 `effector_internal_comment` longtext,
 `key_id_org` varchar(5) NOT NULL
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




