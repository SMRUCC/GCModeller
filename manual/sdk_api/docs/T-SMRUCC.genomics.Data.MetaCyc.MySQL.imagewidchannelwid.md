---
title: imagewidchannelwid
---

# imagewidchannelwid
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `imagewidchannelwid`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `imagewidchannelwid` (
 `ImageWID` bigint(20) NOT NULL,
 `ChannelWID` bigint(20) NOT NULL,
 KEY `FK_ImageWIDChannelWID1` (`ImageWID`),
 KEY `FK_ImageWIDChannelWID2` (`ChannelWID`),
 CONSTRAINT `FK_ImageWIDChannelWID1` FOREIGN KEY (`ImageWID`) REFERENCES `image` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_ImageWIDChannelWID2` FOREIGN KEY (`ChannelWID`) REFERENCES `channel` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




