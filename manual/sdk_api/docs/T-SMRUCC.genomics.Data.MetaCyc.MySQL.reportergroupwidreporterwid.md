---
title: reportergroupwidreporterwid
---

# reportergroupwidreporterwid
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `reportergroupwidreporterwid`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `reportergroupwidreporterwid` (
 `ReporterGroupWID` bigint(20) NOT NULL,
 `ReporterWID` bigint(20) NOT NULL,
 KEY `FK_ReporterGroupWIDReporterW1` (`ReporterGroupWID`),
 KEY `FK_ReporterGroupWIDReporterW2` (`ReporterWID`),
 CONSTRAINT `FK_ReporterGroupWIDReporterW1` FOREIGN KEY (`ReporterGroupWID`) REFERENCES `designelementgroup` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_ReporterGroupWIDReporterW2` FOREIGN KEY (`ReporterWID`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




