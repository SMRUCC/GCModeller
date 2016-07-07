---
title: shine_dalgarno
---

# shine_dalgarno
_namespace: [SMRUCC.genomics.Data.RegulonDB.Tables](N-SMRUCC.genomics.Data.RegulonDB.Tables.html)_

--
 
 DROP TABLE IF EXISTS `shine_dalgarno`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `shine_dalgarno` (
 `shine_dalgarno_id` char(12) NOT NULL,
 `gene_id` char(12) NOT NULL,
 `shine_dalgarno_dist_gene` decimal(10,0) NOT NULL,
 `shine_dalgarno_posleft` decimal(10,0) DEFAULT NULL,
 `shine_dalgarno_posright` decimal(10,0) DEFAULT NULL,
 `shine_dalgarno_sequence` varchar(200) DEFAULT NULL,
 `shine_dalgarno_note` varchar(2000) DEFAULT NULL,
 `sd_internal_comment` longtext,
 `key_id_org` varchar(5) NOT NULL
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




