﻿# aa_seq
_namespace: [SMRUCC.genomics.Analysis.Annotations.CEG.MySQL](./index.md)_

--
 
 DROP TABLE IF EXISTS `aa_seq`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `aa_seq` (
 `gid` varchar(25) DEFAULT NULL,
 `aalength` varchar(8) DEFAULT NULL,
 `aaseq` longtext
 ) ENGINE=MyISAM AUTO_INCREMENT=7687 DEFAULT CHARSET=gb2312;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




