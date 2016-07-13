---
title: swissprot
---

# swissprot
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `swissprot`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `swissprot` (
 `id` varchar(20) NOT NULL DEFAULT '',
 `accession` varchar(20) NOT NULL DEFAULT '',
 `length` int(10) NOT NULL DEFAULT '0',
 `description` text NOT NULL,
 PRIMARY KEY (`id`,`accession`),
 UNIQUE KEY `id` (`id`),
 KEY `accession` (`accession`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




