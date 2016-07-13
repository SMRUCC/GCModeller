---
title: locus2pdb
---

# locus2pdb
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `locus2pdb`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `locus2pdb` (
 `pdbId` varchar(6) NOT NULL DEFAULT '',
 `pdbChain` char(1) NOT NULL DEFAULT '',
 `locusId` int(10) unsigned NOT NULL DEFAULT '0',
 `version` int(2) unsigned NOT NULL DEFAULT '0',
 `seqBegin` int(5) unsigned NOT NULL DEFAULT '0',
 `seqEnd` int(5) unsigned NOT NULL DEFAULT '0',
 `pdbBegin` int(5) unsigned NOT NULL DEFAULT '0',
 `pdbEnd` int(5) unsigned NOT NULL DEFAULT '0',
 `identity` decimal(5,2) unsigned NOT NULL DEFAULT '0.00',
 `alignLength` int(5) unsigned NOT NULL DEFAULT '0',
 `mismatch` int(5) unsigned NOT NULL DEFAULT '0',
 `gap` int(5) unsigned NOT NULL DEFAULT '0',
 `evalue` double NOT NULL DEFAULT '0',
 `evalueDisp` text NOT NULL,
 `score` decimal(10,2) unsigned NOT NULL DEFAULT '0.00',
 PRIMARY KEY (`pdbId`,`pdbChain`,`locusId`,`version`,`seqBegin`,`seqEnd`),
 KEY `locusId` (`locusId`,`version`),
 KEY `pdbId` (`pdbId`,`pdbChain`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




