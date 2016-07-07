---
title: gene_product_homolset
---

# gene_product_homolset
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `gene_product_homolset`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `gene_product_homolset` (
 `id` int(11) NOT NULL AUTO_INCREMENT,
 `gene_product_id` int(11) NOT NULL DEFAULT '0',
 `homolset_id` int(11) NOT NULL DEFAULT '0',
 PRIMARY KEY (`id`),
 KEY `gene_product_id` (`gene_product_id`),
 KEY `homolset_id` (`homolset_id`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




