---
title: promoterfeature
---

# promoterfeature
_namespace: [SMRUCC.genomics.Data.RegulonDB.Tables](N-SMRUCC.genomics.Data.RegulonDB.Tables.html)_

--
 
 DROP TABLE IF EXISTS `promoterfeature`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `promoterfeature` (
 `promoter_feature_id` char(12) DEFAULT NULL,
 `promoter_id` char(12) DEFAULT NULL,
 `feature` varchar(12) DEFAULT NULL,
 `pfeature_posleft` decimal(10,0) DEFAULT NULL,
 `pfeature_posright` decimal(10,0) DEFAULT NULL,
 `pfeature_sequence` varchar(100) DEFAULT NULL,
 `pfeature_relative_posleft` decimal(10,0) DEFAULT NULL,
 `pfeature_relative_posright` decimal(10,0) DEFAULT NULL,
 `pfeature_score` decimal(4,2) DEFAULT NULL,
 `pfeature_box_pair` char(12) DEFAULT NULL
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




