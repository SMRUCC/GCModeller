---
title: reg_phrase
---

# reg_phrase
_namespace: [SMRUCC.genomics.Data.RegulonDB.Tables](N-SMRUCC.genomics.Data.RegulonDB.Tables.html)_

--
 
 DROP TABLE IF EXISTS `reg_phrase`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `reg_phrase` (
 `reg_phrase_id` char(12) NOT NULL,
 `reg_phrase_description` varchar(255) NOT NULL,
 `regulation_ratio` varchar(20) DEFAULT NULL,
 `on_half_life` decimal(20,5) DEFAULT NULL,
 `off_half_life` decimal(20,5) DEFAULT NULL,
 `phrase` varchar(2000) NOT NULL,
 `reg_phrase_function` varchar(25) NOT NULL,
 `reg_phrase_note` varchar(2000) DEFAULT NULL,
 `reg_phrase_internal_comment` longtext,
 `key_id_org` varchar(5) NOT NULL
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




