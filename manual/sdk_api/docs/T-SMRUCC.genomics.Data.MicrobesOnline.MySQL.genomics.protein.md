---
title: protein
---

# protein
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `protein`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `protein` (
 `taxId` int(20) unsigned NOT NULL,
 `proteinId` varchar(100) NOT NULL,
 `name` varchar(20) DEFAULT NULL,
 `dnaFootprintSize` int(5) DEFAULT NULL,
 `geneId` varchar(100) DEFAULT NULL,
 `location` varchar(100) DEFAULT NULL,
 `modifiedForm` varchar(255) DEFAULT NULL,
 `mw` float DEFAULT NULL,
 `mwSeq` float DEFAULT NULL,
 `mwExp` float DEFAULT NULL,
 `neidhardtSpotNum` varchar(50) DEFAULT NULL,
 `PI` float DEFAULT NULL,
 `symmetry` varchar(50) DEFAULT NULL,
 `type` varchar(255) DEFAULT NULL,
 `unmodifiedForm` varchar(255) DEFAULT NULL,
 PRIMARY KEY (`proteinId`),
 KEY `taxId` (`taxId`),
 KEY `geneId` (`geneId`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




