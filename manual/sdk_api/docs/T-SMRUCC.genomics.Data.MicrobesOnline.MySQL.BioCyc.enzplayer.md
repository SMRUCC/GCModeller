---
title: enzplayer
---

# enzplayer
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.BioCyc](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.BioCyc.html)_

--
 
 DROP TABLE IF EXISTS `enzplayer`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `enzplayer` (
 `enzrxnId` varchar(255) NOT NULL,
 `player` varchar(255) NOT NULL,
 `playerId` varchar(255) NOT NULL,
 UNIQUE KEY `combined` (`enzrxnId`(250),`playerId`(250)),
 KEY `enzrxnId` (`enzrxnId`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




