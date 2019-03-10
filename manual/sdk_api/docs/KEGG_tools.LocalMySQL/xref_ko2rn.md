# xref_ko2rn
_namespace: [KEGG_tools.LocalMySQL](./index.md)_

kegg orthology corss reference to kegg reactions database.
 
 --
 
 DROP TABLE IF EXISTS `xref_ko2rn`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `xref_ko2rn` (
 `uid` int(11) NOT NULL AUTO_INCREMENT,
 `ko` varchar(45) NOT NULL,
 `rn` varchar(45) NOT NULL,
 `url` text,
 PRIMARY KEY (`ko`,`rn`),
 UNIQUE KEY `uid_UNIQUE` (`uid`)
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='kegg orthology corss reference to kegg reactions database.';
 /*!40101 SET character_set_client = @saved_cs_client */;
 /*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;
 
 /*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
 /*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
 /*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
 /*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
 /*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
 /*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
 /*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;
 
 -- Dump completed on 2015-12-03 18:35:08




