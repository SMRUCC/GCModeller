---
title: reporterwidfeaturerepormapwid
---

# reporterwidfeaturerepormapwid
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `reporterwidfeaturerepormapwid`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `reporterwidfeaturerepormapwid` (
 `ReporterWID` bigint(20) NOT NULL,
 `FeatureReporterMapWID` bigint(20) NOT NULL,
 KEY `FK_ReporterWIDFeatureReporMa1` (`ReporterWID`),
 KEY `FK_ReporterWIDFeatureReporMa2` (`FeatureReporterMapWID`),
 CONSTRAINT `FK_ReporterWIDFeatureReporMa1` FOREIGN KEY (`ReporterWID`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_ReporterWIDFeatureReporMa2` FOREIGN KEY (`FeatureReporterMapWID`) REFERENCES `bioevent` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




