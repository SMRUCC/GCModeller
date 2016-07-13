---
title: term
---

# term
_namespace: [SMRUCC.genomics.Data.GeneOntology.MySQL.Tables](N-SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.html)_

--
 
 DROP TABLE IF EXISTS `term`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `term` (
 `id` int(11) NOT NULL AUTO_INCREMENT,
 `name` varchar(255) NOT NULL DEFAULT '',
 `term_type` varchar(55) NOT NULL,
 `acc` varchar(255) NOT NULL,
 `is_obsolete` int(11) NOT NULL DEFAULT '0',
 `is_root` int(11) NOT NULL DEFAULT '0',
 `is_relation` int(11) NOT NULL DEFAULT '0',
 PRIMARY KEY (`id`),
 UNIQUE KEY `acc` (`acc`),
 UNIQUE KEY `t0` (`id`),
 KEY `t1` (`name`),
 KEY `t2` (`term_type`),
 KEY `t3` (`acc`),
 KEY `t4` (`id`,`acc`),
 KEY `t5` (`id`,`name`),
 KEY `t6` (`id`,`term_type`),
 KEY `t7` (`id`,`acc`,`name`,`term_type`)
 ) ENGINE=MyISAM AUTO_INCREMENT=43827 DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




