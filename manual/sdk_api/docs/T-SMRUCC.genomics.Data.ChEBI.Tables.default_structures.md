---
title: default_structures
---

# default_structures
_namespace: [SMRUCC.genomics.Data.ChEBI.Tables](N-SMRUCC.genomics.Data.ChEBI.Tables.html)_

--
 
 DROP TABLE IF EXISTS `default_structures`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `default_structures` (
 `id` int(11) NOT NULL,
 `structure_id` int(11) NOT NULL,
 PRIMARY KEY (`id`),
 KEY `FK_STRUCTURES_TO_DEFAULT_STRUC` (`structure_id`),
 CONSTRAINT `FK_STRUCTURES_TO_DEFAULT_STRUC` FOREIGN KEY (`structure_id`) REFERENCES `structures` (`id`)
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




