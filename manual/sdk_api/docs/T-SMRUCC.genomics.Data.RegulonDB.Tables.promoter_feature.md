---
title: promoter_feature
---

# promoter_feature
_namespace: [SMRUCC.genomics.Data.RegulonDB.Tables](N-SMRUCC.genomics.Data.RegulonDB.Tables.html)_

--
 
 DROP TABLE IF EXISTS `promoter_feature`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `promoter_feature` (
 `promoter_feature_id` char(12) DEFAULT NULL,
 `promoter_id` char(12) DEFAULT NULL,
 `box_10_left` decimal(10,0) DEFAULT NULL,
 `box_10_right` decimal(10,0) DEFAULT NULL,
 `box_35_left` decimal(10,0) DEFAULT NULL,
 `box_35_right` decimal(10,0) DEFAULT NULL,
 `box_10_sequence` varchar(100) DEFAULT NULL,
 `box_35_sequence` varchar(100) DEFAULT NULL,
 `score` decimal(4,2) DEFAULT NULL,
 `relative_box_10_left` decimal(10,0) DEFAULT NULL,
 `relative_box_10_right` decimal(10,0) DEFAULT NULL,
 `relative_box_35_left` decimal(10,0) DEFAULT NULL,
 `relative_box_35_right` decimal(10,0) DEFAULT NULL
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




