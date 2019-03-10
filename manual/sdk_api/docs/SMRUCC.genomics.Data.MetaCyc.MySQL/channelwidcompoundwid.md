﻿# channelwidcompoundwid
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](./index.md)_

--
 
 DROP TABLE IF EXISTS `channelwidcompoundwid`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `channelwidcompoundwid` (
 `ChannelWID` bigint(20) NOT NULL,
 `CompoundWID` bigint(20) NOT NULL,
 KEY `FK_ChannelWIDCompoundWID1` (`ChannelWID`),
 KEY `FK_ChannelWIDCompoundWID2` (`CompoundWID`),
 CONSTRAINT `FK_ChannelWIDCompoundWID1` FOREIGN KEY (`ChannelWID`) REFERENCES `channel` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_ChannelWIDCompoundWID2` FOREIGN KEY (`CompoundWID`) REFERENCES `chemical` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




