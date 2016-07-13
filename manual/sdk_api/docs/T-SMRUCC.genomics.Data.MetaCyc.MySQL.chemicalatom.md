---
title: chemicalatom
---

# chemicalatom
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `chemicalatom`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `chemicalatom` (
 `ChemicalWID` bigint(20) NOT NULL,
 `AtomIndex` smallint(6) NOT NULL,
 `Atom` varchar(2) NOT NULL,
 `Charge` smallint(6) NOT NULL,
 `X` decimal(10,0) DEFAULT NULL,
 `Y` decimal(10,0) DEFAULT NULL,
 `Z` decimal(10,0) DEFAULT NULL,
 `StereoParity` decimal(10,0) DEFAULT NULL,
 UNIQUE KEY `UN_ChemicalAtom` (`ChemicalWID`,`AtomIndex`),
 CONSTRAINT `FK_ChemicalAtom` FOREIGN KEY (`ChemicalWID`) REFERENCES `chemical` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




