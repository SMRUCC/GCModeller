---
title: relation
---

# relation
_namespace: [SMRUCC.genomics.Data.ChEBI.Tables](N-SMRUCC.genomics.Data.ChEBI.Tables.html)_

--
 
 DROP TABLE IF EXISTS `relation`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `relation` (
 `id` int(11) NOT NULL,
 `type` text NOT NULL,
 `init_id` int(11) NOT NULL,
 `final_id` int(11) NOT NULL,
 `status` varchar(1) NOT NULL,
 PRIMARY KEY (`id`),
 KEY `final_id` (`final_id`),
 KEY `init_id` (`init_id`),
 CONSTRAINT `FK_RELATION_TO_FINAL_VERTICE` FOREIGN KEY (`final_id`) REFERENCES `vertice` (`id`),
 CONSTRAINT `FK_RELATION_TO_INIT_VERTICE` FOREIGN KEY (`init_id`) REFERENCES `vertice` (`id`)
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




