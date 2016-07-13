---
title: ipr2go
---

# ipr2go
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `ipr2go`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `ipr2go` (
 `iprId` varchar(10) NOT NULL DEFAULT '',
 `goId` varchar(10) NOT NULL DEFAULT '',
 PRIMARY KEY (`iprId`,`goId`),
 KEY `iprId` (`iprId`),
 KEY `goId` (`goId`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




