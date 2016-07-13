---
title: gene_product_property
---

# gene_product_property
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `gene_product_property`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `gene_product_property` (
 `gene_product_id` int(11) NOT NULL DEFAULT '0',
 `property_key` varchar(64) NOT NULL DEFAULT '',
 `property_val` varchar(255) DEFAULT NULL,
 UNIQUE KEY `gppu4` (`gene_product_id`,`property_key`,`property_val`),
 KEY `gpp1` (`gene_product_id`),
 KEY `gpp2` (`property_key`),
 KEY `gpp3` (`property_val`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




