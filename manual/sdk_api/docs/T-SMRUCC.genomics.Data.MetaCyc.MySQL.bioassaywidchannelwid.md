---
title: bioassaywidchannelwid
---

# bioassaywidchannelwid
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `bioassaywidchannelwid`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `bioassaywidchannelwid` (
 `BioAssayWID` bigint(20) NOT NULL,
 `ChannelWID` bigint(20) NOT NULL,
 KEY `FK_BioAssayWIDChannelWID1` (`BioAssayWID`),
 KEY `FK_BioAssayWIDChannelWID2` (`ChannelWID`),
 CONSTRAINT `FK_BioAssayWIDChannelWID1` FOREIGN KEY (`BioAssayWID`) REFERENCES `bioassay` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_BioAssayWIDChannelWID2` FOREIGN KEY (`ChannelWID`) REFERENCES `channel` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




