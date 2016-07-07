---
title: pathway
---

# pathway
_namespace: [SMRUCC.genomics.Data.MicrobesOnline.MySQL.BioCyc](N-SMRUCC.genomics.Data.MicrobesOnline.MySQL.BioCyc.html)_

--
 
 DROP TABLE IF EXISTS `pathway`;
 /*!40101 SET @saved_cs_client = @@character_set_client */;
 /*!40101 SET character_set_client = utf8 */;
 CREATE TABLE `pathway` (
 `pathwayId` varchar(255) NOT NULL,
 `name` varchar(255) DEFAULT NULL,
 PRIMARY KEY (`pathwayId`)
 ) ENGINE=MyISAM DEFAULT CHARSET=latin1;
 /*!40101 SET character_set_client = @saved_cs_client */;
 
 --




