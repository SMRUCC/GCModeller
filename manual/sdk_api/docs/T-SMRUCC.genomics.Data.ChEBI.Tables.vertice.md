---
title: vertice
---

# vertice
_namespace: [SMRUCC.genomics.Data.ChEBI.Tables](N-SMRUCC.genomics.Data.ChEBI.Tables.html)_

--
 
 DROP TABLE IF EXISTS `vertice`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `vertice` (
 `id` int(11) NOT NULL,
 `vertice_ref` varchar(60) NOT NULL,
 `compound_id` int(11) DEFAULT NULL,
 `ontology_id` int(11) NOT NULL,
 PRIMARY KEY (`id`),
 UNIQUE KEY `UNIQUE_ONTOLOGY_REF` (`vertice_ref`,`ontology_id`),
 KEY `ontology_id` (`ontology_id`),
 CONSTRAINT `FK_VERTICE_TO_ONTOLOGY` FOREIGN KEY (`ontology_id`) REFERENCES `ontology` (`id`)
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 /*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;
 
 /*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
 /*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
 /*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
 /*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
 /*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
 /*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
 /*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;
 
 -- Dump completed on 2015-10-22 16:20:17




