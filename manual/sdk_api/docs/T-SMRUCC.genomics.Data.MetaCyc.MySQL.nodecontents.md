---
title: nodecontents
---

# nodecontents
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `nodecontents`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `nodecontents` (
 `WID` bigint(20) NOT NULL,
 `DataSetWID` bigint(20) NOT NULL,
 `Node_NodeContents` bigint(20) DEFAULT NULL,
 `BioAssayDimension` bigint(20) DEFAULT NULL,
 `DesignElementDimension` bigint(20) DEFAULT NULL,
 `QuantitationDimension` bigint(20) DEFAULT NULL,
 PRIMARY KEY (`WID`),
 KEY `FK_NodeContents1` (`DataSetWID`),
 KEY `FK_NodeContents3` (`Node_NodeContents`),
 KEY `FK_NodeContents4` (`BioAssayDimension`),
 KEY `FK_NodeContents5` (`DesignElementDimension`),
 KEY `FK_NodeContents6` (`QuantitationDimension`),
 CONSTRAINT `FK_NodeContents1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_NodeContents3` FOREIGN KEY (`Node_NodeContents`) REFERENCES `node` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_NodeContents4` FOREIGN KEY (`BioAssayDimension`) REFERENCES `bioassaydimension` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_NodeContents5` FOREIGN KEY (`DesignElementDimension`) REFERENCES `designelementdimension` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_NodeContents6` FOREIGN KEY (`QuantitationDimension`) REFERENCES `quantitationtypedimension` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




