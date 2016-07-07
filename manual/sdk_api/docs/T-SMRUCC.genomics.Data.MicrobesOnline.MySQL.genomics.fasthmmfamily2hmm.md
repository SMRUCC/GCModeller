---
title: fasthmmfamily2hmm
---

# fasthmmfamily2hmm
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `fasthmmfamily2hmm`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `fasthmmfamily2hmm` (
 `domainDb` varchar(20) NOT NULL DEFAULT '',
 `domainId` varchar(20) NOT NULL DEFAULT '',
 `hmmName` varchar(10) NOT NULL DEFAULT '',
 `domainLen` int(5) unsigned DEFAULT NULL,
 PRIMARY KEY (`domainDb`,`domainId`,`hmmName`),
 KEY `domainId` (`domainId`),
 KEY `hmmName` (`hmmName`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




