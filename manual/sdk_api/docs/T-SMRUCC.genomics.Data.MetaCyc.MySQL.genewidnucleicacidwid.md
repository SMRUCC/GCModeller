---
title: genewidnucleicacidwid
---

# genewidnucleicacidwid
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `genewidnucleicacidwid`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `genewidnucleicacidwid` (
 `GeneWID` bigint(20) NOT NULL,
 `NucleicAcidWID` bigint(20) NOT NULL,
 KEY `FK_GeneWIDNucleicAcidWID1` (`GeneWID`),
 KEY `FK_GeneWIDNucleicAcidWID2` (`NucleicAcidWID`),
 CONSTRAINT `FK_GeneWIDNucleicAcidWID1` FOREIGN KEY (`GeneWID`) REFERENCES `gene` (`WID`) ON DELETE CASCADE,
 CONSTRAINT `FK_GeneWIDNucleicAcidWID2` FOREIGN KEY (`NucleicAcidWID`) REFERENCES `nucleicacid` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




