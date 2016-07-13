---
title: homolset
---

# homolset
_namespace: [SMRUCC.genomics.Data.GeneOntology.MySQL.Tables](N-SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.html)_

--
 
 DROP TABLE IF EXISTS `homolset`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `homolset` (
 `id` int(11) NOT NULL AUTO_INCREMENT,
 `symbol` varchar(128) DEFAULT NULL,
 `dbxref_id` int(11) DEFAULT NULL,
 `target_gene_product_id` int(11) DEFAULT NULL,
 `taxon_id` int(11) DEFAULT NULL,
 `type_id` int(11) DEFAULT NULL,
 `description` text,
 PRIMARY KEY (`id`),
 UNIQUE KEY `dbxref_id` (`dbxref_id`),
 KEY `target_gene_product_id` (`target_gene_product_id`),
 KEY `taxon_id` (`taxon_id`),
 KEY `type_id` (`type_id`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




