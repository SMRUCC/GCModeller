---
title: phylotree_property
---

# phylotree_property
_namespace: [SMRUCC.genomics.Data.GeneOntology.MySQL.Tables](N-SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.html)_

--
 
 DROP TABLE IF EXISTS `phylotree_property`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `phylotree_property` (
 `id` int(11) NOT NULL AUTO_INCREMENT,
 `phylotree_id` int(11) NOT NULL,
 `property_key` varchar(64) NOT NULL,
 `property_val` mediumtext,
 PRIMARY KEY (`id`),
 KEY `phylotree_id` (`phylotree_id`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




