---
title: chemical_data
---

# chemical_data
_namespace: [SMRUCC.genomics.Data.ChEBI.Tables](N-SMRUCC.genomics.Data.ChEBI.Tables.html)_

--
 
 DROP TABLE IF EXISTS `chemical_data`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `chemical_data` (
 `id` int(11) NOT NULL,
 `compound_id` int(11) NOT NULL,
 `chemical_data` text NOT NULL,
 `source` text NOT NULL,
 `type` text NOT NULL,
 PRIMARY KEY (`id`),
 KEY `compound_id` (`compound_id`),
 CONSTRAINT `FK_CHEMICAL_DATA_TO_COMPOUND` FOREIGN KEY (`compound_id`) REFERENCES `compounds` (`id`)
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




