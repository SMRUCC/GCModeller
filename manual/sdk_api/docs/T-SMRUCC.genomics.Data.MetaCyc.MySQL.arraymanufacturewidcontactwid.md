---
title: arraymanufacturewidcontactwid
---

# arraymanufacturewidcontactwid
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `arraymanufacturewidcontactwid`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `arraymanufacturewidcontactwid` (
 `ArrayManufactureWID` bigint(20) NOT NULL,
 `ContactWID` bigint(20) NOT NULL,
 KEY `FK_ArrayManufactureWIDContac1` (`ArrayManufactureWID`),
 KEY `FK_ArrayManufactureWIDContac2` (`ContactWID`),
 CONSTRAINT `FK_ArrayManufactureWIDContac1` FOREIGN KEY (`ArrayManufactureWID`) REFERENCES `arraymanufacture` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_ArrayManufactureWIDContac2` FOREIGN KEY (`ContactWID`) REFERENCES `contact` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




