---
title: gene_product_count
---

# gene_product_count
_namespace: [SMRUCC.genomics.Data.GeneOntology.MySQL.Tables](N-SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.html)_

--
 
 DROP TABLE IF EXISTS `gene_product_count`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `gene_product_count` (
 `term_id` int(11) NOT NULL,
 `code` varchar(8) DEFAULT NULL,
 `speciesdbname` varchar(55) DEFAULT NULL,
 `species_id` int(11) DEFAULT NULL,
 `product_count` int(11) NOT NULL,
 KEY `species_id` (`species_id`),
 KEY `gpc1` (`term_id`),
 KEY `gpc2` (`code`),
 KEY `gpc3` (`speciesdbname`),
 KEY `gpc4` (`term_id`,`code`,`speciesdbname`),
 KEY `gpc5` (`term_id`,`species_id`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




