# orthology_diseases
_namespace: [KEGG_tools.LocalMySQL](./index.md)_

--
 
 DROP TABLE IF EXISTS `orthology_diseases`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `orthology_diseases` (
 `entry_id` varchar(45) NOT NULL,
 `disease` varchar(45) NOT NULL,
 `id` int(11) NOT NULL AUTO_INCREMENT,
 `description` text,
 `url` text,
 PRIMARY KEY (`disease`,`entry_id`),
 UNIQUE KEY `id_UNIQUE` (`id`)
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




