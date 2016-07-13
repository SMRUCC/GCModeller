---
title: association
---

# association
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `association`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `association` (
 `id` int(11) NOT NULL AUTO_INCREMENT,
 `term_id` int(11) NOT NULL DEFAULT '0',
 `gene_product_id` int(11) NOT NULL DEFAULT '0',
 `is_not` int(11) DEFAULT NULL,
 `role_group` int(11) DEFAULT NULL,
 `assocdate` int(11) DEFAULT NULL,
 `source_db_id` int(11) DEFAULT NULL,
 PRIMARY KEY (`id`),
 UNIQUE KEY `a0` (`id`),
 KEY `source_db_id` (`source_db_id`),
 KEY `a1` (`term_id`),
 KEY `a2` (`gene_product_id`),
 KEY `a3` (`term_id`,`gene_product_id`),
 KEY `a4` (`id`,`term_id`,`gene_product_id`),
 KEY `a5` (`id`,`gene_product_id`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




