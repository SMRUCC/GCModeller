---
title: locus2seed
---

# locus2seed
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `locus2seed`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `locus2seed` (
 `locusId` int(10) unsigned NOT NULL,
 `seedId` varchar(50) NOT NULL,
 `isIdentical` tinyint(1) NOT NULL DEFAULT '0',
 PRIMARY KEY (`locusId`),
 KEY `locusId_seedId_key` (`locusId`,`seedId`),
 KEY `seedId_locusId_key` (`seedId`,`locusId`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




