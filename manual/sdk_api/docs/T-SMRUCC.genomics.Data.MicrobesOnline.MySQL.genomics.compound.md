---
title: compound
---

# compound
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `compound`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `compound` (
 `compoundId` varchar(255) NOT NULL,
 `charge` int(5) DEFAULT NULL,
 `name` varchar(255) DEFAULT NULL,
 `mw` float DEFAULT NULL,
 `pka1` float DEFAULT NULL,
 `pka2` float DEFAULT NULL,
 `pka3` float DEFAULT NULL,
 `sName` varchar(255) DEFAULT NULL,
 PRIMARY KEY (`compoundId`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




