---
title: gene_product_seq
---

# gene_product_seq
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `gene_product_seq`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `gene_product_seq` (
 `gene_product_id` int(11) NOT NULL DEFAULT '0',
 `seq_id` int(11) NOT NULL DEFAULT '0',
 `is_primary_seq` int(11) DEFAULT NULL,
 KEY `gpseq1` (`gene_product_id`),
 KEY `gpseq2` (`seq_id`),
 KEY `gpseq3` (`seq_id`,`gene_product_id`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




