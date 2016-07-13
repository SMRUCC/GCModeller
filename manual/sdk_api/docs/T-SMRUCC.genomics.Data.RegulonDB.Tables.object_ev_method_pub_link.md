---
title: object_ev_method_pub_link
---

# object_ev_method_pub_link
_namespace: [SMRUCC.genomics.Data.RegulonDB.Tables](N-SMRUCC.genomics.Data.RegulonDB.Tables.html)_

--
 
 DROP TABLE IF EXISTS `object_ev_method_pub_link`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `object_ev_method_pub_link` (
 `object_id` char(12) NOT NULL,
 `evidence_id` char(12) DEFAULT NULL,
 `method_id` char(12) DEFAULT NULL,
 `publication_id` char(12) DEFAULT NULL
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




