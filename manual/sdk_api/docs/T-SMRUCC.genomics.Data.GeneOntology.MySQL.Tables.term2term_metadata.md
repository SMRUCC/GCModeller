---
title: term2term_metadata
---

# term2term_metadata
_namespace: [SMRUCC.genomics.Data.GeneOntology.MySQL.Tables](N-SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.html)_

--
 
 DROP TABLE IF EXISTS `term2term_metadata`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `term2term_metadata` (
 `id` int(11) NOT NULL AUTO_INCREMENT,
 `relationship_type_id` int(11) NOT NULL,
 `term1_id` int(11) NOT NULL,
 `term2_id` int(11) NOT NULL,
 PRIMARY KEY (`id`),
 UNIQUE KEY `term1_id` (`term1_id`,`term2_id`),
 KEY `relationship_type_id` (`relationship_type_id`),
 KEY `term2_id` (`term2_id`)
 ) ENGINE=MyISAM AUTO_INCREMENT=2317 DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




