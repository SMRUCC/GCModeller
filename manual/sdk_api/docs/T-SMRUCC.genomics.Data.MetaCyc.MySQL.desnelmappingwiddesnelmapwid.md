---
title: desnelmappingwiddesnelmapwid
---

# desnelmappingwiddesnelmapwid
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `desnelmappingwiddesnelmapwid`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `desnelmappingwiddesnelmapwid` (
 `DesignElementMappingWID` bigint(20) NOT NULL,
 `DesignElementMapWID` bigint(20) NOT NULL,
 KEY `FK_DesnElMappingWIDDesnElMap1` (`DesignElementMappingWID`),
 KEY `FK_DesnElMappingWIDDesnElMap2` (`DesignElementMapWID`),
 CONSTRAINT `FK_DesnElMappingWIDDesnElMap1` FOREIGN KEY (`DesignElementMappingWID`) REFERENCES `designelementmapping` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_DesnElMappingWIDDesnElMap2` FOREIGN KEY (`DesignElementMapWID`) REFERENCES `bioevent` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




