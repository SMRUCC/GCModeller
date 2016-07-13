---
title: pdbentries
---

# pdbentries
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `pdbentries`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `pdbentries` (
 `pdbId` varchar(6) NOT NULL DEFAULT '',
 `header` text NOT NULL,
 `ascessionDate` date DEFAULT NULL,
 `compound` text NOT NULL,
 `source` text,
 `authorList` text,
 `resolution` float DEFAULT NULL,
 `experimentType` text,
 `dbSource` text,
 PRIMARY KEY (`pdbId`),
 KEY `pdbId` (`pdbId`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




