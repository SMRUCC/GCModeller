---
title: locus2go
---

# locus2go
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `locus2go`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `locus2go` (
 `locusId` int(10) unsigned DEFAULT NULL,
 `goId` int(10) unsigned NOT NULL DEFAULT '0',
 `evidence` varchar(255) NOT NULL DEFAULT '',
 UNIQUE KEY `orf2go` (`locusId`,`goId`),
 KEY `orfId` (`locusId`),
 KEY `goId` (`goId`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




