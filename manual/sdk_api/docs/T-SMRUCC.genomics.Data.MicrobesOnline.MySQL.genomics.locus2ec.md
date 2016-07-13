---
title: locus2ec
---

# locus2ec
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `locus2ec`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `locus2ec` (
 `locusId` int(10) unsigned DEFAULT NULL,
 `version` int(2) unsigned DEFAULT '1',
 `scaffoldId` int(10) unsigned NOT NULL DEFAULT '0',
 `ecNum` varchar(20) NOT NULL DEFAULT '',
 `evidence` varchar(50) DEFAULT NULL,
 UNIQUE KEY `combined` (`locusId`,`ecNum`,`version`),
 KEY `scaffoldId` (`scaffoldId`),
 KEY `ecNum` (`ecNum`),
 KEY `locusId` (`locusId`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




