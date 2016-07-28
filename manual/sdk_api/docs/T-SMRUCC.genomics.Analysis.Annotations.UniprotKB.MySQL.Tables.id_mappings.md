---
title: id_mappings
---

# id_mappings
_namespace: [SMRUCC.genomics.Analysis.Annotations.UniprotKB.MySQL.Tables](N-SMRUCC.genomics.Analysis.Annotations.UniprotKB.MySQL.Tables.html)_

--
 
 DROP TABLE IF EXISTS `id_mappings`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `id_mappings` (
 `UniProtKB_AC` int(11) NOT NULL,
 `UniProtKB_ID` varchar(45) DEFAULT NULL,
 `GeneID_EntrezGene` varchar(45) DEFAULT NULL,
 `RefSeq` varchar(45) DEFAULT NULL,
 `GI` varchar(45) DEFAULT NULL,
 `pdb` varchar(45) DEFAULT NULL,
 `go` varchar(45) DEFAULT NULL,
 `UniRef100` varchar(45) DEFAULT NULL,
 `UniRef90` varchar(45) DEFAULT NULL,
 `UniRef50` varchar(45) DEFAULT NULL,
 `UniParc` varchar(45) DEFAULT NULL,
 `pir` varchar(45) DEFAULT NULL,
 `NCBI_Taxon` varchar(45) DEFAULT NULL,
 `MIM` varchar(45) DEFAULT NULL,
 `UniGene` varchar(45) DEFAULT NULL,
 `PubMed` varchar(45) DEFAULT NULL,
 `EMBL` varchar(45) DEFAULT NULL,
 `EMBL_CDS` varchar(45) DEFAULT NULL,
 `Ensembl` varchar(45) DEFAULT NULL,
 `Ensembl_TRS` varchar(45) DEFAULT NULL,
 `Ensembl_PRO` varchar(45) DEFAULT NULL,
 `Additional_PubMed` text,
 PRIMARY KEY (`UniProtKB_AC`),
 UNIQUE KEY `UniProtKB_AC_UNIQUE` (`UniProtKB_AC`)
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




