---
title: protocolwidhardwarewid
---

# protocolwidhardwarewid
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `protocolwidhardwarewid`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `protocolwidhardwarewid` (
 `ProtocolWID` bigint(20) NOT NULL,
 `HardwareWID` bigint(20) NOT NULL,
 KEY `FK_ProtocolWIDHardwareWID1` (`ProtocolWID`),
 KEY `FK_ProtocolWIDHardwareWID2` (`HardwareWID`),
 CONSTRAINT `FK_ProtocolWIDHardwareWID1` FOREIGN KEY (`ProtocolWID`) REFERENCES `parameterizable` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_ProtocolWIDHardwareWID2` FOREIGN KEY (`HardwareWID`) REFERENCES `parameterizable` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




