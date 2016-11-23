# module
_namespace: [KEGG_tools.LocalMySQL](./index.md)_

--
 
 DROP TABLE IF EXISTS `module`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `module` (
 `entry` varchar(45) NOT NULL,
 `name` longtext,
 `definition` longtext,
 `class` text,
 `category` text,
 `type` text,
 PRIMARY KEY (`entry`),
 UNIQUE KEY `entry_UNIQUE` (`entry`)
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




