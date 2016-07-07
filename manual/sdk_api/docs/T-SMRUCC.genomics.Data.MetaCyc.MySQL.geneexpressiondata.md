---
title: geneexpressiondata
---

# geneexpressiondata
_namespace: [SMRUCC.genomics.Data.MetaCyc.MySQL](N-SMRUCC.genomics.Data.MetaCyc.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `geneexpressiondata`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `geneexpressiondata` (
 `B` smallint(6) NOT NULL,
 `D` smallint(6) NOT NULL,
 `Q` smallint(6) NOT NULL,
 `Value` varchar(100) NOT NULL,
 `BioAssayValuesWID` bigint(20) NOT NULL,
 KEY `FK_GEDBAV` (`BioAssayValuesWID`),
 CONSTRAINT `FK_GEDBAV` FOREIGN KEY (`BioAssayValuesWID`) REFERENCES `biodatavalues` (`WID`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




