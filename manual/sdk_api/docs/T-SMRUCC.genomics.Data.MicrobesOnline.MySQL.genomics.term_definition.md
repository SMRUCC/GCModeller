---
title: term_definition
---

# term_definition
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `term_definition`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `term_definition` (
 `term_id` int(11) NOT NULL DEFAULT '0',
 `term_definition` text NOT NULL,
 `dbxref_id` int(11) DEFAULT NULL,
 `term_comment` mediumtext,
 `reference` varchar(255) DEFAULT NULL,
 UNIQUE KEY `term_id` (`term_id`),
 KEY `dbxref_id` (`dbxref_id`),
 KEY `td1` (`term_id`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




