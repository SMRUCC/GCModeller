---
title: gene_product_synonym
---

# gene_product_synonym
_namespace: [SMRUCC.genomics.Data.GeneOntology.MySQL.Tables](N-SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.html)_

--
 
 DROP TABLE IF EXISTS `gene_product_synonym`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `gene_product_synonym` (
 `gene_product_id` int(11) NOT NULL,
 `product_synonym` varchar(255) NOT NULL,
 UNIQUE KEY `gene_product_id` (`gene_product_id`,`product_synonym`),
 KEY `gs1` (`gene_product_id`),
 KEY `gs2` (`product_synonym`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




