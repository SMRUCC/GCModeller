---
title: db
---

# db
_namespace: [SMRUCC.genomics.Data.GeneOntology.MySQL.Tables](N-SMRUCC.genomics.Data.GeneOntology.MySQL.Tables.html)_

--
 
 DROP TABLE IF EXISTS `db`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `db` (
 `id` int(11) NOT NULL AUTO_INCREMENT,
 `name` varchar(55) DEFAULT NULL,
 `fullname` varchar(255) DEFAULT NULL,
 `datatype` varchar(255) DEFAULT NULL,
 `generic_url` varchar(255) DEFAULT NULL,
 `url_syntax` varchar(255) DEFAULT NULL,
 `url_example` varchar(255) DEFAULT NULL,
 `uri_prefix` varchar(255) DEFAULT NULL,
 PRIMARY KEY (`id`),
 UNIQUE KEY `db0` (`id`),
 UNIQUE KEY `name` (`name`),
 KEY `db1` (`name`),
 KEY `db2` (`fullname`),
 KEY `db3` (`datatype`)
 ) ENGINE=MyISAM AUTO_INCREMENT=262 DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




