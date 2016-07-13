---
title: hist
---

# hist
_namespace: [SMRUCC.genomics.Data.ExplorEnz.MySQL](N-SMRUCC.genomics.Data.ExplorEnz.MySQL.html)_

--
 
 DROP TABLE IF EXISTS `hist`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `hist` (
 `ec_num` varchar(12) NOT NULL DEFAULT '',
 `action` varchar(11) NOT NULL DEFAULT '',
 `note` text,
 `history` text,
 `class` int(1) DEFAULT NULL,
 `subclass` int(1) DEFAULT NULL,
 `subsubclass` int(1) DEFAULT NULL,
 `serial` int(1) DEFAULT NULL,
 `status` char(3) DEFAULT NULL,
 `last_change` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
 PRIMARY KEY (`ec_num`),
 UNIQUE KEY `ec_num` (`ec_num`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




