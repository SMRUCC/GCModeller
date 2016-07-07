---
title: regulator
---

# regulator
_namespace: [SMRUCC.genomics.Data.RegulonDB.Tables](N-SMRUCC.genomics.Data.RegulonDB.Tables.html)_

--
 
 DROP TABLE IF EXISTS `regulator`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `regulator` (
 `regulator_id` varchar(12) NOT NULL,
 `product_id` char(12) DEFAULT NULL,
 `regulator_name` varchar(250) DEFAULT NULL,
 `regulator_internal_commnet` varchar(2000) DEFAULT NULL,
 `regulator_note` varchar(12) DEFAULT NULL,
 `key_id_org` varchar(5) NOT NULL
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




