---
title: association_isoform
---

# association_isoform
_namespace: [SMRUCC.genomics.Data.GeneOntology.MySQL.Tables](N-SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.html)_

--
 
 DROP TABLE IF EXISTS `association_isoform`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `association_isoform` (
 `association_id` int(11) NOT NULL,
 `gene_product_id` int(11) NOT NULL,
 KEY `association_id` (`association_id`),
 KEY `gene_product_id` (`gene_product_id`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




