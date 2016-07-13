---
title: reporterdimenswidreporterwid
---

# reporterdimenswidreporterwid
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `reporterdimenswidreporterwid`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `reporterdimenswidreporterwid` (
 `ReporterDimensionWID` bigint(20) NOT NULL,
 `ReporterWID` bigint(20) NOT NULL,
 KEY `FK_ReporterDimensWIDReporter1` (`ReporterDimensionWID`),
 KEY `FK_ReporterDimensWIDReporter2` (`ReporterWID`),
 CONSTRAINT `FK_ReporterDimensWIDReporter1` FOREIGN KEY (`ReporterDimensionWID`) REFERENCES `designelementdimension` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_ReporterDimensWIDReporter2` FOREIGN KEY (`ReporterWID`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




