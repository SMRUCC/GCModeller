---
title: locus2domain
---

# locus2domain
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `locus2domain`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `locus2domain` (
 `domainId` varchar(20) NOT NULL DEFAULT '',
 `locusId` int(10) unsigned NOT NULL DEFAULT '0',
 `version` int(2) unsigned NOT NULL DEFAULT '0',
 `seqBegin` int(5) unsigned NOT NULL DEFAULT '0',
 `seqEnd` int(5) unsigned NOT NULL DEFAULT '0',
 `domainBegin` int(5) unsigned NOT NULL DEFAULT '0',
 `domainEnd` int(5) unsigned NOT NULL DEFAULT '0',
 `score` float NOT NULL DEFAULT '0',
 `evalue` float NOT NULL DEFAULT '0',
 PRIMARY KEY (`domainId`,`locusId`,`version`,`seqBegin`,`seqEnd`),
 KEY `locusId` (`locusId`,`version`),
 KEY `domainId` (`domainId`),
 FULLTEXT KEY `domainIdFull` (`domainId`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




