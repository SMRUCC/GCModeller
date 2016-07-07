---
title: enzrxn
---

# enzrxn
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.BioCyc](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.BioCyc.html)_

--
 
 DROP TABLE IF EXISTS `enzrxn`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `enzrxn` (
 `enzrxnId` varchar(255) NOT NULL,
 `alterSubstrate` text,
 `name` varchar(255) DEFAULT NULL,
 `enzymeId` varchar(255) NOT NULL,
 `direction` varchar(255) DEFAULT NULL,
 PRIMARY KEY (`enzrxnId`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




