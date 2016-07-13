---
title: experiment
---

# experiment
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `experiment`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `experiment` (
 `WID` bigint(20) NOT NULL,
 `Type` varchar(50) NOT NULL,
 `ContactWID` bigint(20) DEFAULT NULL,
 `ArchiveWID` bigint(20) DEFAULT NULL,
 `StartDate` datetime DEFAULT NULL,
 `EndDate` datetime DEFAULT NULL,
 `Description` text,
 `GroupWID` bigint(20) DEFAULT NULL,
 `GroupType` varchar(50) DEFAULT NULL,
 `GroupSize` int(11) NOT NULL,
 `GroupIndex` int(11) DEFAULT NULL,
 `TimePoint` int(11) DEFAULT NULL,
 `TimeUnit` varchar(20) DEFAULT NULL,
 `DataSetWID` bigint(20) NOT NULL,
 `BioSourceWID` bigint(20) DEFAULT NULL,
 PRIMARY KEY (`WID`),
 KEY `FK_Experiment3` (`ContactWID`),
 KEY `FK_Experiment4` (`ArchiveWID`),
 KEY `FK_Experiment2` (`GroupWID`),
 KEY `FK_Experiment5` (`DataSetWID`),
 KEY `FK_Experiment6` (`BioSourceWID`),
 CONSTRAINT `FK_Experiment2` FOREIGN KEY (`GroupWID`) REFERENCES `experiment` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_Experiment3` FOREIGN KEY (`ContactWID`) REFERENCES `contact` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_Experiment4` FOREIGN KEY (`ArchiveWID`) REFERENCES `archive` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_Experiment5` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_Experiment6` FOREIGN KEY (`BioSourceWID`) REFERENCES `biosource` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




