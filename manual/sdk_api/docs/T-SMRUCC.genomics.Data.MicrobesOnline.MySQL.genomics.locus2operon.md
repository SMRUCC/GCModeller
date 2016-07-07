---
title: locus2operon
---

# locus2operon
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `locus2operon`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `locus2operon` (
 `locusId` int(10) unsigned DEFAULT NULL,
 `tuId` int(10) unsigned NOT NULL DEFAULT '0',
 KEY `tuId` (`tuId`),
 KEY `orfId` (`locusId`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




