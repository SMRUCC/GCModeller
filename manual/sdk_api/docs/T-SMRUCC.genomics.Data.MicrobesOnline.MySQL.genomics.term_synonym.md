---
title: term_synonym
---

# term_synonym
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `term_synonym`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `term_synonym` (
 `term_id` int(11) NOT NULL DEFAULT '0',
 `term_synonym` text,
 `acc_synonym` varchar(255) DEFAULT NULL,
 `synonym_type_id` int(11) NOT NULL DEFAULT '0',
 `synonym_category_id` int(11) DEFAULT NULL,
 UNIQUE KEY `term_id` (`term_id`,`term_synonym`(100)),
 KEY `synonym_type_id` (`synonym_type_id`),
 KEY `synonym_category_id` (`synonym_category_id`),
 KEY `ts1` (`term_id`),
 KEY `ts2` (`term_synonym`(100)),
 KEY `ts3` (`term_id`,`term_synonym`(100))
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




