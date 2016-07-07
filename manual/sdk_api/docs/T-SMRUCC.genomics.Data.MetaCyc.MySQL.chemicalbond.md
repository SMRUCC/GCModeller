---
title: chemicalbond
---

# chemicalbond
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `chemicalbond`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `chemicalbond` (
 `ChemicalWID` bigint(20) NOT NULL,
 `Atom1Index` smallint(6) NOT NULL,
 `Atom2Index` smallint(6) NOT NULL,
 `BondType` smallint(6) NOT NULL,
 `BondStereo` decimal(10,0) DEFAULT NULL,
 KEY `FK_ChemicalBond` (`ChemicalWID`),
 CONSTRAINT `FK_ChemicalBond` FOREIGN KEY (`ChemicalWID`) REFERENCES `chemical` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




