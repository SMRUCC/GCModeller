---
title: operon
---

# operon
_namespace: [SMRUCC.genomics.Data.RegulonDB.Tables](N-SMRUCC.genomics.Data.RegulonDB.Tables.html)_

--
 
 DROP TABLE IF EXISTS `operon`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `operon` (
 `operon_id` char(12) NOT NULL,
 `operon_name` varchar(255) NOT NULL,
 `firstgeneposleft` decimal(10,0) NOT NULL,
 `lastgeneposright` decimal(10,0) NOT NULL,
 `regulationposleft` decimal(10,0) NOT NULL,
 `regulationposright` decimal(10,0) NOT NULL,
 `operon_strand` varchar(10) DEFAULT NULL,
 `operon_internal_comment` longtext,
 `key_id_org` varchar(5) NOT NULL
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




