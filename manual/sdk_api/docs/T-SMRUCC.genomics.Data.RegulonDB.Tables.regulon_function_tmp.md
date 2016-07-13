---
title: regulon_function_tmp
---

# regulon_function_tmp
_namespace: [SMRUCC.genomics.Data.RegulonDB.Tables](N-SMRUCC.genomics.Data.RegulonDB.Tables.html)_

--
 
 DROP TABLE IF EXISTS `regulon_function_tmp`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `regulon_function_tmp` (
 `regulon_function_id` char(12) NOT NULL,
 `regulon_id` char(12) DEFAULT NULL,
 `regulon_function_name` varchar(500) NOT NULL,
 `key_id_org` varchar(5) NOT NULL
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




