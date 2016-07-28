---
title: common_annotation
---

# common_annotation
_namespace: [SMRUCC.genomics.Analysis.Annotations.Interpro.Tables](N-SMRUCC.genomics.Analysis.Annotations.Interpro.Tables.html)_

--
 
 DROP TABLE IF EXISTS `common_annotation`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `common_annotation` (
 `ann_id` varchar(7) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `name` varchar(50) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
 `text` mediumtext CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
 `comments` varchar(100) CHARACTER SET latin1 COLLATE latin1_bin DEFAULT NULL,
 PRIMARY KEY (`ann_id`)
 ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




