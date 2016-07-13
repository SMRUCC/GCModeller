---
title: graph_path
---

# graph_path
_namespace: [SMRUCC.genomics.Data.GeneOntology.MySQL.Tables](N-SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.html)_

--
 
 DROP TABLE IF EXISTS `graph_path`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `graph_path` (
 `id` int(11) NOT NULL AUTO_INCREMENT,
 `term1_id` int(11) NOT NULL,
 `term2_id` int(11) NOT NULL,
 `relationship_type_id` int(11) DEFAULT NULL,
 `distance` int(11) DEFAULT NULL,
 `relation_distance` int(11) DEFAULT NULL,
 PRIMARY KEY (`id`),
 UNIQUE KEY `graph_path0` (`id`),
 KEY `relationship_type_id` (`relationship_type_id`),
 KEY `graph_path1` (`term1_id`),
 KEY `graph_path2` (`term2_id`),
 KEY `graph_path3` (`term1_id`,`term2_id`),
 KEY `graph_path4` (`term1_id`,`distance`),
 KEY `graph_path5` (`term1_id`,`term2_id`,`relationship_type_id`),
 KEY `graph_path6` (`term1_id`,`term2_id`,`relationship_type_id`,`distance`,`relation_distance`),
 KEY `graph_path7` (`term2_id`,`relationship_type_id`),
 KEY `graph_path8` (`term1_id`,`relationship_type_id`)
 ) ENGINE=MyISAM AUTO_INCREMENT=1226557 DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




