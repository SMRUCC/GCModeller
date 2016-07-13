---
title: quanttypemapwidquanttypewid
---

# quanttypemapwidquanttypewid
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `quanttypemapwidquanttypewid`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `quanttypemapwidquanttypewid` (
 `QuantitationTypeMapWID` bigint(20) NOT NULL,
 `QuantitationTypeWID` bigint(20) NOT NULL,
 KEY `FK_QuantTypeMapWIDQuantTypeW1` (`QuantitationTypeMapWID`),
 KEY `FK_QuantTypeMapWIDQuantTypeW2` (`QuantitationTypeWID`),
 CONSTRAINT `FK_QuantTypeMapWIDQuantTypeW1` FOREIGN KEY (`QuantitationTypeMapWID`) REFERENCES `bioevent` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_QuantTypeMapWIDQuantTypeW2` FOREIGN KEY (`QuantitationTypeWID`) REFERENCES `quantitationtype` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




