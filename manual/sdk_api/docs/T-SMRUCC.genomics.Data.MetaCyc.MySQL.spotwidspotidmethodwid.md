---
title: spotwidspotidmethodwid
---

# spotwidspotidmethodwid
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `spotwidspotidmethodwid`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `spotwidspotidmethodwid` (
 `SpotWID` bigint(20) NOT NULL,
 `SpotIdMethodWID` bigint(20) NOT NULL,
 KEY `FK_SpotWIDMethWID1` (`SpotWID`),
 KEY `FK_SpotWIDMethWID2` (`SpotIdMethodWID`),
 CONSTRAINT `FK_SpotWIDMethWID1` FOREIGN KEY (`SpotWID`) REFERENCES `spot` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_SpotWIDMethWID2` FOREIGN KEY (`SpotIdMethodWID`) REFERENCES `spotidmethod` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




