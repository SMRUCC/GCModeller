---
title: iprinfo
---

# iprinfo
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `iprinfo`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `iprinfo` (
 `iprId` varchar(9) NOT NULL DEFAULT '',
 `type` varchar(16) DEFAULT NULL,
 `shortName` varchar(50) DEFAULT NULL,
 `proteinCount` int(5) DEFAULT NULL,
 `iprName` varchar(255) DEFAULT NULL,
 PRIMARY KEY (`iprId`),
 KEY `type` (`type`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




