---
title: arraydesignwidreportergroupwid
---

# arraydesignwidreportergroupwid
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `arraydesignwidreportergroupwid`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `arraydesignwidreportergroupwid` (
 `ArrayDesignWID` bigint(20) NOT NULL,
 `ReporterGroupWID` bigint(20) NOT NULL,
 KEY `FK_ArrayDesignWIDReporterGro1` (`ArrayDesignWID`),
 KEY `FK_ArrayDesignWIDReporterGro2` (`ReporterGroupWID`),
 CONSTRAINT `FK_ArrayDesignWIDReporterGro1` FOREIGN KEY (`ArrayDesignWID`) REFERENCES `arraydesign` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_ArrayDesignWIDReporterGro2` FOREIGN KEY (`ReporterGroupWID`) REFERENCES `designelementgroup` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




