---
title: entry
---

# entry
_namespace: [SMRUCC.genomics.Data.ExplorEnz.MySQL](N-SMRUCC.genomics.Data.ExplorEnz.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `entry`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `entry` (
 `ec_num` varchar(12) NOT NULL DEFAULT '',
 `accepted_name` varchar(300) DEFAULT NULL,
 `reaction` text,
 `other_names` text,
 `sys_name` text,
 `comments` text,
 `links` text,
 `class` int(1) DEFAULT NULL,
 `subclass` int(1) DEFAULT NULL,
 `subsubclass` int(1) DEFAULT NULL,
 `serial` int(1) DEFAULT NULL,
 `status` char(3) DEFAULT NULL,
 `diagram` varchar(255) DEFAULT NULL,
 `cas_num` varchar(100) DEFAULT NULL,
 `glossary` text,
 `last_change` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
 `id` int(11) NOT NULL AUTO_INCREMENT,
 UNIQUE KEY `id` (`id`),
 UNIQUE KEY `ec_num` (`ec_num`),
 FULLTEXT KEY `ec_num_2` (`ec_num`,`accepted_name`,`reaction`,`other_names`,`sys_name`,`comments`,`links`,`diagram`,`cas_num`,`glossary`)
 ) ENGINE=MyISAM AUTO_INCREMENT=6616 DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




