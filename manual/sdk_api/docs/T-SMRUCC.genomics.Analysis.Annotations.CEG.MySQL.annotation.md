---
title: annotation
---

# annotation
_namespace: [SMRUCC.genomics.Analysis.Annotations.CEG.MySQL](N-SMRUCC.genomics.Analysis.Annotations.CEG.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `annotation`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `annotation` (
 `gid` varchar(25) DEFAULT NULL,
 `Gene_Name` varchar(80) DEFAULT NULL,
 `fundescrp` varchar(255) DEFAULT NULL
 ) ENGINE=MyISAM AUTO_INCREMENT=11862 DEFAULT CHARSET=gb2312;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




