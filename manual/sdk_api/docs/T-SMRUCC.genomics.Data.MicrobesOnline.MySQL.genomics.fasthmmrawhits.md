---
title: fasthmmrawhits
---

# fasthmmrawhits
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `fasthmmrawhits`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `fasthmmrawhits` (
 `hmmId` varchar(20) NOT NULL DEFAULT '',
 `locusId` int(10) unsigned NOT NULL DEFAULT '0',
 `version` int(2) unsigned NOT NULL DEFAULT '0',
 `seqBegin` int(5) unsigned NOT NULL DEFAULT '0',
 `seqEnd` int(5) unsigned NOT NULL DEFAULT '0',
 `domainBegin` int(5) unsigned NOT NULL DEFAULT '0',
 `domainEnd` int(5) unsigned NOT NULL DEFAULT '0',
 `score` float NOT NULL DEFAULT '0',
 `evalue` float NOT NULL DEFAULT '0',
 PRIMARY KEY (`hmmId`,`locusId`,`version`,`seqBegin`),
 KEY `locusId` (`locusId`,`version`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1 MAX_ROWS=1000000000;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




