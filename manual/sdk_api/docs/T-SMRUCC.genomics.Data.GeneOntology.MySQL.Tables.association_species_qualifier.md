---
title: association_species_qualifier
---

# association_species_qualifier
_namespace: [SMRUCC.genomics.Data.GeneOntology.MySQL.Tables](N-SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.html)_

--
 
 DROP TABLE IF EXISTS `association_species_qualifier`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `association_species_qualifier` (
 `id` int(11) NOT NULL AUTO_INCREMENT,
 `association_id` int(11) NOT NULL,
 `species_id` int(11) DEFAULT NULL,
 PRIMARY KEY (`id`),
 KEY `association_id` (`association_id`),
 KEY `species_id` (`species_id`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




