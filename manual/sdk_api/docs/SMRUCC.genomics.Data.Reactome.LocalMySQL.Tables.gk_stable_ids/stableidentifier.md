﻿# stableidentifier
_namespace: [SMRUCC.genomics.Data.Reactome.LocalMySQL.Tables.gk_stable_ids](./index.md)_

--
 
 DROP TABLE IF EXISTS `stableidentifier`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `stableidentifier` (
 `DB_ID` int(12) unsigned NOT NULL AUTO_INCREMENT,
 `identifier` varchar(32) DEFAULT NULL,
 `identifierVersion` int(4) DEFAULT NULL,
 `instanceId` int(12) DEFAULT NULL,
 PRIMARY KEY (`DB_ID`),
 KEY `identifier` (`identifier`(12))
 ) ENGINE=MyISAM AUTO_INCREMENT=1792857 DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 /*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;
 
 /*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
 /*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
 /*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
 /*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
 /*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
 /*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
 /*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;
 
 -- Dump completed on 2015-10-08 21:48:17




