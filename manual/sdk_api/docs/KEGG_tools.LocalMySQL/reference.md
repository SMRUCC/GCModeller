# reference
_namespace: [KEGG_tools.LocalMySQL](./index.md)_

--
 
 DROP TABLE IF EXISTS `reference`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `reference` (
 `authors` longtext,
 `title` longtext,
 `journal` longtext,
 `pmid` bigint(20) NOT NULL,
 PRIMARY KEY (`pmid`),
 UNIQUE KEY `pmid_UNIQUE` (`pmid`)
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




