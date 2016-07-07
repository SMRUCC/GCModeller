---
title: assoc_rel
---

# assoc_rel
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `assoc_rel`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `assoc_rel` (
 `id` int(11) NOT NULL AUTO_INCREMENT,
 `from_id` int(11) NOT NULL DEFAULT '0',
 `to_id` int(11) NOT NULL DEFAULT '0',
 `relationship_type_id` int(11) NOT NULL DEFAULT '0',
 PRIMARY KEY (`id`),
 KEY `from_id` (`from_id`),
 KEY `to_id` (`to_id`),
 KEY `relationship_type_id` (`relationship_type_id`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




