# pathway
_namespace: [KEGG_tools.LocalMySQL](./index.md)_

--
 
 DROP TABLE IF EXISTS `pathway`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `pathway` (
 `entry_id` varchar(45) NOT NULL,
 `name` longtext,
 `definition` longtext,
 `class` text,
 `category` text,
 PRIMARY KEY (`entry_id`),
 UNIQUE KEY `entry_id_UNIQUE` (`entry_id`)
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




