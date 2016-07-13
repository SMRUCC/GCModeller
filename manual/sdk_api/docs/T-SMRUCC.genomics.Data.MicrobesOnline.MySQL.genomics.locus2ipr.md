---
title: locus2ipr
---

# locus2ipr
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `locus2ipr`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `locus2ipr` (
 `locusId` int(10) unsigned DEFAULT NULL,
 `iprId` varchar(9) NOT NULL DEFAULT '',
 `taxonomyId` int(10) DEFAULT NULL,
 UNIQUE KEY `orf2ipr` (`locusId`,`iprId`),
 KEY `orfId` (`locusId`),
 KEY `iprId` (`iprId`),
 KEY `taxonomyId` (`taxonomyId`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




