---
title: term2term
---

# term2term
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `term2term`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `term2term` (
 `id` int(11) NOT NULL AUTO_INCREMENT,
 `relationship_type_id` int(11) NOT NULL DEFAULT '0',
 `term1_id` int(11) NOT NULL DEFAULT '0',
 `term2_id` int(11) NOT NULL DEFAULT '0',
 `complete` int(11) NOT NULL DEFAULT '0',
 PRIMARY KEY (`id`),
 UNIQUE KEY `term1_id` (`term1_id`,`term2_id`,`relationship_type_id`,`complete`),
 KEY `tt1` (`term1_id`),
 KEY `tt2` (`term2_id`),
 KEY `tt3` (`term1_id`,`term2_id`),
 KEY `tt4` (`relationship_type_id`)
 ) ENGINE=MyISAM AUTO_INCREMENT=39874 DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




