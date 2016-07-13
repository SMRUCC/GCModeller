---
title: gene_product_ancestor
---

# gene_product_ancestor
_namespace: [SMRUCC.genomics.Data.GeneOntology.MySQL.Tables](N-SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.html)_

--
 
 DROP TABLE IF EXISTS `gene_product_ancestor`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `gene_product_ancestor` (
 `gene_product_id` int(11) NOT NULL,
 `ancestor_id` int(11) NOT NULL,
 `phylotree_id` int(11) NOT NULL,
 `branch_length` float DEFAULT NULL,
 `is_transitive` int(11) NOT NULL DEFAULT '0',
 UNIQUE KEY `gene_product_id` (`gene_product_id`,`ancestor_id`,`phylotree_id`),
 KEY `ancestor_id` (`ancestor_id`),
 KEY `phylotree_id` (`phylotree_id`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




