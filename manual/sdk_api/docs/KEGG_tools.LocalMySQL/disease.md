# disease
_namespace: [KEGG_tools.LocalMySQL](./index.md)_

--
 
 DROP TABLE IF EXISTS `disease`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `disease` (
 `entry_id` varchar(45) NOT NULL,
 `definition` longtext,
 `guid` int(11) NOT NULL AUTO_INCREMENT,
 PRIMARY KEY (`entry_id`),
 UNIQUE KEY `guid_UNIQUE` (`guid`),
 UNIQUE KEY `entry_id_UNIQUE` (`entry_id`)
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




