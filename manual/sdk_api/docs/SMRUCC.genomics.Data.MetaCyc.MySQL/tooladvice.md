﻿# tooladvice
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](./index.md)_

--
 
 DROP TABLE IF EXISTS `tooladvice`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `tooladvice` (
 `OtherWID` bigint(20) NOT NULL,
 `ToolName` varchar(50) NOT NULL,
 `Advice` longtext
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




