---
title: protocolapplwidpersonwid
---

# protocolapplwidpersonwid
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `protocolapplwidpersonwid`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `protocolapplwidpersonwid` (
 `ProtocolApplicationWID` bigint(20) NOT NULL,
 `PersonWID` bigint(20) NOT NULL,
 KEY `FK_ProtocolApplWIDPersonWID1` (`ProtocolApplicationWID`),
 KEY `FK_ProtocolApplWIDPersonWID2` (`PersonWID`),
 CONSTRAINT `FK_ProtocolApplWIDPersonWID1` FOREIGN KEY (`ProtocolApplicationWID`) REFERENCES `parameterizableapplication` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_ProtocolApplWIDPersonWID2` FOREIGN KEY (`PersonWID`) REFERENCES `contact` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




