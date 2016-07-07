---
title: gene_product_homology
---

# gene_product_homology
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `gene_product_homology`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `gene_product_homology` (
 `gene_product1_id` int(11) NOT NULL DEFAULT '0',
 `gene_product2_id` int(11) NOT NULL DEFAULT '0',
 `relationship_type_id` int(11) NOT NULL DEFAULT '0',
 KEY `gene_product1_id` (`gene_product1_id`),
 KEY `gene_product2_id` (`gene_product2_id`),
 KEY `relationship_type_id` (`relationship_type_id`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




