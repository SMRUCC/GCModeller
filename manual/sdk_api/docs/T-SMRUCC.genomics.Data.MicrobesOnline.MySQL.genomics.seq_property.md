---
title: seq_property
---

# seq_property
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `seq_property`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `seq_property` (
 `id` int(11) NOT NULL AUTO_INCREMENT,
 `seq_id` int(11) NOT NULL DEFAULT '0',
 `property_key` varchar(64) NOT NULL DEFAULT '',
 `property_val` varchar(255) NOT NULL DEFAULT '',
 PRIMARY KEY (`id`),
 UNIQUE KEY `seq_id` (`seq_id`,`property_key`,`property_val`),
 KEY `seqp0` (`seq_id`),
 KEY `seqp1` (`property_key`),
 KEY `seqp2` (`property_val`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




