﻿# quanttypedimenswidquanttypewid
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](./index.md)_

--
 
 DROP TABLE IF EXISTS `quanttypedimenswidquanttypewid`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `quanttypedimenswidquanttypewid` (
 `QuantitationTypeDimensionWID` bigint(20) NOT NULL,
 `QuantitationTypeWID` bigint(20) NOT NULL,
 KEY `FK_QuantTypeDimensWIDQuantTy1` (`QuantitationTypeDimensionWID`),
 KEY `FK_QuantTypeDimensWIDQuantTy2` (`QuantitationTypeWID`),
 CONSTRAINT `FK_QuantTypeDimensWIDQuantTy1` FOREIGN KEY (`QuantitationTypeDimensionWID`) REFERENCES `quantitationtypedimension` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_QuantTypeDimensWIDQuantTy2` FOREIGN KEY (`QuantitationTypeWID`) REFERENCES `quantitationtype` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




