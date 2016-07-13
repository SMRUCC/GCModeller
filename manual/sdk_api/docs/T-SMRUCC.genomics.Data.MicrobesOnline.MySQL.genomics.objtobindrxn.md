---
title: objtobindrxn
---

# objtobindrxn
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.genomics.html)_

--
 
 DROP TABLE IF EXISTS `objtobindrxn`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `objtobindrxn` (
 `objectId` varchar(255) NOT NULL,
 `bindrxn` varchar(255) NOT NULL,
 UNIQUE KEY `combined` (`objectId`(250),`bindrxn`(250)),
 KEY `objectId` (`objectId`),
 KEY `bindrxn` (`bindrxn`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




