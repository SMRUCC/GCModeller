﻿# derivbioassaywidbioassaymapwid
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](./index.md)_

--
 
 DROP TABLE IF EXISTS `derivbioassaywidbioassaymapwid`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `derivbioassaywidbioassaymapwid` (
 `DerivedBioAssayWID` bigint(20) NOT NULL,
 `BioAssayMapWID` bigint(20) NOT NULL,
 KEY `FK_DerivBioAssayWIDBioAssayM1` (`DerivedBioAssayWID`),
 KEY `FK_DerivBioAssayWIDBioAssayM2` (`BioAssayMapWID`),
 CONSTRAINT `FK_DerivBioAssayWIDBioAssayM1` FOREIGN KEY (`DerivedBioAssayWID`) REFERENCES `bioassay` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_DerivBioAssayWIDBioAssayM2` FOREIGN KEY (`BioAssayMapWID`) REFERENCES `bioevent` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




