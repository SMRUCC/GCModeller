﻿# association_qualifier
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](./index.md)_

--
 
 DROP TABLE IF EXISTS `association_qualifier`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `association_qualifier` (
 `id` int(11) NOT NULL AUTO_INCREMENT,
 `association_id` int(11) NOT NULL DEFAULT '0',
 `term_id` int(11) NOT NULL DEFAULT '0',
 `value` varchar(255) DEFAULT NULL,
 PRIMARY KEY (`id`),
 KEY `term_id` (`term_id`),
 KEY `aq1` (`association_id`,`term_id`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




