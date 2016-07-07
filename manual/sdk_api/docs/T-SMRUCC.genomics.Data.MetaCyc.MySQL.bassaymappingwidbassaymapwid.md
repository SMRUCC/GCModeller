---
title: bassaymappingwidbassaymapwid
---

# bassaymappingwidbassaymapwid
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `bassaymappingwidbassaymapwid`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `bassaymappingwidbassaymapwid` (
 `BioAssayMappingWID` bigint(20) NOT NULL,
 `BioAssayMapWID` bigint(20) NOT NULL,
 KEY `FK_BAssayMappingWIDBAssayMap1` (`BioAssayMappingWID`),
 KEY `FK_BAssayMappingWIDBAssayMap2` (`BioAssayMapWID`),
 CONSTRAINT `FK_BAssayMappingWIDBAssayMap1` FOREIGN KEY (`BioAssayMappingWID`) REFERENCES `bioassaymapping` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_BAssayMappingWIDBAssayMap2` FOREIGN KEY (`BioAssayMapWID`) REFERENCES `bioevent` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




