---
title: protocolwidsoftwarewid
---

# protocolwidsoftwarewid
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `protocolwidsoftwarewid`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `protocolwidsoftwarewid` (
 `ProtocolWID` bigint(20) NOT NULL,
 `SoftwareWID` bigint(20) NOT NULL,
 KEY `FK_ProtocolWIDSoftwareWID1` (`ProtocolWID`),
 KEY `FK_ProtocolWIDSoftwareWID2` (`SoftwareWID`),
 CONSTRAINT `FK_ProtocolWIDSoftwareWID1` FOREIGN KEY (`ProtocolWID`) REFERENCES `parameterizable` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_ProtocolWIDSoftwareWID2` FOREIGN KEY (`SoftwareWID`) REFERENCES `parameterizable` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




