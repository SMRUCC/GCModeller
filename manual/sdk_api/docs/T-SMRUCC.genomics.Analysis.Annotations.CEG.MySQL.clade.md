---
title: clade
---

# clade
_namespace: [SMRUCC.genomics.Analysis.Annotations.CEG.MySQL](N-SMRUCC.genomics.Analysis.Annotations.CEG.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `clade`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `clade` (
 `oganismid` int(4) NOT NULL,
 `phylum` varchar(100) DEFAULT NULL,
 `abbr` varchar(100) DEFAULT NULL,
 `class` varchar(100) DEFAULT NULL,
 `order` varchar(100) NOT NULL,
 `family` varchar(100) DEFAULT NULL,
 `genus` text NOT NULL,
 PRIMARY KEY (`oganismid`)
 ) ENGINE=MyISAM AUTO_INCREMENT=7687 DEFAULT CHARSET=gb2312;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




