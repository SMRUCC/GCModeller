---
title: reference
---

# reference
_namespace: [SMRUCC.genomics.Analysis.Annotations.CEG.MySQL](N-SMRUCC.genomics.Analysis.Annotations.CEG.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `reference`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `reference` (
 `oganismid` int(4) DEFAULT NULL,
 `abbr` varchar(5) NOT NULL,
 `oganism` varchar(255) DEFAULT NULL,
 `pubmedid` varchar(20) DEFAULT NULL,
 `pub_title` text NOT NULL
 ) ENGINE=MyISAM AUTO_INCREMENT=7687 DEFAULT CHARSET=gb2312;
 /*!40101 SET character_set_client = @saved_cs_client */;
 /*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;
 
 /*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
 /*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
 /*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
 /*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
 /*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
 /*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
 /*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;
 
 -- Dump completed on 2015-10-09 2:15:39




