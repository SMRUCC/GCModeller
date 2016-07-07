---
title: contact
---

# contact
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `contact`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `contact` (
 `WID` bigint(20) NOT NULL,
 `DataSetWID` bigint(20) NOT NULL,
 `MAGEClass` varchar(100) NOT NULL,
 `Identifier` varchar(255) DEFAULT NULL,
 `Name` varchar(255) DEFAULT NULL,
 `URI` varchar(255) DEFAULT NULL,
 `Address` varchar(255) DEFAULT NULL,
 `Phone` varchar(255) DEFAULT NULL,
 `TollFreePhone` varchar(255) DEFAULT NULL,
 `Email` varchar(255) DEFAULT NULL,
 `Fax` varchar(255) DEFAULT NULL,
 `Parent` bigint(20) DEFAULT NULL,
 `LastName` varchar(255) DEFAULT NULL,
 `FirstName` varchar(255) DEFAULT NULL,
 `MidInitials` varchar(255) DEFAULT NULL,
 `Affiliation` bigint(20) DEFAULT NULL,
 PRIMARY KEY (`WID`),
 KEY `FK_Contact1` (`DataSetWID`),
 KEY `FK_Contact3` (`Parent`),
 KEY `FK_Contact4` (`Affiliation`),
 CONSTRAINT `FK_Contact1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_Contact3` FOREIGN KEY (`Parent`) REFERENCES `contact` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_Contact4` FOREIGN KEY (`Affiliation`) REFERENCES `contact` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




