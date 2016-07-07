---
title: tigrinfo
---

# tigrinfo
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `tigrinfo`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `tigrinfo` (
 `id` varchar(16) DEFAULT NULL,
 `tigrId` varchar(30) NOT NULL DEFAULT '',
 `type` varchar(32) DEFAULT NULL,
 `roleId` int(5) DEFAULT NULL,
 `geneSymbol` varchar(10) DEFAULT NULL,
 `ec` varchar(16) DEFAULT NULL,
 `definition` longtext,
 PRIMARY KEY (`tigrId`),
 KEY `type` (`type`),
 KEY `roleId` (`roleId`),
 KEY `ec` (`ec`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




