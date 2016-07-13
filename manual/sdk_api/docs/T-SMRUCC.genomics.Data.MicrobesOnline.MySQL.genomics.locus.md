---
title: locus
---

# locus
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `locus`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `locus` (
 `locusId` int(10) unsigned NOT NULL AUTO_INCREMENT,
 `version` int(2) unsigned NOT NULL DEFAULT '1',
 `priority` tinyint(3) unsigned DEFAULT NULL,
 `created` date DEFAULT NULL,
 `posId` int(10) unsigned DEFAULT NULL,
 `evidence` varchar(50) DEFAULT NULL,
 `scaffoldId` int(10) unsigned DEFAULT NULL,
 `type` smallint(5) unsigned DEFAULT NULL,
 `exonCoords` text,
 `cdsCoords` text,
 PRIMARY KEY (`locusId`,`version`),
 KEY `Indx_ORF_PosId` (`posId`),
 KEY `Indx_ORF_OrfId` (`locusId`),
 KEY `scaffoldId` (`scaffoldId`),
 KEY `priority` (`priority`)
 ) ENGINE=MyISAM AUTO_INCREMENT=11826436 DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




