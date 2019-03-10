# orthology_genes
_namespace: [KEGG_tools.LocalMySQL](./index.md)_

--
 
 DROP TABLE IF EXISTS `orthology_genes`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `orthology_genes` (
 `ko` varchar(100) NOT NULL,
 `gene` varchar(100) NOT NULL,
 `id` int(11) NOT NULL AUTO_INCREMENT,
 `url` text,
 `sp_code` varchar(45) DEFAULT NULL COMMENT 'The bacterial genome name brief code in KEGG database',
 `name` varchar(45) DEFAULT NULL,
 PRIMARY KEY (`gene`,`ko`),
 UNIQUE KEY `id_UNIQUE` (`id`)
 ) ENGINE=InnoDB AUTO_INCREMENT=9312 DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




### Properties

#### sp_code
The bacterial genome name brief code in KEGG database
