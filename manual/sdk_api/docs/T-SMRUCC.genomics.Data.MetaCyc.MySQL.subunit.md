---
title: subunit
---

# subunit
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `subunit`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `subunit` (
 `ComplexWID` bigint(20) NOT NULL,
 `SubunitWID` bigint(20) NOT NULL,
 `Coefficient` smallint(6) DEFAULT NULL,
 KEY `FK_Subunit1` (`ComplexWID`),
 KEY `FK_Subunit2` (`SubunitWID`),
 CONSTRAINT `FK_Subunit1` FOREIGN KEY (`ComplexWID`) REFERENCES `protein` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_Subunit2` FOREIGN KEY (`SubunitWID`) REFERENCES `protein` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




