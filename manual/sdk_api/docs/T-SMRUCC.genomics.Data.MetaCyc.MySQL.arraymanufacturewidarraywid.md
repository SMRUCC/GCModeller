---
title: arraymanufacturewidarraywid
---

# arraymanufacturewidarraywid
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `arraymanufacturewidarraywid`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `arraymanufacturewidarraywid` (
 `ArrayManufactureWID` bigint(20) NOT NULL,
 `ArrayWID` bigint(20) NOT NULL,
 KEY `FK_ArrayManufactureWIDArrayW1` (`ArrayManufactureWID`),
 KEY `FK_ArrayManufactureWIDArrayW2` (`ArrayWID`),
 CONSTRAINT `FK_ArrayManufactureWIDArrayW1` FOREIGN KEY (`ArrayManufactureWID`) REFERENCES `arraymanufacture` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_ArrayManufactureWIDArrayW2` FOREIGN KEY (`ArrayWID`) REFERENCES `array_` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




