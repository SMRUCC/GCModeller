# orthology
_namespace: [KEGG_tools.LocalMySQL](./index.md)_

--
 
 DROP TABLE IF EXISTS `orthology`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `orthology` (
 `entry` char(45) NOT NULL,
 `name` mediumtext,
 `definition` longtext,
 `pathways` int(11) DEFAULT NULL COMMENT 'Number of pathways that associated with this kegg orthology data',
 `modules` int(11) DEFAULT NULL,
 `genes` int(11) DEFAULT NULL,
 `disease` int(11) DEFAULT NULL,
 `brief_A` text,
 `brief_B` text,
 `brief_C` text,
 `brief_D` text,
 `brief_E` text,
 `EC` varchar(45) DEFAULT NULL,
 PRIMARY KEY (`entry`),
 UNIQUE KEY `entry_UNIQUE` (`entry`)
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




### Properties

#### pathways
Number of pathways that associated with this kegg orthology data
