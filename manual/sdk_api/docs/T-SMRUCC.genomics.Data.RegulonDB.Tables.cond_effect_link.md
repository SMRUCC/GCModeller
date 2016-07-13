---
title: cond_effect_link
---

# cond_effect_link
_namespace: [SMRUCC.genomics.Data.RegulonDB.Tables](N-SMRUCC.genomics.Data.RegulonDB.Tables.html)_

--
 
 DROP TABLE IF EXISTS `cond_effect_link`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `cond_effect_link` (
 `cond_effect_link_id` char(12) NOT NULL,
 `condition_id` char(12) NOT NULL,
 `medium_id` char(12) NOT NULL,
 `effect` varchar(250) NOT NULL,
 `cond_effect_link_notes` varchar(2000) DEFAULT NULL
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




