---
title: softwarewidsoftwarewid
---

# softwarewidsoftwarewid
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `softwarewidsoftwarewid`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `softwarewidsoftwarewid` (
 `SoftwareWID1` bigint(20) NOT NULL,
 `SoftwareWID2` bigint(20) NOT NULL,
 KEY `FK_SoftwareWIDSoftwareWID1` (`SoftwareWID1`),
 KEY `FK_SoftwareWIDSoftwareWID2` (`SoftwareWID2`),
 CONSTRAINT `FK_SoftwareWIDSoftwareWID1` FOREIGN KEY (`SoftwareWID1`) REFERENCES `parameterizable` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_SoftwareWIDSoftwareWID2` FOREIGN KEY (`SoftwareWID2`) REFERENCES `parameterizable` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




