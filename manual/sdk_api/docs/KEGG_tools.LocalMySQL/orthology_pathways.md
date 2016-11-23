# orthology_pathways
_namespace: [KEGG_tools.LocalMySQL](./index.md)_

--
 
 DROP TABLE IF EXISTS `orthology_pathways`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `orthology_pathways` (
 `entry_id` varchar(45) NOT NULL,
 `pathway` varchar(45) NOT NULL,
 `id` int(11) NOT NULL AUTO_INCREMENT,
 `describ` text,
 `url` text,
 PRIMARY KEY (`entry_id`,`pathway`),
 UNIQUE KEY `id_UNIQUE` (`id`)
 ) ENGINE=InnoDB AUTO_INCREMENT=17 DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




