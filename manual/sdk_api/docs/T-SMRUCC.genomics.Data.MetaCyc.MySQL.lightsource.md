---
title: lightsource
---

# lightsource
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `lightsource`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `lightsource` (
 `WID` bigint(20) NOT NULL,
 `Wavelength` float DEFAULT NULL,
 `Type` varchar(100) DEFAULT NULL,
 `InstrumentWID` bigint(20) DEFAULT NULL,
 `DataSetWID` bigint(20) NOT NULL,
 PRIMARY KEY (`WID`),
 KEY `LightSource_DWID` (`DataSetWID`),
 CONSTRAINT `FK_LightSource1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




