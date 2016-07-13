---
title: sequenceposition
---

# sequenceposition
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `sequenceposition`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `sequenceposition` (
 `WID` bigint(20) NOT NULL,
 `DataSetWID` bigint(20) NOT NULL,
 `MAGEClass` varchar(100) NOT NULL,
 `Start_` smallint(6) DEFAULT NULL,
 `End` smallint(6) DEFAULT NULL,
 `CompositeCompositeMap` bigint(20) DEFAULT NULL,
 `Composite` bigint(20) DEFAULT NULL,
 `ReporterCompositeMap` bigint(20) DEFAULT NULL,
 `Reporter` bigint(20) DEFAULT NULL,
 PRIMARY KEY (`WID`),
 KEY `FK_SequencePosition1` (`DataSetWID`),
 KEY `FK_SequencePosition2` (`CompositeCompositeMap`),
 KEY `FK_SequencePosition3` (`Composite`),
 KEY `FK_SequencePosition4` (`ReporterCompositeMap`),
 KEY `FK_SequencePosition5` (`Reporter`),
 CONSTRAINT `FK_SequencePosition1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_SequencePosition2` FOREIGN KEY (`CompositeCompositeMap`) REFERENCES `bioevent` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_SequencePosition3` FOREIGN KEY (`Composite`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_SequencePosition4` FOREIGN KEY (`ReporterCompositeMap`) REFERENCES `bioevent` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_SequencePosition5` FOREIGN KEY (`Reporter`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




