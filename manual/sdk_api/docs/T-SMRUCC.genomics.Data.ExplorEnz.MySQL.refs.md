---
title: refs
---

# refs
_namespace: [SMRUCC.genomics.Data.ExplorEnz.MySQL](N-SMRUCC.genomics.Data.ExplorEnz.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `refs`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `refs` (
 `cite_key` varchar(48) NOT NULL DEFAULT '',
 `type` varchar(7) DEFAULT NULL,
 `author` text,
 `title` text,
 `journal` varchar(72) DEFAULT NULL,
 `volume` varchar(20) DEFAULT NULL,
 `year` int(11) DEFAULT NULL,
 `first_page` varchar(12) DEFAULT NULL,
 `last_page` varchar(11) DEFAULT NULL,
 `pubmed_id` varchar(8) DEFAULT NULL,
 `language` varchar(127) DEFAULT NULL,
 `booktitle` varchar(255) DEFAULT NULL,
 `editor` varchar(128) DEFAULT NULL,
 `edition` char(3) DEFAULT NULL,
 `publisher` varchar(65) DEFAULT NULL,
 `address` varchar(65) DEFAULT NULL,
 `erratum` text,
 `entry_title` text,
 `patent_yr` int(11) DEFAULT NULL,
 `link` char(1) DEFAULT NULL,
 `last_change` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
 PRIMARY KEY (`cite_key`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 /*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;
 
 /*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
 /*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
 /*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
 /*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
 /*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
 /*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
 /*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;
 
 -- Dump completed on 2015-12-03 19:58:29




