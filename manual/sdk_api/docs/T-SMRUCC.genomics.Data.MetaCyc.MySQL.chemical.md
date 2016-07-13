---
title: chemical
---

# chemical
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `chemical`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `chemical` (
 `WID` bigint(20) NOT NULL,
 `Name` varchar(255) NOT NULL,
 `Class` char(1) DEFAULT NULL,
 `BeilsteinName` varchar(50) DEFAULT NULL,
 `SystematicName` varchar(255) DEFAULT NULL,
 `CAS` varchar(50) DEFAULT NULL,
 `Charge` smallint(6) DEFAULT NULL,
 `EmpiricalFormula` varchar(50) DEFAULT NULL,
 `MolecularWeightCalc` float DEFAULT NULL,
 `MolecularWeightExp` float DEFAULT NULL,
 `OctH2OPartitionCoeff` varchar(50) DEFAULT NULL,
 `PKA1` float DEFAULT NULL,
 `PKA2` float DEFAULT NULL,
 `PKA3` float DEFAULT NULL,
 `WaterSolubility` char(1) DEFAULT NULL,
 `Smiles` varchar(255) DEFAULT NULL,
 `DataSetWID` bigint(20) NOT NULL,
 PRIMARY KEY (`WID`),
 KEY `CHEMICAL_DWID_NAME` (`DataSetWID`,`Name`),
 KEY `CHEMICAL_NAME` (`Name`),
 CONSTRAINT `FK_Chemical1` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




