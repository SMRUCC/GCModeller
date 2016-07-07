---
title: quanttymapwidquanttymapwi
---

# quanttymapwidquanttymapwi
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `quanttymapwidquanttymapwi`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `quanttymapwidquanttymapwi` (
 `QuantitationTypeMappingWID` bigint(20) NOT NULL,
 `QuantitationTypeMapWID` bigint(20) NOT NULL,
 KEY `FK_QuantTyMapWIDQuantTyMapWI1` (`QuantitationTypeMappingWID`),
 KEY `FK_QuantTyMapWIDQuantTyMapWI2` (`QuantitationTypeMapWID`),
 CONSTRAINT `FK_QuantTyMapWIDQuantTyMapWI1` FOREIGN KEY (`QuantitationTypeMappingWID`) REFERENCES `quantitationtypemapping` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_QuantTyMapWIDQuantTyMapWI2` FOREIGN KEY (`QuantitationTypeMapWID`) REFERENCES `bioevent` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




