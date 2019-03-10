﻿# locus2swissprot
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](./index.md)_

--
 
 DROP TABLE IF EXISTS `locus2swissprot`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `locus2swissprot` (
 `locusId` int(10) unsigned NOT NULL DEFAULT '0',
 `version` int(2) unsigned NOT NULL DEFAULT '0',
 `id` varchar(20) NOT NULL DEFAULT '',
 `accession` varchar(20) NOT NULL DEFAULT '',
 `identity` int(3) unsigned NOT NULL DEFAULT '0',
 `bidir` int(2) unsigned NOT NULL DEFAULT '0',
 KEY `accession` (`accession`),
 KEY `id` (`id`),
 KEY `locusId` (`locusId`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




