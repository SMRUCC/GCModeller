---
title: gene_product_homology
---

# gene_product_homology
_namespace: [SMRUCC.genomics.Data.GeneOntology.MySQL.Tables](N-SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.html)_

--
 
 DROP TABLE IF EXISTS `gene_product_homology`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `gene_product_homology` (
 `gene_product1_id` int(11) NOT NULL,
 `gene_product2_id` int(11) NOT NULL,
 `relationship_type_id` int(11) NOT NULL,
 KEY `gene_product1_id` (`gene_product1_id`),
 KEY `gene_product2_id` (`gene_product2_id`),
 KEY `relationship_type_id` (`relationship_type_id`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




