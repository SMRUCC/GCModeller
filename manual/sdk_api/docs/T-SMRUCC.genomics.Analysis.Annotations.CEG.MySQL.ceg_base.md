---
title: ceg_base
---

# ceg_base
_namespace: [SMRUCC.genomics.Analysis.Annotations.CEG.MySQL](N-SMRUCC.genomics.Analysis.Annotations.CEG.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `ceg_base`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `ceg_base` (
 `access_num` varchar(255) DEFAULT NULL,
 `koid` varchar(255) DEFAULT NULL,
 `cogid` varchar(255) NOT NULL,
 `description` varchar(255) NOT NULL,
 `ec` varchar(100) NOT NULL
 ) ENGINE=MyISAM DEFAULT CHARSET=gb2312;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




