---
title: charts_tmp
---

# charts_tmp
_namespace: [SMRUCC.genomics.Data.RegulonDB.Tables](N-SMRUCC.genomics.Data.RegulonDB.Tables.html)_

--
 
 DROP TABLE IF EXISTS `charts_tmp`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `charts_tmp` (
 `chart_name` varchar(150) NOT NULL,
 `chart_type` varchar(150) NOT NULL,
 `chart_title` varchar(150) DEFAULT NULL,
 `title_x` varchar(150) DEFAULT NULL,
 `title_y` varchar(150) DEFAULT NULL,
 `object_name` varchar(150) DEFAULT NULL,
 `number_option` decimal(10,0) DEFAULT NULL,
 `query_number` decimal(10,0) DEFAULT NULL,
 `chart_id` decimal(15,5) NOT NULL
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




