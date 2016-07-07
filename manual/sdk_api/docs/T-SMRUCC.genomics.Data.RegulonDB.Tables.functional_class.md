---
title: functional_class
---

# functional_class
_namespace: [SMRUCC.genomics.Data.RegulonDB.Tables](N-SMRUCC.genomics.Data.RegulonDB.Tables.html)_

--
 
 DROP TABLE IF EXISTS `functional_class`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `functional_class` (
 `functional_class_id` char(12) NOT NULL,
 `fc_description` varchar(500) NOT NULL,
 `fc_label_index` varchar(50) NOT NULL,
 `head_class` char(12) DEFAULT NULL,
 `color_class` varchar(20) DEFAULT NULL,
 `fc_reference` varchar(255) NOT NULL,
 `fc_note` varchar(2000) DEFAULT NULL,
 `fc_internal_comment` longtext,
 `key_id_org` varchar(5) NOT NULL
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




