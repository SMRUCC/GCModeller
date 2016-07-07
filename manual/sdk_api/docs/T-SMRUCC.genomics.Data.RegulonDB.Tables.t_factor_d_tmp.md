---
title: t_factor_d_tmp
---

# t_factor_d_tmp
_namespace: [SMRUCC.genomics.Data.RegulonDB.Tables](N-SMRUCC.genomics.Data.RegulonDB.Tables.html)_

--
 
 DROP TABLE IF EXISTS `t_factor_d_tmp`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `t_factor_d_tmp` (
 `tf_id` decimal(10,0) NOT NULL,
 `t_factor_id` char(12) NOT NULL,
 `t_factor_name` varchar(255) DEFAULT NULL,
 `t_factor_site_length` decimal(10,0) DEFAULT NULL,
 `t_factor_key_id_org` char(5) NOT NULL,
 `t_factor_site_group` decimal(10,0) NOT NULL
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




