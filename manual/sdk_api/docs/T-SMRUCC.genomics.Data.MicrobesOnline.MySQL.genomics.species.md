---
title: species
---

# species
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `species`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `species` (
 `id` int(11) NOT NULL AUTO_INCREMENT,
 `ncbi_taxa_id` int(11) DEFAULT NULL,
 `common_name` varchar(255) DEFAULT NULL,
 `lineage_string` text,
 `genus` varchar(55) DEFAULT NULL,
 `species` varchar(255) DEFAULT NULL,
 `parent_id` int(11) DEFAULT NULL,
 `left_value` int(11) DEFAULT NULL,
 `right_value` int(11) DEFAULT NULL,
 `taxonomic_rank` varchar(255) DEFAULT NULL,
 PRIMARY KEY (`id`),
 UNIQUE KEY `sp0` (`id`),
 UNIQUE KEY `ncbi_taxa_id` (`ncbi_taxa_id`),
 KEY `sp1` (`ncbi_taxa_id`),
 KEY `sp2` (`common_name`),
 KEY `sp3` (`genus`),
 KEY `sp4` (`species`),
 KEY `sp5` (`genus`,`species`),
 KEY `sp6` (`id`,`ncbi_taxa_id`),
 KEY `sp7` (`id`,`ncbi_taxa_id`,`genus`,`species`),
 KEY `sp8` (`parent_id`),
 KEY `sp9` (`left_value`),
 KEY `sp10` (`right_value`),
 KEY `sp11` (`left_value`,`right_value`),
 KEY `sp12` (`id`,`left_value`),
 KEY `sp13` (`genus`,`left_value`,`right_value`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




