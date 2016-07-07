---
title: bioevent
---

# bioevent
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `bioevent`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `bioevent` (
 `WID` bigint(20) NOT NULL,
 `DataSetWID` bigint(20) NOT NULL,
 `MAGEClass` varchar(100) NOT NULL,
 `Identifier` varchar(255) DEFAULT NULL,
 `Name` varchar(255) DEFAULT NULL,
 `CompositeSequence` bigint(20) DEFAULT NULL,
 `Reporter` bigint(20) DEFAULT NULL,
 `CompositeSequence2` bigint(20) DEFAULT NULL,
 `BioAssayMapTarget` bigint(20) DEFAULT NULL,
 `TargetQuantitationType` bigint(20) DEFAULT NULL,
 `DerivedBioAssayDataTarget` bigint(20) DEFAULT NULL,
 `QuantitationTypeMapping` bigint(20) DEFAULT NULL,
 `DesignElementMapping` bigint(20) DEFAULT NULL,
 `Transformation_BioAssayMapping` bigint(20) DEFAULT NULL,
 `BioMaterial_Treatments` bigint(20) DEFAULT NULL,
 `Order_` smallint(6) DEFAULT NULL,
 `Treatment_Action` bigint(20) DEFAULT NULL,
 `Treatment_ActionMeasurement` bigint(20) DEFAULT NULL,
 `Array_` bigint(20) DEFAULT NULL,
 `PhysicalBioAssayTarget` bigint(20) DEFAULT NULL,
 `PhysicalBioAssay` bigint(20) DEFAULT NULL,
 `Target` bigint(20) DEFAULT NULL,
 `PhysicalBioAssaySource` bigint(20) DEFAULT NULL,
 `MeasuredBioAssayTarget` bigint(20) DEFAULT NULL,
 `PhysicalBioAssay2` bigint(20) DEFAULT NULL,
 PRIMARY KEY (`WID`),
 KEY `FK_BioEvent1` (`DataSetWID`),
 KEY `FK_BioEvent3` (`CompositeSequence`),
 KEY `FK_BioEvent4` (`Reporter`),
 KEY `FK_BioEvent5` (`CompositeSequence2`),
 KEY `FK_BioEvent6` (`BioAssayMapTarget`),
 KEY `FK_BioEvent7` (`TargetQuantitationType`),
 KEY `FK_BioEvent8` (`DerivedBioAssayDataTarget`),
 KEY `FK_BioEvent9` (`QuantitationTypeMapping`),
 KEY `FK_BioEvent10` (`DesignElementMapping`),
 KEY `FK_BioEvent11` (`Transformation_BioAssayMapping`),
 KEY `FK_BioEvent12` (`BioMaterial_Treatments`),
 KEY `FK_BioEvent13` (`Treatment_Action`),
 KEY `FK_BioEvent14` (`Treatment_ActionMeasurement`),
 KEY `FK_BioEvent15` (`Array_`),
 KEY `FK_BioEvent16` (`PhysicalBioAssayTarget`),
 KEY `FK_BioEvent17` (`PhysicalBioAssay`),
 KEY `FK_BioEvent18` (`Target`),
 KEY `FK_BioEvent19` (`PhysicalBioAssaySource`),
 KEY `FK_BioEvent20` (`MeasuredBioAssayTarget`),
 KEY `FK_BioEvent21` (`PhysicalBioAssay2`),
 CONSTRAINT `FK_BioEvent1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_BioEvent10` FOREIGN KEY (`DesignElementMapping`) REFERENCES `designelementmapping` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_BioEvent11` FOREIGN KEY (`Transformation_BioAssayMapping`) REFERENCES `bioassaymapping` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_BioEvent12` FOREIGN KEY (`BioMaterial_Treatments`) REFERENCES `biosource` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_BioEvent13` FOREIGN KEY (`Treatment_Action`) REFERENCES `term` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_BioEvent14` FOREIGN KEY (`Treatment_ActionMeasurement`) REFERENCES `measurement` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_BioEvent15` FOREIGN KEY (`Array_`) REFERENCES `array_` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_BioEvent16` FOREIGN KEY (`PhysicalBioAssayTarget`) REFERENCES `bioassay` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_BioEvent17` FOREIGN KEY (`PhysicalBioAssay`) REFERENCES `bioassay` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_BioEvent18` FOREIGN KEY (`Target`) REFERENCES `bioassay` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_BioEvent19` FOREIGN KEY (`PhysicalBioAssaySource`) REFERENCES `bioassay` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_BioEvent20` FOREIGN KEY (`MeasuredBioAssayTarget`) REFERENCES `bioassay` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_BioEvent21` FOREIGN KEY (`PhysicalBioAssay2`) REFERENCES `bioassay` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_BioEvent3` FOREIGN KEY (`CompositeSequence`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_BioEvent4` FOREIGN KEY (`Reporter`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_BioEvent5` FOREIGN KEY (`CompositeSequence2`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_BioEvent6` FOREIGN KEY (`BioAssayMapTarget`) REFERENCES `bioassay` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_BioEvent7` FOREIGN KEY (`TargetQuantitationType`) REFERENCES `quantitationtype` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_BioEvent8` FOREIGN KEY (`DerivedBioAssayDataTarget`) REFERENCES `bioassaydata` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_BioEvent9` FOREIGN KEY (`QuantitationTypeMapping`) REFERENCES `quantitationtypemapping` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




