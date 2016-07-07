---
title: pathwayplayer
---

# pathwayplayer
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `pathwayplayer`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `pathwayplayer` (
 `pathwayId` varchar(255) NOT NULL,
 `player` varchar(255) DEFAULT NULL,
 `playerId` varchar(255) DEFAULT NULL,
 UNIQUE KEY `combined` (`pathwayId`(250),`playerId`(250)),
 KEY `pathwayId` (`pathwayId`),
 KEY `player` (`player`),
 KEY `playerId` (`playerId`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 
 
 --




