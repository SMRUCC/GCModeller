---
title: quantitationtypetuple
---

# quantitationtypetuple
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `quantitationtypetuple`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `quantitationtypetuple` (
 `WID` bigint(20) NOT NULL,
 `DataSetWID` bigint(20) NOT NULL,
 `DesignElementTuple` bigint(20) DEFAULT NULL,
 `QuantitationType` bigint(20) DEFAULT NULL,
 `QuantitationTypeTuple_Datum` bigint(20) DEFAULT NULL,
 PRIMARY KEY (`WID`),
 KEY `FK_QuantitationTypeTuple1` (`DataSetWID`),
 KEY `FK_QuantitationTypeTuple2` (`DesignElementTuple`),
 KEY `FK_QuantitationTypeTuple3` (`QuantitationType`),
 KEY `FK_QuantitationTypeTuple4` (`QuantitationTypeTuple_Datum`),
 CONSTRAINT `FK_QuantitationTypeTuple1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_QuantitationTypeTuple2` FOREIGN KEY (`DesignElementTuple`) REFERENCES `designelementtuple` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_QuantitationTypeTuple3` FOREIGN KEY (`QuantitationType`) REFERENCES `quantitationtype` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_QuantitationTypeTuple4` FOREIGN KEY (`QuantitationTypeTuple_Datum`) REFERENCES `datum` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




