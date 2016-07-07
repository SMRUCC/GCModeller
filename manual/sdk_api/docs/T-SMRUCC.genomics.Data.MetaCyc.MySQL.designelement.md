---
title: designelement
---

# designelement
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `designelement`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `designelement` (
 `WID` bigint(20) NOT NULL,
 `DataSetWID` bigint(20) NOT NULL,
 `MAGEClass` varchar(100) NOT NULL,
 `Identifier` varchar(255) DEFAULT NULL,
 `Name` varchar(255) DEFAULT NULL,
 `FeatureGroup_Features` bigint(20) DEFAULT NULL,
 `DesignElement_ControlType` bigint(20) DEFAULT NULL,
 `Feature_Position` bigint(20) DEFAULT NULL,
 `Zone` bigint(20) DEFAULT NULL,
 `Feature_FeatureLocation` bigint(20) DEFAULT NULL,
 `FeatureGroup` bigint(20) DEFAULT NULL,
 `Reporter_WarningType` bigint(20) DEFAULT NULL,
 PRIMARY KEY (`WID`),
 KEY `FK_DesignElement1` (`DataSetWID`),
 KEY `FK_DesignElement3` (`FeatureGroup_Features`),
 KEY `FK_DesignElement4` (`DesignElement_ControlType`),
 KEY `FK_DesignElement5` (`Feature_Position`),
 KEY `FK_DesignElement6` (`Zone`),
 KEY `FK_DesignElement7` (`Feature_FeatureLocation`),
 KEY `FK_DesignElement8` (`FeatureGroup`),
 KEY `FK_DesignElement9` (`Reporter_WarningType`),
 CONSTRAINT `FK_DesignElement1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_DesignElement3` FOREIGN KEY (`FeatureGroup_Features`) REFERENCES `designelementgroup` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_DesignElement4` FOREIGN KEY (`DesignElement_ControlType`) REFERENCES `term` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_DesignElement5` FOREIGN KEY (`Feature_Position`) REFERENCES `position_` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_DesignElement6` FOREIGN KEY (`Zone`) REFERENCES `zone` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_DesignElement7` FOREIGN KEY (`Feature_FeatureLocation`) REFERENCES `featurelocation` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_DesignElement8` FOREIGN KEY (`FeatureGroup`) REFERENCES `designelementgroup` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_DesignElement9` FOREIGN KEY (`Reporter_WarningType`) REFERENCES `term` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




