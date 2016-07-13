---
title: interpro
---

# interpro
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `interpro`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `interpro` (
 `locusId` int(10) unsigned NOT NULL DEFAULT '0',
 `version` int(2) unsigned NOT NULL DEFAULT '1',
 `checksum` varchar(16) DEFAULT NULL,
 `length` int(5) unsigned DEFAULT NULL,
 `domainDb` varchar(30) DEFAULT NULL,
 `domainId` varchar(30) NOT NULL DEFAULT '',
 `domainDesc` varchar(255) DEFAULT NULL,
 `domainStart` int(5) NOT NULL DEFAULT '0',
 `domainEnd` int(5) DEFAULT NULL,
 `evalue` float DEFAULT NULL,
 `status` varchar(10) DEFAULT NULL,
 `date` varchar(50) DEFAULT NULL,
 `iprId` varchar(9) DEFAULT NULL,
 `iprName` varchar(255) DEFAULT NULL,
 `geneOntology` longtext,
 PRIMARY KEY (`locusId`,`version`,`domainId`,`domainStart`),
 KEY `locusId` (`locusId`,`version`),
 KEY `iprId` (`iprId`),
 KEY `checksum` (`checksum`),
 KEY `domainId` (`domainId`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




