---
title: proteinwidspotwid
---

# proteinwidspotwid
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `proteinwidspotwid`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `proteinwidspotwid` (
 `ProteinWID` bigint(20) NOT NULL,
 `SpotWID` bigint(20) NOT NULL,
 KEY `FK_ProteinWIDSpotWID1` (`ProteinWID`),
 KEY `FK_ProteinWIDSpotWID2` (`SpotWID`),
 CONSTRAINT `FK_ProteinWIDSpotWID1` FOREIGN KEY (`ProteinWID`) REFERENCES `protein` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_ProteinWIDSpotWID2` FOREIGN KEY (`SpotWID`) REFERENCES `spot` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




