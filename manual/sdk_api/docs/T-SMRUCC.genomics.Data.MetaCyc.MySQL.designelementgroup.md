---
title: designelementgroup
---

# designelementgroup
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `designelementgroup`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `designelementgroup` (
 `WID` bigint(20) NOT NULL,
 `DataSetWID` bigint(20) NOT NULL,
 `MAGEClass` varchar(100) NOT NULL,
 `Identifier` varchar(255) DEFAULT NULL,
 `Name` varchar(255) DEFAULT NULL,
 `ArrayDesign_FeatureGroups` bigint(20) DEFAULT NULL,
 `DesignElementGroup_Species` bigint(20) DEFAULT NULL,
 `FeatureWidth` float DEFAULT NULL,
 `FeatureLength` float DEFAULT NULL,
 `FeatureHeight` float DEFAULT NULL,
 `FeatureGroup_TechnologyType` bigint(20) DEFAULT NULL,
 `FeatureGroup_FeatureShape` bigint(20) DEFAULT NULL,
 `FeatureGroup_DistanceUnit` bigint(20) DEFAULT NULL,
 PRIMARY KEY (`WID`),
 KEY `FK_DesignElementGroup1` (`DataSetWID`),
 KEY `FK_DesignElementGroup3` (`ArrayDesign_FeatureGroups`),
 KEY `FK_DesignElementGroup4` (`DesignElementGroup_Species`),
 KEY `FK_DesignElementGroup5` (`FeatureGroup_TechnologyType`),
 KEY `FK_DesignElementGroup6` (`FeatureGroup_FeatureShape`),
 KEY `FK_DesignElementGroup7` (`FeatureGroup_DistanceUnit`),
 CONSTRAINT `FK_DesignElementGroup1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_DesignElementGroup3` FOREIGN KEY (`ArrayDesign_FeatureGroups`) REFERENCES `arraydesign` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_DesignElementGroup4` FOREIGN KEY (`DesignElementGroup_Species`) REFERENCES `term` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_DesignElementGroup5` FOREIGN KEY (`FeatureGroup_TechnologyType`) REFERENCES `term` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_DesignElementGroup6` FOREIGN KEY (`FeatureGroup_FeatureShape`) REFERENCES `term` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_DesignElementGroup7` FOREIGN KEY (`FeatureGroup_DistanceUnit`) REFERENCES `unit` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




