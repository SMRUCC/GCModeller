---
title: database_accession
---

# database_accession
_namespace: [SMRUCC.genomics.Data.ChEBI.Tables](N-SMRUCC.genomics.Data.ChEBI.Tables.html)_

--
 
 DROP TABLE IF EXISTS `database_accession`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `database_accession` (
 `id` int(11) NOT NULL,
 `compound_id` int(11) NOT NULL,
 `accession_number` varchar(255) NOT NULL,
 `type` text NOT NULL,
 `source` text NOT NULL,
 PRIMARY KEY (`id`),
 KEY `compound_id` (`compound_id`),
 CONSTRAINT `FK_DB_ACCESSION_TO_COMPOUND` FOREIGN KEY (`compound_id`) REFERENCES `compounds` (`id`)
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




