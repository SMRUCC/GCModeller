---
title: gene_product_dbxref
---

# gene_product_dbxref
_namespace: [SMRUCC.genomics.Data.GeneOntology.MySQL.Tables](N-SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.html)_

--
 
 DROP TABLE IF EXISTS `gene_product_dbxref`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `gene_product_dbxref` (
 `gene_product_id` int(11) NOT NULL,
 `dbxref_id` int(11) NOT NULL,
 UNIQUE KEY `gpx3` (`gene_product_id`,`dbxref_id`),
 KEY `gpx1` (`gene_product_id`),
 KEY `gpx2` (`dbxref_id`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




