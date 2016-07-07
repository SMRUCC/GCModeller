---
title: gene_product
---

# gene_product
_namespace: [SMRUCC.genomics.Data.GeneOntology.MySQL.Tables](N-SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.html)_

--
 
 DROP TABLE IF EXISTS `gene_product`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `gene_product` (
 `id` int(11) NOT NULL AUTO_INCREMENT,
 `symbol` varchar(128) NOT NULL,
 `dbxref_id` int(11) NOT NULL,
 `species_id` int(11) DEFAULT NULL,
 `type_id` int(11) DEFAULT NULL,
 `full_name` text,
 PRIMARY KEY (`id`),
 UNIQUE KEY `dbxref_id` (`dbxref_id`),
 UNIQUE KEY `g0` (`id`),
 KEY `type_id` (`type_id`),
 KEY `g1` (`symbol`),
 KEY `g2` (`dbxref_id`),
 KEY `g3` (`species_id`),
 KEY `g4` (`id`,`species_id`),
 KEY `g5` (`dbxref_id`,`species_id`),
 KEY `g6` (`id`,`dbxref_id`),
 KEY `g7` (`id`,`species_id`),
 KEY `g8` (`id`,`dbxref_id`,`species_id`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




