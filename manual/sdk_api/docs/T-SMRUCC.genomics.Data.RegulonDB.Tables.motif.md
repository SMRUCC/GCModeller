---
title: motif
---

# motif
_namespace: [SMRUCC.genomics.Data.RegulonDB.Tables](N-SMRUCC.genomics.Data.RegulonDB.Tables.html)_

--
 
 DROP TABLE IF EXISTS `motif`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `motif` (
 `motif_id` char(12) NOT NULL,
 `product_id` char(12) NOT NULL,
 `motif_posleft` decimal(10,0) NOT NULL,
 `motif_posright` decimal(10,0) NOT NULL,
 `motif_sequence` varchar(3000) DEFAULT NULL,
 `motif_description` varchar(4000) DEFAULT NULL,
 `motif_type` varchar(255) DEFAULT NULL,
 `motif_note` varchar(2000) DEFAULT NULL,
 `motif_internal_comment` longtext,
 `key_id_org` varchar(5) NOT NULL
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




