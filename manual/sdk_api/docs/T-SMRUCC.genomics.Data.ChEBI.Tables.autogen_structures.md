---
title: autogen_structures
---

# autogen_structures
_namespace: [SMRUCC.genomics.Data.ChEBI.Tables](N-SMRUCC.genomics.Data.ChEBI.Tables.html)_

--
 
 DROP TABLE IF EXISTS `autogen_structures`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `autogen_structures` (
 `id` int(11) NOT NULL,
 `structure_id` int(11) NOT NULL,
 PRIMARY KEY (`id`),
 KEY `FK_STRUCTURES_TO_AUTOGEN_STRUC` (`structure_id`),
 CONSTRAINT `FK_STRUCTURES_TO_AUTOGEN_STRUC` FOREIGN KEY (`structure_id`) REFERENCES `structures` (`id`)
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




