---
title: reguloncluster
---

# reguloncluster
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `reguloncluster`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `reguloncluster` (
 `clusterId` int(10) unsigned NOT NULL DEFAULT '0',
 `locusId` int(10) unsigned NOT NULL DEFAULT '0',
 `updated` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
 `link` varchar(50) NOT NULL DEFAULT 'GNScore',
 PRIMARY KEY (`clusterId`,`locusId`),
 KEY `clusterId` (`clusterId`),
 KEY `locusId` (`locusId`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




