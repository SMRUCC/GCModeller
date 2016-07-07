---
title: organism
---

# organism
_namespace: [SMRUCC.genomics.Data.RegulonDB.Tables](N-SMRUCC.genomics.Data.RegulonDB.Tables.html)_

--
 
 DROP TABLE IF EXISTS `organism`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `organism` (
 `organism_id` char(12) NOT NULL,
 `organism_name` varchar(1000) NOT NULL,
 `organism_description` varchar(2000) DEFAULT NULL,
 `organism_note` varchar(2000) DEFAULT NULL,
 `organism_internal_comment` longtext
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




