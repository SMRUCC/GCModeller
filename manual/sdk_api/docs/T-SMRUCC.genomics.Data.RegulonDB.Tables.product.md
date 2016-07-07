---
title: product
---

# product
_namespace: [SMRUCC.genomics.Data.RegulonDB.Tables](N-SMRUCC.genomics.Data.RegulonDB.Tables.html)_

--
 
 DROP TABLE IF EXISTS `product`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `product` (
 `product_id` char(12) NOT NULL,
 `product_name` varchar(255) DEFAULT NULL,
 `molecular_weigth` decimal(20,5) DEFAULT NULL,
 `isoelectric_point` decimal(20,10) DEFAULT NULL,
 `location` varchar(1000) DEFAULT NULL,
 `anticodon` varchar(10) DEFAULT NULL,
 `product_sequence` varchar(4000) DEFAULT NULL,
 `product_note` longtext,
 `product_internal_comment` longtext,
 `key_id_org` varchar(5) NOT NULL,
 `product_type` varchar(25) DEFAULT NULL
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




