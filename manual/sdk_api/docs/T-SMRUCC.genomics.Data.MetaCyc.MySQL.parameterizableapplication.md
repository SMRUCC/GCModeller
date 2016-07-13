---
title: parameterizableapplication
---

# parameterizableapplication
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `parameterizableapplication`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `parameterizableapplication` (
 `WID` bigint(20) NOT NULL,
 `DataSetWID` bigint(20) NOT NULL,
 `MAGEClass` varchar(100) NOT NULL,
 `ArrayDesign` bigint(20) DEFAULT NULL,
 `ArrayManufacture` bigint(20) DEFAULT NULL,
 `BioEvent_ProtocolApplications` bigint(20) DEFAULT NULL,
 `SerialNumber` varchar(255) DEFAULT NULL,
 `Hardware` bigint(20) DEFAULT NULL,
 `ActivityDate` varchar(255) DEFAULT NULL,
 `ProtocolApplication` bigint(20) DEFAULT NULL,
 `ProtocolApplication2` bigint(20) DEFAULT NULL,
 `Protocol` bigint(20) DEFAULT NULL,
 `Version` varchar(255) DEFAULT NULL,
 `ReleaseDate` datetime DEFAULT NULL,
 `Software` bigint(20) DEFAULT NULL,
 PRIMARY KEY (`WID`),
 KEY `FK_ParameterizableApplicatio1` (`DataSetWID`),
 KEY `FK_ParameterizableApplicatio3` (`ArrayDesign`),
 KEY `FK_ParameterizableApplicatio4` (`ArrayManufacture`),
 KEY `FK_ParameterizableApplicatio5` (`BioEvent_ProtocolApplications`),
 KEY `FK_ParameterizableApplicatio6` (`Hardware`),
 KEY `FK_ParameterizableApplicatio7` (`ProtocolApplication`),
 KEY `FK_ParameterizableApplicatio8` (`ProtocolApplication2`),
 KEY `FK_ParameterizableApplicatio9` (`Protocol`),
 KEY `FK_ParameterizableApplicatio10` (`Software`),
 CONSTRAINT `FK_ParameterizableApplicatio1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_ParameterizableApplicatio10` FOREIGN KEY (`Software`) REFERENCES `parameterizable` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_ParameterizableApplicatio3` FOREIGN KEY (`ArrayDesign`) REFERENCES `arraydesign` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_ParameterizableApplicatio4` FOREIGN KEY (`ArrayManufacture`) REFERENCES `arraymanufacture` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_ParameterizableApplicatio5` FOREIGN KEY (`BioEvent_ProtocolApplications`) REFERENCES `bioevent` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_ParameterizableApplicatio6` FOREIGN KEY (`Hardware`) REFERENCES `parameterizable` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_ParameterizableApplicatio7` FOREIGN KEY (`ProtocolApplication`) REFERENCES `parameterizableapplication` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_ParameterizableApplicatio8` FOREIGN KEY (`ProtocolApplication2`) REFERENCES `parameterizableapplication` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_ParameterizableApplicatio9` FOREIGN KEY (`Protocol`) REFERENCES `parameterizable` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




