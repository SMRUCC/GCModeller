---
title: gene_product_ancestor
---

# gene_product_ancestor
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `gene_product_ancestor`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `gene_product_ancestor` (
 `gene_product_id` int(11) NOT NULL DEFAULT '0',
 `ancestor_id` int(11) NOT NULL DEFAULT '0',
 KEY `gene_product_id` (`gene_product_id`),
 KEY `ancestor_id` (`ancestor_id`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




