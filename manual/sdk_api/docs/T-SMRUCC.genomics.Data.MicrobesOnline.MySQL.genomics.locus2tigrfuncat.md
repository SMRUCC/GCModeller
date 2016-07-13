---
title: locus2tigrfuncat
---

# locus2tigrfuncat
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `locus2tigrfuncat`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `locus2tigrfuncat` (
 `locusId` int(10) unsigned NOT NULL DEFAULT '0',
 `mainRole` varchar(255) DEFAULT NULL,
 `subRole` varchar(255) DEFAULT NULL,
 `evidence` varchar(64) DEFAULT NULL,
 PRIMARY KEY (`locusId`),
 KEY `mainRole` (`mainRole`),
 KEY `subRole` (`subRole`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




