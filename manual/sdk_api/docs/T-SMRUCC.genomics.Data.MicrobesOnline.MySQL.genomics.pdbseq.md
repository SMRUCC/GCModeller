---
title: pdbseq
---

# pdbseq
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `pdbseq`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `pdbseq` (
 `pdbId` varchar(6) NOT NULL DEFAULT '',
 `pdbChain` char(1) NOT NULL DEFAULT '',
 `version` int(2) unsigned NOT NULL DEFAULT '0',
 `sequence` longblob NOT NULL,
 PRIMARY KEY (`pdbId`,`pdbChain`,`version`),
 KEY `pdbId` (`pdbId`,`pdbChain`,`version`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




