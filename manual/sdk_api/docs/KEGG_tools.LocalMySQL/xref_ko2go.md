# xref_ko2go
_namespace: [KEGG_tools.LocalMySQL](./index.md)_

kegg orthology cross reference to go database
 
 --
 
 DROP TABLE IF EXISTS `xref_ko2go`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `xref_ko2go` (
 `uid` int(11) NOT NULL AUTO_INCREMENT,
 `ko` varchar(45) NOT NULL,
 `go` varchar(45) NOT NULL,
 `url` text,
 PRIMARY KEY (`ko`,`go`),
 UNIQUE KEY `uid_UNIQUE` (`uid`)
 ) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8 COMMENT='kegg orthology cross reference to go database';
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




