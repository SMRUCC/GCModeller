﻿# association_property
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](./index.md)_

--
 
 DROP TABLE IF EXISTS `association_property`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `association_property` (
 `id` int(11) NOT NULL AUTO_INCREMENT,
 `association_id` int(11) NOT NULL DEFAULT '0',
 `relationship_type_id` int(11) NOT NULL DEFAULT '0',
 `term_id` int(11) NOT NULL DEFAULT '0',
 PRIMARY KEY (`id`),
 KEY `association_id` (`association_id`),
 KEY `relationship_type_id` (`relationship_type_id`),
 KEY `term_id` (`term_id`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




