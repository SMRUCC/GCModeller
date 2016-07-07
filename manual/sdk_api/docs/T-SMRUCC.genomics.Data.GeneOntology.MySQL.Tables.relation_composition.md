---
title: relation_composition
---

# relation_composition
_namespace: [SMRUCC.genomics.Data.GeneOntology.MySQL.Tables](N-SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.html)_

--
 
 DROP TABLE IF EXISTS `relation_composition`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `relation_composition` (
 `id` int(11) NOT NULL AUTO_INCREMENT,
 `relation1_id` int(11) NOT NULL,
 `relation2_id` int(11) NOT NULL,
 `inferred_relation_id` int(11) NOT NULL,
 PRIMARY KEY (`id`),
 UNIQUE KEY `relation1_id` (`relation1_id`,`relation2_id`,`inferred_relation_id`),
 KEY `rc1` (`relation1_id`),
 KEY `rc2` (`relation2_id`),
 KEY `rc3` (`inferred_relation_id`),
 KEY `rc4` (`relation1_id`,`relation2_id`,`inferred_relation_id`)
 ) ENGINE=MyISAM AUTO_INCREMENT=20 DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




