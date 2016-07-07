---
title: reg_phrase_ri_link
---

# reg_phrase_ri_link
_namespace: [SMRUCC.genomics.Data.RegulonDB.Tables](N-SMRUCC.genomics.Data.RegulonDB.Tables.html)_

--
 
 DROP TABLE IF EXISTS `reg_phrase_ri_link`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `reg_phrase_ri_link` (
 `reg_phrase_id` char(12) NOT NULL,
 `regulatory_interaction_id` char(12) NOT NULL
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




