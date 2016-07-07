---
title: attenuator
---

# attenuator
_namespace: [SMRUCC.genomics.Data.RegulonDB.Tables](N-SMRUCC.genomics.Data.RegulonDB.Tables.html)_

--
 
 DROP TABLE IF EXISTS `attenuator`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `attenuator` (
 `attenuator_id` varchar(12) NOT NULL,
 `gene_id` char(12) DEFAULT NULL,
 `attenuator_type` varchar(16) DEFAULT NULL,
 `attenuator_strand` varchar(12) DEFAULT NULL
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




