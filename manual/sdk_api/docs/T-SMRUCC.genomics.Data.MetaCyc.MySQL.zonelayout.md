---
title: zonelayout
---

# zonelayout
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `zonelayout`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `zonelayout` (
 `WID` bigint(20) NOT NULL,
 `DataSetWID` bigint(20) NOT NULL,
 `NumFeaturesPerRow` smallint(6) DEFAULT NULL,
 `NumFeaturesPerCol` smallint(6) DEFAULT NULL,
 `SpacingBetweenRows` float DEFAULT NULL,
 `SpacingBetweenCols` float DEFAULT NULL,
 `ZoneLayout_DistanceUnit` bigint(20) DEFAULT NULL,
 PRIMARY KEY (`WID`),
 KEY `FK_ZoneLayout1` (`DataSetWID`),
 KEY `FK_ZoneLayout2` (`ZoneLayout_DistanceUnit`),
 CONSTRAINT `FK_ZoneLayout1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_ZoneLayout2` FOREIGN KEY (`ZoneLayout_DistanceUnit`) REFERENCES `unit` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 /*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;
 
 /*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
 /*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
 /*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
 /*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
 /*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
 /*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
 /*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;
 
 -- Dump completed on 2015-12-03 20:02:01




