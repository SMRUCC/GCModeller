# xref_ko2cog
_namespace: [KEGG_tools.LocalMySQL](./index.md)_

KEGG orthology database cross reference to COG database.
 
 --
 
 DROP TABLE IF EXISTS `xref_ko2cog`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `xref_ko2cog` (
 `uid` int(11) NOT NULL AUTO_INCREMENT,
 `ko` varchar(45) NOT NULL,
 `COG` varchar(45) NOT NULL,
 `url` text,
 PRIMARY KEY (`ko`,`COG`),
 UNIQUE KEY `uid_UNIQUE` (`uid`)
 ) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8 COMMENT='KEGG orthology database cross reference to COG database.';
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




