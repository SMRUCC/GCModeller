---
title: html
---

# html
_namespace: [SMRUCC.genomics.Data.ExplorEnz.MySQL](N-SMRUCC.genomics.Data.ExplorEnz.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `html`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `html` (
 `ec_num` varchar(12) NOT NULL DEFAULT '',
 `accepted_name` text,
 `reaction` text,
 `other_names` text,
 `sys_name` text,
 `comments` text,
 `links` text,
 `glossary` text,
 `last_change` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
 PRIMARY KEY (`ec_num`),
 UNIQUE KEY `ec_num` (`ec_num`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




