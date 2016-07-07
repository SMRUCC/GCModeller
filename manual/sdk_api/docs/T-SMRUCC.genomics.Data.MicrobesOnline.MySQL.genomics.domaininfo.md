---
title: domaininfo
---

# domaininfo
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `domaininfo`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `domaininfo` (
 `domainDb` varchar(20) NOT NULL DEFAULT '',
 `domainId` varchar(20) NOT NULL DEFAULT '',
 `domainName` varchar(50) NOT NULL DEFAULT '',
 `iprId` varchar(10) DEFAULT NULL,
 `iprName` varchar(100) DEFAULT NULL,
 `domainLen` int(5) unsigned DEFAULT NULL,
 `fileName` varchar(100) DEFAULT NULL,
 PRIMARY KEY (`domainDb`,`domainId`),
 KEY `domainId` (`domainId`),
 KEY `iprId` (`iprId`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




