---
title: method
---

# method
_namespace: [SMRUCC.genomics.Data.RegulonDB.Tables](N-SMRUCC.genomics.Data.RegulonDB.Tables.html)_

--
 
 DROP TABLE IF EXISTS `method`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `method` (
 `method_id` char(12) NOT NULL,
 `method_name` varchar(200) NOT NULL,
 `method_description` varchar(2000) DEFAULT NULL,
 `parameter_used` varchar(2000) DEFAULT NULL,
 `method_note` varchar(2000) DEFAULT NULL,
 `method_internal_comment` longtext,
 `key_id_org` varchar(5) NOT NULL
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




