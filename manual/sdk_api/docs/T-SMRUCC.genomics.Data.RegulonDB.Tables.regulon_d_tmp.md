---
title: regulon_d_tmp
---

# regulon_d_tmp
_namespace: [SMRUCC.genomics.Data.RegulonDB.Tables](N-SMRUCC.genomics.Data.RegulonDB.Tables.html)_

--
 
 DROP TABLE IF EXISTS `regulon_d_tmp`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `regulon_d_tmp` (
 `re_id` decimal(10,0) NOT NULL,
 `regulon_id` char(12) NOT NULL,
 `regulon_name` varchar(500) DEFAULT NULL,
 `regulon_key_id_org` char(5) NOT NULL,
 `regulon_tf_group` decimal(10,0) NOT NULL
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




