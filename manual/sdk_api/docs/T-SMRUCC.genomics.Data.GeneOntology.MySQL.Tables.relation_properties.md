---
title: relation_properties
---

# relation_properties
_namespace: [SMRUCC.genomics.Data.GeneOntology.MySQL.Tables](N-SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.html)_

--
 
 DROP TABLE IF EXISTS `relation_properties`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `relation_properties` (
 `relationship_type_id` int(11) NOT NULL,
 `is_transitive` int(11) DEFAULT NULL,
 `is_symmetric` int(11) DEFAULT NULL,
 `is_anti_symmetric` int(11) DEFAULT NULL,
 `is_cyclic` int(11) DEFAULT NULL,
 `is_reflexive` int(11) DEFAULT NULL,
 `is_metadata_tag` int(11) DEFAULT NULL,
 UNIQUE KEY `relationship_type_id` (`relationship_type_id`),
 UNIQUE KEY `rp1` (`relationship_type_id`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




