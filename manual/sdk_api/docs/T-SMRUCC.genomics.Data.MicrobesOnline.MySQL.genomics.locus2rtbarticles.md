---
title: locus2rtbarticles
---

# locus2rtbarticles
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `locus2rtbarticles`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `locus2rtbarticles` (
 `locusId` int(10) unsigned NOT NULL DEFAULT '0',
 `pubMedId` varchar(20) DEFAULT NULL,
 `wetExp` tinyint(1) unsigned NOT NULL DEFAULT '0',
 KEY `seqfeature_id` (`locusId`),
 KEY `pubMedId` (`pubMedId`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




