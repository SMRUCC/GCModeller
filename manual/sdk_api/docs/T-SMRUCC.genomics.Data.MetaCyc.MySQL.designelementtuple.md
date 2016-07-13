---
title: designelementtuple
---

# designelementtuple
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `designelementtuple`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `designelementtuple` (
 `WID` bigint(20) NOT NULL,
 `DataSetWID` bigint(20) NOT NULL,
 `BioAssayTuple` bigint(20) DEFAULT NULL,
 `DesignElement` bigint(20) DEFAULT NULL,
 PRIMARY KEY (`WID`),
 KEY `FK_DesignElementTuple1` (`DataSetWID`),
 KEY `FK_DesignElementTuple2` (`BioAssayTuple`),
 KEY `FK_DesignElementTuple3` (`DesignElement`),
 CONSTRAINT `FK_DesignElementTuple1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_DesignElementTuple2` FOREIGN KEY (`BioAssayTuple`) REFERENCES `bioassaytuple` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_DesignElementTuple3` FOREIGN KEY (`DesignElement`) REFERENCES `designelement` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




