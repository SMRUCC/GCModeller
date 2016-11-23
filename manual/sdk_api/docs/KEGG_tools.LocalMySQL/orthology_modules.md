# orthology_modules
_namespace: [KEGG_tools.LocalMySQL](./index.md)_

--
 
 DROP TABLE IF EXISTS `orthology_modules`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `orthology_modules` (
 `entry_id` varchar(45) NOT NULL,
 `module` varchar(45) NOT NULL,
 `id` int(11) NOT NULL AUTO_INCREMENT,
 PRIMARY KEY (`module`,`entry_id`),
 UNIQUE KEY `id_UNIQUE` (`id`)
 ) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




