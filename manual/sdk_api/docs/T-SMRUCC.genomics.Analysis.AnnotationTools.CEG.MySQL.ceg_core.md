---
title: ceg_core
---

# ceg_core
_namespace: [SMRUCC.genomics.Analysis.AnnotationTools.CEG.MySQL](N-SMRUCC.genomics.Analysis.AnnotationTools.CEG.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `ceg_core`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `ceg_core` (
 `access_num` varchar(50) DEFAULT NULL,
 `gid` varchar(25) NOT NULL DEFAULT '',
 `koid` varchar(30) DEFAULT NULL,
 `cogid` varchar(255) NOT NULL,
 `hprd_nid` varchar(12) DEFAULT NULL,
 `nhit_ref` varchar(12) DEFAULT NULL,
 `nevalue` varchar(12) DEFAULT NULL,
 `nscore` varchar(20) DEFAULT NULL,
 `hprd_aid` varchar(20) NOT NULL,
 `ahit_ref` varchar(20) NOT NULL,
 `aevalue` varchar(20) NOT NULL,
 `ascore` varchar(20) NOT NULL,
 `degid` varchar(15) NOT NULL,
 `organismid` int(4) NOT NULL,
 PRIMARY KEY (`gid`),
 FULLTEXT KEY `gid` (`gid`,`access_num`)
 ) ENGINE=MyISAM AUTO_INCREMENT=7687 DEFAULT CHARSET=gb2312;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




