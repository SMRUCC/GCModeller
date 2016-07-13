---
title: pdbreps
---

# pdbreps
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `pdbreps`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `pdbreps` (
 `pdbIdRep` varchar(6) NOT NULL DEFAULT '',
 `pdbChainRep` char(1) NOT NULL DEFAULT '',
 `pdbId` varchar(6) NOT NULL DEFAULT '',
 `pdbChain` char(1) NOT NULL DEFAULT '',
 PRIMARY KEY (`pdbIdRep`,`pdbChainRep`,`pdbId`,`pdbChain`),
 KEY `pdbIdRep` (`pdbIdRep`,`pdbChainRep`),
 KEY `pdbId` (`pdbId`,`pdbChain`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




