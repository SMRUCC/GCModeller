---
title: quanttypewidconfidenceindwid
---

# quanttypewidconfidenceindwid
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `quanttypewidconfidenceindwid`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `quanttypewidconfidenceindwid` (
 `QuantitationTypeWID` bigint(20) NOT NULL,
 `ConfidenceIndicatorWID` bigint(20) NOT NULL,
 KEY `FK_QuantTypeWIDConfidenceInd1` (`QuantitationTypeWID`),
 KEY `FK_QuantTypeWIDConfidenceInd2` (`ConfidenceIndicatorWID`),
 CONSTRAINT `FK_QuantTypeWIDConfidenceInd1` FOREIGN KEY (`QuantitationTypeWID`) REFERENCES `quantitationtype` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_QuantTypeWIDConfidenceInd2` FOREIGN KEY (`ConfidenceIndicatorWID`) REFERENCES `quantitationtype` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




