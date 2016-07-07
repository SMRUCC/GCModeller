---
title: quanttypewidquanttypemapwid
---

# quanttypewidquanttypemapwid
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `quanttypewidquanttypemapwid`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `quanttypewidquanttypemapwid` (
 `QuantitationTypeWID` bigint(20) NOT NULL,
 `QuantitationTypeMapWID` bigint(20) NOT NULL,
 KEY `FK_QuantTypeWIDQuantTypeMapW1` (`QuantitationTypeWID`),
 KEY `FK_QuantTypeWIDQuantTypeMapW2` (`QuantitationTypeMapWID`),
 CONSTRAINT `FK_QuantTypeWIDQuantTypeMapW1` FOREIGN KEY (`QuantitationTypeWID`) REFERENCES `quantitationtype` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_QuantTypeWIDQuantTypeMapW2` FOREIGN KEY (`QuantitationTypeMapWID`) REFERENCES `bioevent` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




