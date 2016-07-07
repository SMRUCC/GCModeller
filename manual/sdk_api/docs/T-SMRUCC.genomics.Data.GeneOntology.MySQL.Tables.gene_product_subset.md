---
title: gene_product_subset
---

# gene_product_subset
_namespace: [SMRUCC.genomics.Data.GeneOntology.MySQL.Tables](N-SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.html)_

--
 
 DROP TABLE IF EXISTS `gene_product_subset`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `gene_product_subset` (
 `gene_product_id` int(11) NOT NULL,
 `subset_id` int(11) NOT NULL,
 UNIQUE KEY `gps3` (`gene_product_id`,`subset_id`),
 KEY `gps1` (`gene_product_id`),
 KEY `gps2` (`subset_id`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




