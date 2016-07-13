---
title: compounds
---

# compounds
_namespace: [SMRUCC.genomics.Data.ChEBI.Tables](N-SMRUCC.genomics.Data.ChEBI.Tables.html)_

--
 
 DROP TABLE IF EXISTS `compounds`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `compounds` (
 `id` int(11) NOT NULL,
 `name` text,
 `source` varchar(32) NOT NULL,
 `parent_id` int(11) DEFAULT NULL,
 `chebi_accession` varchar(30) NOT NULL,
 `status` varchar(1) NOT NULL,
 `definition` text,
 `star` int(11) NOT NULL,
 `modified_on` text,
 `created_by` text,
 PRIMARY KEY (`id`)
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




