---
title: graph_path
---

# graph_path
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `graph_path`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `graph_path` (
 `id` int(11) NOT NULL AUTO_INCREMENT,
 `term1_id` int(11) NOT NULL DEFAULT '0',
 `term2_id` int(11) NOT NULL DEFAULT '0',
 `distance` int(11) NOT NULL DEFAULT '0',
 PRIMARY KEY (`id`),
 UNIQUE KEY `graph_path0` (`id`),
 KEY `graph_path1` (`term1_id`),
 KEY `graph_path2` (`term2_id`),
 KEY `graph_path3` (`term1_id`,`term2_id`),
 KEY `graph_path4` (`term1_id`,`distance`)
 ) ENGINE=MyISAM AUTO_INCREMENT=565752 DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




