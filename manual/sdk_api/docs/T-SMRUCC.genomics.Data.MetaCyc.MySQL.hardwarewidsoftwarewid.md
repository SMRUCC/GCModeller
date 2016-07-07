---
title: hardwarewidsoftwarewid
---

# hardwarewidsoftwarewid
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `hardwarewidsoftwarewid`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `hardwarewidsoftwarewid` (
 `HardwareWID` bigint(20) NOT NULL,
 `SoftwareWID` bigint(20) NOT NULL,
 KEY `FK_HardwareWIDSoftwareWID1` (`HardwareWID`),
 KEY `FK_HardwareWIDSoftwareWID2` (`SoftwareWID`),
 CONSTRAINT `FK_HardwareWIDSoftwareWID1` FOREIGN KEY (`HardwareWID`) REFERENCES `parameterizable` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_HardwareWIDSoftwareWID2` FOREIGN KEY (`SoftwareWID`) REFERENCES `parameterizable` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




