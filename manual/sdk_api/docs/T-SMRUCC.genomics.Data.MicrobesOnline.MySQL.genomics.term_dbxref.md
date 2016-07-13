---
title: term_dbxref
---

# term_dbxref
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `term_dbxref`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `term_dbxref` (
 `term_id` int(11) NOT NULL DEFAULT '0',
 `dbxref_id` int(11) NOT NULL DEFAULT '0',
 `is_for_definition` int(11) NOT NULL DEFAULT '0',
 UNIQUE KEY `term_id` (`term_id`,`dbxref_id`,`is_for_definition`),
 KEY `tx0` (`term_id`),
 KEY `tx1` (`dbxref_id`),
 KEY `tx2` (`term_id`,`dbxref_id`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




