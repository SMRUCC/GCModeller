﻿# enumeration
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](./index.md)_

--
 
 DROP TABLE IF EXISTS `enumeration`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `enumeration` (
 `TableName` varchar(50) NOT NULL,
 `ColumnName` varchar(50) NOT NULL,
 `Value` varchar(50) NOT NULL,
 `Meaning` text
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




