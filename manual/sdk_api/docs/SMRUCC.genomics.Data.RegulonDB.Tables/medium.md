# medium
_namespace: [SMRUCC.genomics.Data.RegulonDB.Tables](./index.md)_

--
 
 DROP TABLE IF EXISTS `medium`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `medium` (
 `medium_id` char(12) NOT NULL,
 `medium_name` varchar(300) NOT NULL,
 `medium_description` varchar(2000) NOT NULL
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




