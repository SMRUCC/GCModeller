---
title: locus2regprecise
---

# locus2regprecise
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `locus2regprecise`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `locus2regprecise` (
 `locusId` int(11) NOT NULL,
 `geneInOperonIndex` int(11) NOT NULL,
 `sourceTypeTermId` int(11) NOT NULL,
 `regulonId` int(11) NOT NULL,
 `regulatorName` varchar(255) NOT NULL,
 `regulatorLocusId` int(11) NOT NULL,
 `regulationMode` varchar(255) NOT NULL
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




