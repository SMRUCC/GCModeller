---
title: manufacturelims
---

# manufacturelims
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `manufacturelims`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `manufacturelims` (
 `WID` bigint(20) NOT NULL,
 `DataSetWID` bigint(20) NOT NULL,
 `MAGEClass` varchar(100) NOT NULL,
 `ArrayManufacture_FeatureLIMSs` bigint(20) DEFAULT NULL,
 `Quality` varchar(255) DEFAULT NULL,
 `Feature` bigint(20) DEFAULT NULL,
 `BioMaterial` bigint(20) DEFAULT NULL,
 `ManufactureLIMS_IdentifierLIMS` bigint(20) DEFAULT NULL,
 `BioMaterialPlateIdentifier` varchar(255) DEFAULT NULL,
 `BioMaterialPlateRow` varchar(255) DEFAULT NULL,
 `BioMaterialPlateCol` varchar(255) DEFAULT NULL,
 PRIMARY KEY (`WID`),
 KEY `FK_ManufactureLIMS1` (`DataSetWID`),
 KEY `FK_ManufactureLIMS3` (`ArrayManufacture_FeatureLIMSs`),
 KEY `FK_ManufactureLIMS4` (`Feature`),
 KEY `FK_ManufactureLIMS5` (`BioMaterial`),
 CONSTRAINT `FK_ManufactureLIMS1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_ManufactureLIMS3` FOREIGN KEY (`ArrayManufacture_FeatureLIMSs`) REFERENCES `arraymanufacture` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_ManufactureLIMS4` FOREIGN KEY (`Feature`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_ManufactureLIMS5` FOREIGN KEY (`BioMaterial`) REFERENCES `biosource` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




