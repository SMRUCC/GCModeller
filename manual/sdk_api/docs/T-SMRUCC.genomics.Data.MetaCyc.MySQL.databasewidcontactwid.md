---
title: databasewidcontactwid
---

# databasewidcontactwid
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `databasewidcontactwid`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `databasewidcontactwid` (
 `DatabaseWID` bigint(20) NOT NULL,
 `ContactWID` bigint(20) NOT NULL,
 KEY `FK_DatabaseWIDContactWID1` (`DatabaseWID`),
 KEY `FK_DatabaseWIDContactWID2` (`ContactWID`),
 CONSTRAINT `FK_DatabaseWIDContactWID1` FOREIGN KEY (`DatabaseWID`) REFERENCES `database_` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_DatabaseWIDContactWID2` FOREIGN KEY (`ContactWID`) REFERENCES `contact` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




