---
title: tigrroles
---

# tigrroles
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `tigrroles`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `tigrroles` (
 `roleId` int(5) NOT NULL DEFAULT '0',
 `level` enum('main','sub1') NOT NULL DEFAULT 'main',
 `description` varchar(255) NOT NULL DEFAULT '',
 KEY `roleId` (`roleId`),
 KEY `description` (`description`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




