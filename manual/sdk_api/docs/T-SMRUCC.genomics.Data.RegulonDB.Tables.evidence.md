---
title: evidence
---

# evidence
_namespace: [SMRUCC.genomics.Data.RegulonDB.Tables](N-SMRUCC.genomics.Data.RegulonDB.Tables.html)_

--
 
 DROP TABLE IF EXISTS `evidence`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `evidence` (
 `evidence_id` char(12) NOT NULL,
 `evidence_name` varchar(2000) NOT NULL,
 `type_object` varchar(200) DEFAULT NULL,
 `evidence_code` varchar(30) DEFAULT NULL,
 `evidence_note` varchar(2000) DEFAULT NULL,
 `evidence_internal_comment` longtext,
 `key_id_org` varchar(5) NOT NULL,
 `evidence_type` char(3) DEFAULT NULL
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




