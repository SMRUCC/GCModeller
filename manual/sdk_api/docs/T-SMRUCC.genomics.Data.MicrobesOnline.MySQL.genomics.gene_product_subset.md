---
title: gene_product_subset
---

# gene_product_subset
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `gene_product_subset`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `gene_product_subset` (
 `gene_product_id` int(11) NOT NULL DEFAULT '0',
 `subset_id` int(11) NOT NULL DEFAULT '0',
 UNIQUE KEY `gps3` (`gene_product_id`,`subset_id`),
 KEY `gps1` (`gene_product_id`),
 KEY `gps2` (`subset_id`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




