---
title: nucleicacid
---

# nucleicacid
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `nucleicacid`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `nucleicacid` (
 `WID` bigint(20) NOT NULL,
 `Name` varchar(200) DEFAULT NULL,
 `Type` varchar(30) NOT NULL,
 `Class` varchar(30) DEFAULT NULL,
 `Topology` varchar(30) DEFAULT NULL,
 `Strandedness` varchar(30) DEFAULT NULL,
 `SequenceDerivation` varchar(30) DEFAULT NULL,
 `Fragment` char(1) DEFAULT NULL,
 `FullySequenced` char(1) DEFAULT NULL,
 `MoleculeLength` int(11) DEFAULT NULL,
 `MoleculeLengthApproximate` varchar(10) DEFAULT NULL,
 `CumulativeLength` int(11) DEFAULT NULL,
 `CumulativeLengthApproximate` varchar(10) DEFAULT NULL,
 `GeneticCodeWID` bigint(20) DEFAULT NULL,
 `BioSourceWID` bigint(20) DEFAULT NULL,
 `DataSetWID` bigint(20) NOT NULL,
 PRIMARY KEY (`WID`),
 KEY `FK_NucleicAcid1` (`GeneticCodeWID`),
 KEY `FK_NucleicAcid2` (`BioSourceWID`),
 KEY `FK_NucleicAcid3` (`DataSetWID`),
 CONSTRAINT `FK_NucleicAcid1` FOREIGN KEY (`GeneticCodeWID`) REFERENCES `geneticcode` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_NucleicAcid2` FOREIGN KEY (`BioSourceWID`) REFERENCES `biosource` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_NucleicAcid3` FOREIGN KEY (`DataSetWID`) REFERENCES `dataset` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




