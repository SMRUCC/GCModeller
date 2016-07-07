---
title: swissprot2locus
---

# swissprot2locus
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `swissprot2locus`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `swissprot2locus` (
 `locusId` int(10) unsigned NOT NULL DEFAULT '0',
 `version` int(2) unsigned NOT NULL DEFAULT '0',
 `id` varchar(20) NOT NULL DEFAULT '',
 `accession` varchar(20) NOT NULL DEFAULT '',
 `identity` int(3) unsigned NOT NULL DEFAULT '0',
 PRIMARY KEY (`id`,`accession`),
 UNIQUE KEY `id` (`id`),
 KEY `locusId` (`locusId`),
 KEY `accession` (`accession`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




