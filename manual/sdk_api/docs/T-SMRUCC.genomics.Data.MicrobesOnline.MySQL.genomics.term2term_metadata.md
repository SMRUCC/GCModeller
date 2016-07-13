---
title: term2term_metadata
---

# term2term_metadata
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `term2term_metadata`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `term2term_metadata` (
 `id` int(11) NOT NULL AUTO_INCREMENT,
 `relationship_type_id` int(11) NOT NULL DEFAULT '0',
 `term1_id` int(11) NOT NULL DEFAULT '0',
 `term2_id` int(11) NOT NULL DEFAULT '0',
 PRIMARY KEY (`id`),
 UNIQUE KEY `term1_id` (`term1_id`,`term2_id`),
 KEY `relationship_type_id` (`relationship_type_id`),
 KEY `term2_id` (`term2_id`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




