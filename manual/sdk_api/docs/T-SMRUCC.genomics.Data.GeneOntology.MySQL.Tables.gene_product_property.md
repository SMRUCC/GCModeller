---
title: gene_product_property
---

# gene_product_property
_namespace: [SMRUCC.genomics.Data.GeneOntology.MySQL.Tables](N-SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.html)_

--
 
 DROP TABLE IF EXISTS `gene_product_property`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `gene_product_property` (
 `gene_product_id` int(11) NOT NULL,
 `property_key` varchar(64) NOT NULL,
 `property_val` varchar(255) DEFAULT NULL,
 UNIQUE KEY `gppu4` (`gene_product_id`,`property_key`,`property_val`),
 KEY `gpp1` (`gene_product_id`),
 KEY `gpp2` (`property_key`),
 KEY `gpp3` (`property_val`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




