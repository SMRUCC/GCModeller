---
title: hardwarewidcontactwid
---

# hardwarewidcontactwid
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `hardwarewidcontactwid`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `hardwarewidcontactwid` (
 `HardwareWID` bigint(20) NOT NULL,
 `ContactWID` bigint(20) NOT NULL,
 KEY `FK_HardwareWIDContactWID1` (`HardwareWID`),
 KEY `FK_HardwareWIDContactWID2` (`ContactWID`),
 CONSTRAINT `FK_HardwareWIDContactWID1` FOREIGN KEY (`HardwareWID`) REFERENCES `parameterizable` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_HardwareWIDContactWID2` FOREIGN KEY (`ContactWID`) REFERENCES `contact` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




