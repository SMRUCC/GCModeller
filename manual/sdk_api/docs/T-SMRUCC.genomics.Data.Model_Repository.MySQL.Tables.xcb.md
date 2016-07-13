---
title: xcb
---

# xcb
_namespace: [SMRUCC.genomics.Data.Model_Repository.MySQL.Tables](N-SMRUCC.genomics.Data.Model_Repository.MySQL.Tables.html)_

--
 
 DROP TABLE IF EXISTS `xcb`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `xcb` (
 `uid` int(11) NOT NULL AUTO_INCREMENT,
 `g1_entity` varchar(45) NOT NULL,
 `g2_entity` varchar(45) NOT NULL,
 `pcc` double DEFAULT '0',
 `spcc` double DEFAULT '0',
 `wgcna_weight` double DEFAULT '0',
 PRIMARY KEY (`g1_entity`,`g2_entity`),
 UNIQUE KEY `uid_UNIQUE` (`uid`)
 ) ENGINE=InnoDB AUTO_INCREMENT=18275626 DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 /*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;
 
 /*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
 /*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
 /*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
 /*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
 /*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
 /*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
 /*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;
 
 -- Dump completed on 2015-12-03 21:01:55




